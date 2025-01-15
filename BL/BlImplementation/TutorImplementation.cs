
using BO;
using Helpers;
using System;

namespace BlImplementation;

internal class TutorImplementation : BlApi.ITutor
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void Create(BO.Tutor tutor)
    {
        DO.Tutor doTutor = new(tutor.Id, tutor.FullName, tutor.CellNumber, tutor.Email, tutor.Password, 
            tutor.CurrentAddress, tutor.Latitude, tutor.Longitude, (DO.Role)tutor.Role,
            tutor.Active, tutor.Distance, (DO.DistanceType)tutor.DistanceType);

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
        return TutorManager.ConvertToBO(doTutor);

    }

    public IEnumerable<BO.TutorInList> SortTutorsInList(bool isActive, BO.TutorSortField sortField)
    {
        IEnumerable<BO.TutorInList> tutorInLists = null;
        try
        {
            List<DO.Tutor> doTutors = (List<DO.Tutor>)_dal.Tutor.ReadAll((DO.Tutor tutor) => tutor.Active == isActive);
            TutorManager.SortByField<DO.Tutor>(doTutors, sortField.ToString());
            return (List<BO.TutorInList>)doTutors.Select(doTutor => TutorManager.ConvertToBO(doTutor));
        }
        catch (Exception ex) {
            throw new Exception();
        }
    }

    public void Update(int id, BO.Tutor boTutor)
    {
        try
        {
            _dal.Tutor.Update(id,TutorManager.ConvertToDO(boTutor));
        }
        catch (Exception ex)
        {
            throw new BO.BlAlreadyExistsException($"Student with ID={boTutor.Id} already exists", ex);
        }
    }
}
