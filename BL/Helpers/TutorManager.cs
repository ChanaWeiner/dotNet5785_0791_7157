

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

    internal static List<BO.TutorInList> GetTutorsInList()
    {
        List <BO.TutorInList> list = null;
        return list;
    }

    internal static double GetDistance()
    {
        return 0.0;
    }

    internal static BO.CallStatus GetCallStatus(DO.StudentCall studentCall)
    {
        return BO.CallStatus.InProgress;
    }
}
