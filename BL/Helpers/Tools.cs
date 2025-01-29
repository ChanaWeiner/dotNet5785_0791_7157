using DalApi;

namespace Helpers;


static internal class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static List<T> SortByField<T>(List<T> list, string fieldName) where T : class
    {
        return list.OrderBy(item =>
            item.GetType().GetProperty(fieldName)?.GetValue(item)).ToList();
    }

    internal static bool IsValidAddress(string address, out double latitude, out double longitude)
    {
        // כאן יש לממש בדיקה מול שירות חיצוני או מסד נתונים, כרגע החזרת ערכים דיפולטיביים להמחשה
        latitude = 0;
        longitude = 0;
        return !string.IsNullOrWhiteSpace(address);
    }

    internal static double CalculateDistance(int volunteerId, double callLat, double callLong)
    {
        var tutor = s_dal.Tutor.Read(volunteerId);
        if (tutor == null)
            throw new Exception($"Volunteer with ID {volunteerId} not found");

        return GetDistance(tutor.Latitude, tutor.Longitude, callLat, callLong);
    }

    private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // חישוב מרחק בקילומטרים בין שתי נקודות קואורדינטות גיאוגרפיות
        double earthRadiusKm = 6371;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
}
