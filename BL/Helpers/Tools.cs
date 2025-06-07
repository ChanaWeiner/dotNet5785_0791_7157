using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using DalApi;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Helpers;

static internal class Tools
{
    private static IDal s_dal = Factory.Get;

    /// <summary>
    /// Converts an object's properties to a string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object.</typeparam>
    /// <param name="obj">The object to convert.</param>
    /// <returns>A string representing the object's properties and their values.</returns>
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

    /// <summary>
    /// Sorts a list of objects by a specified property field.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the list.</typeparam>
    /// <param name="list">The list to sort.</param>
    /// <param name="fieldName">The property field name to sort by.</param>
    /// <returns>A new sorted list of objects.</returns>
    internal static List<T> SortByField<T>(List<T> list, string fieldName) where T : class
    {
        return list.OrderBy(item =>
            item.GetType().GetProperty(fieldName)?.GetValue(item)).ToList();
    }

    /// <summary>
    /// Checks if a tutor has the role of manager.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor.</param>
    /// <returns>True if the tutor is a manager, otherwise false.</returns>
    internal static bool IsManagerId(int tutorId)
    {
        return s_dal.Tutor.Read(tutorId).Role == DO.Role.Manager;
    }

    #region distance
    /// <summary>
    /// Calculates the distance in kilometers between a tutor and a student call location.
    /// </summary>
    /// <param name="volunteerId">The ID of the tutor.</param>
    /// <param name="callLat">The latitude of the student call location.</param>
    /// <param name="callLong">The longitude of the student call location.</param>
    /// <returns>The distance in kilometers between the tutor and the student call location.</returns>
    internal static double CalculateDistance(int volunteerId, double callLat, double callLong)
    {
        var tutor = s_dal.Tutor.Read(volunteerId);
        if (tutor == null)
            throw new BO.BlDoesNotExistException($"Tutor with ID {volunteerId} not found");

        return GetDistance(tutor.Latitude, tutor.Longitude, callLat, callLong);
    }

    /// <summary>
    /// Calculates the distance between two geographical coordinates.
    /// </summary>
    /// <param name="lat1">Latitude of the first point.</param>
    /// <param name="lon1">Longitude of the first point.</param>
    /// <param name="lat2">Latitude of the second point.</param>
    /// <param name="lon2">Longitude of the second point.</param>
    /// <returns>The distance between the two points in kilometers.</returns>
    private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        // Haversine formula to calculate distance between two coordinates on Earth.
        double earthRadiusKm = 6371;
        double dLat = DegreesToRadians(lat2 - lat1);
        double dLon = DegreesToRadians(lon2 - lon1);
        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return earthRadiusKm * c;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">The angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    private static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
    #endregion

    #region check address
    /// <summary>
    /// Retrieves the geographical coordinates (latitude and longitude) for a given address.
    /// </summary>
    /// <param name="address">The address to get the coordinates for.</param>
    /// <returns>A tuple containing the latitude and longitude of the address.</returns>
    /// <exception cref="BO.BlValidationException">Thrown when the address is invalid.</exception>
    public static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new BO.BlValidationException("The address is invalid.");
        }

        string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&api_key=679a8da6c01a6853187846vomb04142";

        try
        {
            using (WebClient client = new WebClient())
            {
                string response = client.DownloadString(url);

                var result = JsonSerializer.Deserialize<GeocodeResponse[]>(response);

                if (result == null || result.Length == 0)
                {
                    throw new BO.BlValidationException("The address is invalid.");
                }

                double latitude = double.Parse(result[0].Latitude);
                double longitude = double.Parse(result[0].Longitude);

                return (latitude, longitude);
            }
        }
        catch (Exception ex)
        {
            throw new BO.BlValidationException("Error retrieving coordinates" + ex.Message);
        }
    }

    /// <summary>
    /// Represents the structure of a geocoding response.
    /// </summary>
    private class GeocodeResponse
    {
        [JsonPropertyName("lat")]
        public string Latitude { get; set; } // Latitude as string

        [JsonPropertyName("lon")]
        public string Longitude { get; set; } // Longitude as string

        [JsonPropertyName("display_name")]
        public string DisplayName { get; set; } // Full address representation
    }
    #endregion

    #region validation

    /// <summary>
    /// Validates if an ID is valid (9 digits, checksum).
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>True if the ID is valid, otherwise false.</returns>
    internal static bool IsValidId(int id)
    {
        if (id <= 0)
            return false;

        string idString = id.ToString();
        if (idString.Length != 9)
            return false;

        // Calculate checksum digit
        int sum = 0;
        for (int i = 0; i < idString.Length; i++)
        {
            int digit = int.Parse(idString[i].ToString());
            digit *= (i % 2) + 1;
            if (digit > 9) digit -= 9;
            sum += digit;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// Validates if a phone number is in the correct format.
    /// </summary>
    /// <param name="phoneNumber">The phone number to validate.</param>
    /// <returns>True if the phone number is valid, otherwise false.</returns>
    internal static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        string phonePattern = @"^(\+972|0)([23489]|5[0-9])-?\d{7}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    /// <summary>
    /// Validates if an email address is in the correct format.
    /// </summary>
    /// <param name="email">The email address to validate.</param>
    /// <returns>True if the email is valid, otherwise false.</returns>
    internal static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
    #endregion
}
