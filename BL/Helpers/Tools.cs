using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using DalApi;
using System.Reflection;
using System.Text;
using System.Collections;

namespace Helpers;


static internal class Tools
{
    private static IDal s_dal = Factory.Get; //stage 4


    internal static string ToStringProperty<T>(this T obj)
    {
        if (obj == null) return "null";

        Type type = obj.GetType();
        PropertyInfo[] properties = type.GetProperties();
        StringBuilder sb = new StringBuilder();

        foreach (PropertyInfo prop in properties)
        {
            object value = prop.GetValue(obj, null);

            if (value is IEnumerable enumerable && value is not string)
            {
                sb.AppendLine($"{prop.Name}: [{string.Join(", ", enumerable.Cast<object>())}]");
            }
            else
            {
                sb.AppendLine($"{prop.Name}: {value}");
            }
        }

        return sb.ToString().TrimEnd();
    }

    internal static List<T> SortByField<T>(List<T> list, string fieldName) where T : class
    {
        return list.OrderBy(item =>
            item.GetType().GetProperty(fieldName)?.GetValue(item)).ToList();
    }
    internal static bool IsManagerId(int tutorId)
    {
        return s_dal.Tutor.Read(tutorId).Role == DO.Role.Manager;
    }

    #region distance
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
    #endregion

    #region check address
    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("הכתובת אינה תקינה");
        }

        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(url);
                //Console.WriteLine("Response from server: " + response); // הדפסה לבדיקה

                var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);
                Console.WriteLine(result.ToString()); // הדפסה לבדיקה



                if (result == null || result.Length == 0)
                {
                    throw new Exception("לא נמצאו קואורדינטות לכתובת זו");
                }

                double latitude = double.Parse(result[0].Latitude);
                double longitude = double.Parse(result[0].Longitude);

                Console.WriteLine($"נבחרה הכתובת: {result[0].DisplayName}");
                return (latitude, longitude);
            }
        }
        catch (Exception ex)
        {
            throw new Exception("שגיאה בעת שליפת קואורדינטות: " + ex.Message);
        }
    }

 

    private class GeocodeResponse
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; } // מוגדר כמחרוזת

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } // מוגדר כמחרוזת

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; }
    }
    #endregion
}

