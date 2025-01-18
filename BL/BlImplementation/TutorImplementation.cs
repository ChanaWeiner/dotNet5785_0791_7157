
using BO;
using Helpers;
using System;

namespace BlImplementation;

internal class TutorImplementation : BlApi.ITutor
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Tutor boTutor)
    {
        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, boTutor.Password, 
            boTutor.CurrentAddress, boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
            boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            _dal.Tutor.Create(doTutor);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={doTutor.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Role GetRole(string name, string password)
    {
        throw new NotImplementedException();
    }

    public BO.Tutor Read(int id)
    {
        var doTutor = _dal.Tutor.Read(id) ?? throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var doAssignment=_dal.Assignment.Read((DO.Assignment assignment)=>assignment.TutorId==doTutor.id) ?? throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var doStudentCall=_dal.StudentCall.Read(doAssignment.StudentCallId) ?? throw new BO.BlDoesNotExistException($"Student with ID={id} does Not exist");
        var status= TutorManager.GetCallStatus(doStudentCall);
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
                Subject = (Subjects)doStudentCall.Subject,
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

    public IEnumerable<BO.TutorInList> SortTutorsInList(bool isActive, BO.TutorSortField sortField)
    {
        IEnumerable<BO.TutorInList> tutorInLists = null;
        try
        {
            List<BO.TutorInList> tutorsInList = TutorManager.GetTutorsInList();
            return TutorManager.SortByField<BO.TutorInList>(tutorsInList, sortField.ToString());
        }
        catch (Exception ex) {
            throw new Exception();
        }
    }

    public void Update(int id, BO.Tutor boTutor)
    {
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
