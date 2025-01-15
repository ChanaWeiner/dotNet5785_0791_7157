
using BlApi;
using DalApi;
using Helpers;
using System;

namespace BlImplementation;

internal class TutorImplementation : ITutor
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Tutor tutor)
    {
        DO.Tutor doTutor = new(tutor.Id, tutor.FullName, tutor.CellNumber, tutor.Email, tutor.Password, tutor.CurrentAddress, tutor.Latitude, tutor.Longitude, tutor.role, tutor.Active, tutor.Distance, tutor.DistanceType);

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
            Role = doTutor.role,
            Active = doTutor.Active,
            Distance = doTutor.Distance,
            DistanceType = doTutor.DistanceType
        };

    }

    public IEnumerable<BO.TutorInList> SortTutorsInList(bool isActive, BO.TutorSortField sortField)
    {
        IEnumerable<BO.TutorInList> tutorInLists = _dal.;

    }

    public void Update(int id, BO.Tutor tutor)
    {
        throw new NotImplementedException();
    }
}
