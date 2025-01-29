namespace Helpers;


static internal class Tools
{
    internal static List<T> SortByField<T>(List<T> list, string fieldName) where T : class
    {
        return list.OrderBy(item =>
            item.GetType().GetProperty(fieldName)?.GetValue(item)).ToList();
    }
}
