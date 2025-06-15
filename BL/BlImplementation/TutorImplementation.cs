using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Linq;

namespace BlImplementation;

internal class TutorImplementation : BlApi.ITutor
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    #region Stage 5
    public void AddObserver(Action listObserver) =>
    TutorManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
TutorManager.Observers.AddObserver(id, observer); //stage 5k
    public void RemoveObserver(Action listObserver) =>
TutorManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
TutorManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


    /// <summary>
    /// Creates a new tutor in the system after validating the provided tutor object.
    /// </summary>
    /// <param name="boTutor">The business object representing the tutor to be created.</param>
    public void Create(BO.Tutor boTutor)
    {
        try
        {
            // Validate the provided tutor object before proceeding with creation.
            TutorManager.Validation(ref boTutor);
        }
        catch (BO.BlValidationException error)
        {
            // If validation fails, rethrow the exception.
            throw error;
        }

        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email,
                               TutorManager.HashPassword(boTutor.Password), boTutor.CurrentAddress,
                               boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
                               boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            // Attempt to create the tutor in the data access layer (DAL).
            _dal.Tutor.Create(doTutor);
            TutorManager.Observers.NotifyListUpdated(); //stage 5                                                    
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            // If tutor already exists, throw a custom exception indicating that.
            throw new BO.BlAlreadyExistsException($"Tutor with ID={doTutor.Id} already exists", ex);
        }
    }

    /// <summary>
    /// Deletes a tutor from the system. Throws an exception if the tutor has assignments.
    /// </summary>
    /// <param name="id">The ID of the tutor to be deleted.</param>
    public void Delete(int id)
    {
        // If the tutor has active assignments, prevent deletion.

        if (_dal.Assignment.ReadAll(a => a.TutorId == id).Any())
        {
            throw new BO.BlCanNotBeDeletedException($"Tutor with ID={id} has assignment/s, they can't be deleted");
        }

        try
        {
            // Attempt to delete the tutor from the DAL.
            _dal.Tutor.Delete(id);
            TutorManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            // If the tutor does not exist, throw an exception.
            throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist", ex);
        }
    }

    /// <summary>
    /// Allows a tutor to log in by verifying their credentials.
    /// </summary>
    /// <param name="id">The ID of the tutor attempting to log in.</param>
    /// <param name="password">The password entered by the tutor.</param>
    /// <returns>The role of the logged-in tutor.</returns>
    public BO.Role LogIn(int id, string password)
    {
        // Retrieve the tutor by ID from the DAL.
        var doTutor = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id) ??
       throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist");

        // Verify the provided password against the stored password.
        if (!TutorManager.VerifyPassword(password, doTutor.Password))
        {
            throw new BO.BlValidationException("Password isn't correct");
        }

        // Return the role of the tutor.
        return (BO.Role)doTutor.Role;
    }

    /// <summary>
    /// Retrieves detailed information about a specific tutor.
    /// </summary>
    /// <param name="id">The ID of the tutor to be retrieved.</param>
    /// <returns>A business object containing detailed tutor information.</returns>
    public BO.Tutor Read(int id)
    {
        // Retrieve the tutor from the DAL.
        var doTutor = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id) ??
       throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist");
        // Retrieve the tutor's assignment if available.
        var doAssignment = _dal.Assignment.Read(a => a.TutorId == doTutor.Id && a.EndTime == null );
        BO.CallStatus? status = null;
        double distance = 0.0;
        DO.StudentCall doStudentCall = null;

        if (doAssignment != null)
        {
            // If there is an assignment, retrieve the associated student call.
            doStudentCall = _dal.StudentCall.Read(doAssignment.StudentCallId);
            status = StudentCallManager.CalculateCallStatus(doStudentCall);
            distance = Tools.CalculateDistance(id, doStudentCall.Latitude, doStudentCall.Longitude);
        }

        bool hasCallInProgress = (status != null && status == BO.CallStatus.InProgress || status == BO.CallStatus.InProgressAtRisk);

        // Construct and return the detailed tutor information.
        return new BO.Tutor()
        {
            Id = id,
            FullName = doTutor.FullName,
            CellNumber = doTutor.CellNumber,
            Email = doTutor.Email,
            Password = doTutor.Password,
            CurrentAddress = doTutor.CurrentAddress,
            Latitude = doTutor.Latitude,
            Longitude = doTutor.Longitude,
            Role = (BO.Role)doTutor.Role,
            Active = doTutor.Active,
            Distance = doTutor.Distance,
            DistanceType = (BO.DistanceType)doTutor.DistanceType,
            TotalCallsSelfCanceled = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.SelfCancel),
            TotalCallsHandled = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.Treated),
            TotalCallsExpired = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.Expired),
            CurrentCallInProgress = hasCallInProgress ? new BO.CallInProgress()
            {
                Id = doAssignment.Id,
                CallId = doAssignment.StudentCallId,
                Subject = (BO.Subjects)doStudentCall.Subject,
                Description = doStudentCall.Description,
                FullAddress = doStudentCall.FullAddress,
                OpenTime = doStudentCall.OpenTime,
                MaxEndTime = doStudentCall.FinalTime,
                EntryTime = doAssignment!.EntryTime,
                Distance = distance,
                Status = (BO.CallStatus)status!,
            } : null
        };
    }

    /// <summary>
    /// Sorts the tutors list based on the specified criteria and returns the sorted list.
    /// </summary>
    /// <param name="isActive">The active status of the tutors to filter by. Can be null to ignore this filter.</param>
    /// <param name="sortField">The field to sort the tutors by.</param>
    /// <returns>A sorted list of tutors based on the given criteria.</returns>
    public IEnumerable<BO.TutorInList> SortTutorsInList(BO.TutorField? sortField = BO.TutorField.Id)
    {
        // Retrieve tutors from the DAL based on the active status filter.
        IEnumerable<DO.Tutor> doTutor = _dal.Tutor.ReadAll();

        // Convert the retrieved tutors from DAL objects to BO objects.
        IEnumerable<BO.TutorInList> tutorsInList = doTutor.Select(TutorManager.ConvertFromDoToBo).ToList();

        return tutorsInList.OrderBy(item =>
            item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
    }

    /// <summary>
    /// Updates the details of a tutor, ensuring that only authorized users can perform the update.
    /// </summary>
    /// <param name="id">The ID of the tutor who asks to update.</param>
    /// <param name="boTutor">The business object containing the updated tutor information.</param>
    public void Update(int id, BO.Tutor boTutor)
    {

        var oldDoTutor = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == boTutor.Id) ??
    throw new BO.BlDoesNotExistException($"Tutor with ID={boTutor.Id} does not exist");


        bool isManager = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id).Role == DO.Role.Manager;

        if (oldDoTutor.Role != (DO.Role)boTutor.Role && !isManager)
            throw new BO.BlValidationException("You are not authorized to change the tutor's role.");

        if (id != boTutor.Id && !isManager)
        {
            // If the user is not authorized, throw a validation exception.
            throw new BO.BlValidationException("You are not authorized to update the tutor.");
        }
        if (boTutor.CurrentCallInProgress != null && !boTutor.Active)
        {
            // If the tutor is trying to update their status to inactive while having a call in progress, throw an exception.
            throw new BO.BlValidationException("You cannot set the tutor to inactive while there is a call in progress.");
        }

        try
        {
            // Validate the provided tutor object before proceeding with the update.
            TutorManager.Validation(ref boTutor);
        }
        catch (BO.BlValidationException error)
        {
            // If validation fails, rethrow the exception.
            throw error;
        }

        DO.Tutor newDoTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, boTutor.Password,
            boTutor.CurrentAddress, boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
            boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            // Attempt to update the tutor in the DAL.
            _dal.Tutor.Update(newDoTutor);
            TutorManager.Observers.NotifyItemUpdated(boTutor.Id);
            TutorManager.Observers.NotifyListUpdated();
        }
        catch (DalDoesNotExistException ex)
        {
            // If the tutor does not exist, throw an exception.
            throw new BO.BlDoesNotExistException($"Tutor with ID={boTutor.Id} does not exist", ex);
        }
    }

    public IEnumerable<BO.TutorInList> FilterTutorsInList(BO.TutorField? tutorField = null, object? filterValue = null)
    {
        var doTutors = _dal.Tutor.ReadAll();
        IEnumerable<BO.TutorInList> tutorsInList = doTutors.Select(TutorManager.ConvertFromDoToBo);
        if (filterValue != null)
            tutorsInList = tutorsInList.Where(tutor => tutor.GetType().GetProperty(tutorField.ToString()).GetValue(tutor).ToString().Equals(filterValue));

        return tutorsInList;
    }
}
