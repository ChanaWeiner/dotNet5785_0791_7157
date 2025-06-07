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
    internal static ObserverManager Observers = new(); //stage 5
    /// <summary>
    /// Converts a DO.Tutor object to a BO.TutorInList object.
    /// </summary>
    /// <param name="tutor">The DO.Tutor object to convert.</param>
    /// <returns>A BO.TutorInList object containing relevant tutor information.</returns>
    internal static BO.TutorInList ConvertFromDoToBo(DO.Tutor tutor)
    {
        var tutorAssignments = s_dal.Assignment.ReadAll(a => a.TutorId == tutor.Id);
        var currenCallId = tutorAssignments.Where(a => a.EndTime == null)
                .Select(a => a.StudentCallId)
                .FirstOrDefault();
        var currentCall = s_dal.StudentCall.Read(currenCallId);
        return new BO.TutorInList
        {
            Id = tutor.Id,
            FullName = tutor.FullName,
            IsActive = tutor.Active,

            // Counting total handled calls (where EndOfTreatment is "Handled")
            TotalHandledCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.Treated),

            // Counting total canceled calls (where EndOfTreatment is "Canceled")
            TotalCancelledCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.ManagerCancel ||
                                                              a.EndOfTreatment == DO.EndOfTreatment.SelfCancel),

            // Counting total expired calls (where EndOfTreatment is "Expired")
            TotalExpiredCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.Expired),

            // Getting the current call ID and type
            CurrentCallId = currenCallId,

            CurrentSubject = currentCall != null ? (BO.Subjects)currentCall.Subject : null
        };
    }

    /// <summary>
    /// Counts the number of calls with a specific EndOfTreatment status for a tutor.
    /// </summary>
    /// <param name="tutorId">The tutor's ID.</param>
    /// <param name="endStatus">The EndOfTreatment status to filter by.</param>
    /// <returns>The count of calls with the specified EndOfTreatment status.</returns>
    internal static int CountCallsByEndStatus(int tutorId, BO.EndOfTreatment endStatus)
    {
        return s_dal.Assignment.ReadAll(a =>
            a.TutorId == tutorId && a.EndOfTreatment != null && (BO.EndOfTreatment)a.EndOfTreatment == endStatus
        ).Select(a => a.StudentCallId)
        .Distinct()
        .Count();
    }

    #region validation
    /// <summary>
    /// Validates the properties of a BO.Tutor object.
    /// </summary>
    /// <param name="boTutor">The BO.Tutor object to validate.</param>
    /// <exception cref="BO.BlValidationException">Thrown when any validation fails.</exception>
    internal static void Validation(ref BO.Tutor boTutor)
    {
        // Validate ID presence
        if (boTutor.Id <= 0)
            throw new BO.BlValidationException("ID is required.");
        if (!Tools.IsValidId(boTutor.Id))
            throw new BO.BlValidationException($"ID {boTutor.Id} is invalid.");

        // Validate full name presence
        if (string.IsNullOrWhiteSpace(boTutor.FullName))
            throw new BO.BlValidationException("Full name is required.");
        if (boTutor.FullName.Length < 2 || boTutor.FullName.Length > 100)
            throw new BO.BlValidationException($"Full name '{boTutor.FullName}' must be between 2 and 100 characters.");

        // Validate phone number presence
        if (string.IsNullOrWhiteSpace(boTutor.CellNumber))
            throw new BO.BlValidationException("Phone number is required.");
        if (!Tools.IsValidPhoneNumber(boTutor.CellNumber))
            throw new BO.BlValidationException($"Phone number '{boTutor.CellNumber}' is invalid.");

        // Validate email presence
        if (string.IsNullOrWhiteSpace(boTutor.Email))
            throw new BO.BlValidationException("Email address is required.");
        if (!Tools.IsValidEmail(boTutor.Email))
            throw new BO.BlValidationException($"Email address '{boTutor.Email}' is invalid.");

        // Validate password presence
        if (string.IsNullOrWhiteSpace(boTutor.Password))
            throw new BO.BlValidationException("Password is required.");
        if (!IsValidPassword(boTutor.Password))
            throw new BO.BlValidationException("Password is not strong enough.");

        // Validate address presence and get coordinates
        if (string.IsNullOrWhiteSpace(boTutor.CurrentAddress))
            throw new BO.BlValidationException("Current address is required.");
        try
        {
            (boTutor.Latitude, boTutor.Longitude) = Tools.GetCoordinates(boTutor.CurrentAddress);
        }
        catch (Exception ex)
        {
            throw new BO.BlValidationException($"Failed to get coordinates for address: {ex.Message}");
        }

        // Validate role (Enum)
        if (!Enum.IsDefined(typeof(BO.Role), boTutor.Role))
            throw new BO.BlValidationException($"Role - This option: '{boTutor.Role}' is not defined");

        // Validate distance (must be non-negative)
        if (boTutor.Distance < 0)
            throw new BO.BlValidationException($"Distance {boTutor.Distance} must be a non-negative value.");

        // Validate distance type (Enum)
        if (!Enum.IsDefined(typeof(BO.DistanceType), boTutor.DistanceType))
            throw new BO.BlValidationException($"DistanceType - This option: '{boTutor.DistanceType}' is not defined");
    }


    /// <summary>
    /// Validates if a password meets strength requirements (upper, lower, digit, special).
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if the password is strong, otherwise false.</returns>
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

    #endregion

    /// <summary>
    /// Hashes a password using BCrypt.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password.</returns>
    public static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    /// <summary>
    /// Verifies if a password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain password.</param>
    /// <param name="hashedPassword">The hashed password.</param>
    /// <returns>True if the password matches the hashed password, otherwise false.</returns>
    public static bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);
}
