

using DalApi;

namespace Helpers;

internal class TutorManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static List<T> SortByField<T>(List<T> list, string fieldName) where T : class
    {
        return list.OrderBy(item =>
            item.GetType().GetProperty(fieldName)?.GetValue(item)).ToList();
    }

    internal static BO.Tutor ConvertToBO(DO.Tutor doTutor)
    {
        return new BO.Tutor
        {
            Id = doTutor.Id,
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
            DistanceType = (BO.DistanceType)doTutor.DistanceType
        };
    }

    internal static DO.Tutor ConvertToDO(BO.Tutor boTutor)
    {
        return new DO.Tutor
        {
            Id = boTutor.Id,
            FullName = boTutor.FullName,
            CellNumber = boTutor.CellNumber,
            Email = boTutor.Email,
            Password = boTutor.Password,
            CurrentAddress = boTutor.CurrentAddress,
            Latitude = boTutor.Latitude,
            Longitude = boTutor.Longitude,
            Role = (DO.Role)boTutor.Role,
            Active = boTutor.Active,
            Distance = boTutor.Distance,
            DistanceType = (DO.DistanceType)boTutor.DistanceType
        };
    }

}
