

using BlImplementation;
using BO;
using DalApi;
using DO;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Helpers;

internal class TutorManager
{
    private static IDal s_dal = Factory.Get; //stage 4
   

    internal static List<BO.TutorInList> GetTutorsInList(bool?isActive)
    {
        List<DO.Tutor> doTutor = s_dal.Tutor.ReadAll((DO.Tutor tutor)=> (isActive==null || tutor.Active==isActive)).ToList()
            ?? throw new Exception($"Student with ID= does Not exist");
        List <BO.TutorInList> tutorInList = doTutor.Select(ConvertFromDoToBo).ToList();
        return tutorInList;
    }

    private static BO.TutorInList ConvertFromDoToBo(DO.Tutor tutor)
    {
        var assignments = s_dal.Assignment.ReadAll();
        var currenCallId = s_dal.Assignment.ReadAll(a => a.TutorId == tutor.Id && a.EndTime == null)
                .Select(a => a.StudentCallId)
                .FirstOrDefault();
        return new BO.TutorInList
        {
            Id = tutor.Id,
            FullName = tutor.FullName,
            IsActive = tutor.Active,

            // Calculating total handled calls (where EndOfTreatment is "Handled")
            TotalHandledCalls = assignments.Count(a => a.TutorId == tutor.Id && a.EndOfTreatment == DO.EndOfTreatment.Treated),

            // Calculating total canceled calls (where EndOfTreatment is "Canceled")
            TotalCancelledCalls = assignments.Count(a => a.TutorId == tutor.Id &&( a.EndOfTreatment == DO.EndOfTreatment.ManagerCancel||
            a.EndOfTreatment == DO.EndOfTreatment.SelfCancel)),

            // Calculating total expired calls (where EndOfTreatment is "Expired")
            TotalExpiredCalls = assignments.Count(a => a.TutorId == tutor.Id && a.EndOfTreatment == DO.EndOfTreatment.Expired),

            // Current call ID and type
            CurrentCallId = currenCallId,

            CurrentSubject = (BO.Subjects)s_dal.StudentCall.Read((DO.StudentCall studentCall)=> studentCall.Id== currenCallId)!.Subject
        };
    }


    internal static double GetDistance(DO.Tutor tutor,DO.StudentCall studentCall)
    {
        return Math.Sqrt(Math.Pow(tutor.Latitude-studentCall.Latitude,2)+ Math.Pow(tutor.Longitude - studentCall.Longitude, 2));
    }

    internal static void Validation(ref BO.Tutor boTutor)
    {
        // ID validation
        if (!IsValidId(boTutor.Id))
            throw new ArgumentOutOfRangeException(nameof(boTutor.Id), $"ID {boTutor.Id} is invalid.");

        // Full name validation
        if (string.IsNullOrWhiteSpace(boTutor.FullName) || boTutor.FullName.Length < 2 || boTutor.FullName.Length > 100)
            throw new ArgumentException($"Full name '{boTutor.FullName}' must be between 2 and 100 characters.", nameof(boTutor.FullName));

        // Phone number validation
        if (!IsValidPhoneNumber(boTutor.CellNumber))
            throw new FormatException($"Phone number '{boTutor.CellNumber}' is invalid.");

        // Email validation
        if (!IsValidEmail(boTutor.Email))
            throw new FormatException($"Email address '{boTutor.Email}' is invalid.");

        // Password validation (if provided)
        if (!string.IsNullOrEmpty(boTutor.Password) && boTutor.Password.Length < 8)
            throw new ArgumentException($"Password must be at least 8 characters long.", nameof(boTutor.Password));

        // Coordinates validation
        if (!Tools.IsValidAddress(boTutor.CurrentAddress!, out double latitude, out double longitude))
            throw new ArgumentException($"Coordinates are invalid: Latitude={boTutor.Latitude}, Longitude={boTutor.Longitude}.");
        boTutor.Latitude = latitude;
        boTutor.Longitude = longitude;
        // Address validation (if provided)
        if (!string.IsNullOrEmpty(boTutor.CurrentAddress) && boTutor.CurrentAddress.Length < 5)
            throw new ArgumentException($"Address '{boTutor.CurrentAddress}' must be at least 5 characters long.", nameof(boTutor.CurrentAddress));

        // Role (Enum) validation
        if (!Enum.IsDefined(typeof(BO.Role), boTutor.Role))
            throw new InvalidEnumArgumentException(nameof(boTutor.Role), (int)boTutor.Role, typeof(BO.Role));

        // Distance validation
        if (boTutor.Distance < 0)
            throw new ArgumentOutOfRangeException(nameof(boTutor.Distance), $"Distance {boTutor.Distance} must be a non-negative value.");

        // DistanceType (Enum) validation
        if (!Enum.IsDefined(typeof(BO.DistanceType), boTutor.DistanceType))
            throw new InvalidEnumArgumentException(nameof(boTutor.DistanceType), (int)boTutor.DistanceType, typeof(BO.DistanceType));
    }

    private static bool IsValidId(int id)
    {
        if (id <= 0)
            return false;

        string idString = id.ToString();
        if (idString.Length != 9)
            return false;

        // חישוב ספרת ביקורת
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

    private static bool IsValidPhoneNumber(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        // בדיקת פורמט של מספר טלפון
        string phonePattern = @"^(\+972|0)([23489]|5[0-9])\d{7}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        // בדיקת פורמט של אימייל
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }

}
