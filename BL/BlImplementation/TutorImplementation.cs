
using DO;
using Helpers;
using System;
using System.Xml.Linq;

namespace BlImplementation;

internal class TutorImplementation : BlApi.ITutor
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Tutor boTutor)
    {
        try
        {
            TutorManager.Validation(ref boTutor);
        }
        catch (Exception error)
        {
            throw error;
        }

        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, TutorManager.HashPassword(boTutor.Password),
            boTutor.CurrentAddress, boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
            boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            _dal.Tutor.Create(doTutor);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Tutor with ID={doTutor.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        var assignments = _dal.Assignment.ReadAll(a => a.TutorId == id).ToList();
        if (assignments.Count > 0)
        {
            throw new BO.BlCanNotBeDeletedException($"Tutor with ID={id} has assignment/s ,he can't be deleted");

        }
        try
        {

            _dal.Tutor.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Tutor with ID={id} does Not exist", ex);
        }
    }

    public BO.Role LogIn(int id, string password)
    {
        DO.Tutor doTutor= _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id) ??
            throw new BO.BlDoesNotExistException($"Tutor with ID={id} does Not exist");
       
        if (TutorManager.VerifyPassword(password,doTutor.Password ) )
        {
            throw new BO.BlValidationException($"Password isn't correct");
        }
        return (BO.Role)doTutor.Role;
    }

    public BO.Tutor Read(int id)
    {

        var doTutor = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id) ??
       throw new BO.BlDoesNotExistException($"Tutor with ID={id} does Not exist");
        var doAssignment =  _dal.Assignment.Read(a => a.TutorId == doTutor.Id && a.EndTime != null) ;
        var doStudentCall =  _dal.StudentCall.Read(doAssignment.StudentCallId) ;
        BO.CallStatus? status=null;
        double distance=0.0;
        if (doStudentCall != null)
        {
             status = StudentCallManager.GetCallStatus(doStudentCall);
             distance = Tools.CalculateDistance(id, doStudentCall.Latitude, doStudentCall.Longitude);

        }

        return new()
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
            TotalCallsSelfCanceled = TutorManager.countCallsByEndStatus(id, BO.EndOfTreatment.SelfCancel),
            TotalCallsHandled = TutorManager.countCallsByEndStatus(id, BO.EndOfTreatment.Treated),
            TotalCallsExpired = TutorManager.countCallsByEndStatus(id, BO.EndOfTreatment.Expired),
            CurrentCallInProgress = doAssignment != null ? new BO.CallInProgress()
            {
                Id = doTutor.Id,
                CallId = doStudentCall.Id,
                Subject = (BO.Subjects)doStudentCall.Subject,
                Description = doStudentCall.Description!,
                FullAddress = doStudentCall.FullAddress,
                OpenTime = doStudentCall.OpenTime,
                MaxEndTime = doStudentCall.FinalTime,
                EntryTime = doAssignment!.EntryTime,
                Distance = distance,
                Status = (BO.CallStatus)status!,
            } : null
        };
    }

    public IEnumerable<BO.TutorInList> SortTutorsInList(bool? isActive, BO.TutorSortField? sortField = BO.TutorSortField.Id)
    {
        IEnumerable<BO.TutorInList> tutorInLists;
        try
        {
            List<BO.TutorInList> tutorsInList = TutorManager.GetTutorsInList(isActive);
            return Tools.SortByField<BO.TutorInList>(tutorsInList, sortField.ToString());
        }
        catch (Exception ex)
        {
            throw new Exception();
        }
    }

    public void Update(int id, BO.Tutor boTutor)
    {
        try
        {
            TutorManager.Validation(ref boTutor);

        }
        catch (Exception error)
        {
            throw error;
        }
        bool isManager = _dal.Tutor.Read((DO.Tutor tutor) => tutor.Id == id).Role == DO.Role.Manager;

        if (id != boTutor.Id || isManager)
        {
            throw new Exception();
        }
        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, boTutor.Password,
            boTutor.CurrentAddress, boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
            boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);
        try
        {
            _dal.Tutor.Update(doTutor);
        }
        catch (DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Tutor with ID={id} does Not exist", ex);
        }
    }
}
