
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
        catch(Exception error)
        {
            throw error;
        }
       
        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, boTutor.Password, 
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
        var assignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.TutorId == id).ToList();
        if (assignments.Count > 0) {
            throw new Exception();

        }
        try
        {
            
            _dal.Tutor.Delete(id);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    public BO.Role LogIn(string name, string password)
    {
        var doTutor=_dal.Tutor.Read((DO.Tutor tutor)=>tutor.FullName==name);
        if (doTutor == null)
        {
            throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        }
        if (doTutor.Password != password)
        {


            throw new BO.PasswordException($"Student with ID={id} does Not exist");

        }
        return (BO.Role)doTutor.Role;
    }

    public BO.Tutor Read(int id)
    {
        var doTutor = _dal.Tutor.Read(id) ??
            throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var doAssignment=_dal.Assignment.Read((DO.Assignment assignment)=>assignment.TutorId==doTutor.id) ?? 
            throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var doStudentCall=_dal.StudentCall.Read(doAssignment.StudentCallId) ??
            throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var status= StudentCallManager.GetCallStatus(doStudentCall);
        double distance = TutorManager.GetDistance(doTutor,doStudentCall);
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
            Role = doTutor.Role,
            Active = doTutor.Active,
            Distance = doTutor.Distance,
            DistanceType = doTutor.DistanceType,
            CurrentCallInProgress = new()
            {
                Id = doTutor.Id,
                CallId = doStudentCall.Id,
                Subject = (BO.Subjects)doStudentCall.Subject,
                Description = doStudentCall.Description,
                FullAddress = doStudentCall.FullAddress,
                OpenTime = doStudentCall.OpenTime,
                MaxEndTime = doStudentCall.FinalTime,
                EntryTime = doAssignment.EntryTime,
                Distance = distance,
                Status = status,
            }
        };
    }

    public IEnumerable<BO.TutorInList> SortTutorsInList(bool ?isActive, BO.TutorSortField ?sortField=BO.TutorSortField.Id)
    {
        IEnumerable<BO.TutorInList> tutorInLists;
        try
        {
            List<BO.TutorInList> tutorsInList = TutorManager.GetTutorsInList(isActive);
            return Tools.SortByField<BO.TutorInList>(tutorsInList, sortField.ToString());
        }
        catch (Exception ex) {
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
        catch (Exception ex)
        {
            throw new Exception();
        }
    }
}
