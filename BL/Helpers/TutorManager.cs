

using BlImplementation;
using BO;
using DalApi;
using DO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using BCrypt.Net;


namespace Helpers;

internal class TutorManager
{
    private static IDal s_dal = Factory.Get;

    internal static BO.TutorInList ConvertFromDoToBo(DO.Tutor tutor)
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

    internal static int CountCallsByEndStatus(int tutorId, BO.EndOfTreatment endStatus)
    {
        return s_dal.Assignment.ReadAll(a =>
            a.TutorId == tutorId && a.EndOfTreatment!=null && (BO.EndOfTreatment)a.EndOfTreatment == endStatus
        ).Select(a=>a.StudentCallId)
        .Distinct()
        .Count();
    }

    #region validation
    internal static void Validation(ref BO.Tutor boTutor)
    {
        // ID validation
        if (!IsValidId(boTutor.Id))
            throw new BO.BlValidationException($"ID {boTutor.Id} is invalid.");

        // Full name validation
        if (string.IsNullOrWhiteSpace(boTutor.FullName) || boTutor.FullName.Length < 2 || boTutor.FullName.Length > 100)
            throw new BO.BlValidationException($"Full name '{boTutor.FullName}' must be between 2 and 100 characters.");

        // Phone number validation
        if (!IsValidPhoneNumber(boTutor.CellNumber))
            throw new FormatException($"Phone number '{boTutor.CellNumber}' is invalid.");

        // Email validation
        if (!IsValidEmail(boTutor.Email))
            throw new FormatException($"Email address '{boTutor.Email}' is invalid.");

        // Password validation (if provided)

        if (!IsValidPassword(boTutor.Password))
            throw new BO.BlValidationException("Password is not strong enough");
        try
        {
            (boTutor.Latitude, boTutor.Longitude) = Tools.GetCoordinates(boTutor.CurrentAddress!);
        }
        catch (Exception ex) 
        {
            throw ex;
        }

        // Role (Enum) validation
        if (!Enum.IsDefined(typeof(BO.Role), boTutor.Role))
            throw new BO.BlValidationException($"Role - This option: '{boTutor.Role}' is not defind");

        // Distance validation
        if (boTutor.Distance < 0)
            throw new BO.BlValidationException($"Distance {boTutor.Distance} must be a non-negative value.");

        // DistanceType (Enum) validation
        if (!Enum.IsDefined(typeof(BO.DistanceType), boTutor.DistanceType))
            throw new BO.BlValidationException($"DistanceType - This option: '{boTutor.DistanceType}' is not defind");
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            return false;

        bool hasUpper = password.Any(char.IsUpper);
        bool hasLower = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

        return hasUpper && hasLower && hasDigit && hasSpecial;
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


    #endregion
    
    public static string HashPassword(string password)=> BCrypt.Net.BCrypt.HashPassword(password);

    public static bool VerifyPassword(string password, string hashedPassword)=> BCrypt.Net.BCrypt.Verify(password, hashedPassword);

}
