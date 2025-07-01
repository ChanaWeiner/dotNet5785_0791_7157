using BlImplementation;
using BO;
using DalApi;
using DO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using BCrypt.Net;
using System.Collections.Generic;
using BlApi;

namespace Helpers;

internal class TutorManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    internal static ObserverManager Observers = new(); //stage 5
    /// <summary>
    /// Converts a DO.Tutor object to a BO.TutorInList object.
    /// </summary>
    /// <param name="tutor">The DO.Tutor object to convert.</param>
    /// <returns>A BO.TutorInList object containing relevant tutor information.</returns>
    internal static BO.TutorInList ConvertFromDoToBo(DO.Tutor tutor)
    {
        // שימוש ב-TutorManager לגישה ל-Assignment
        IEnumerable<DO.Assignment> tutorAssignments = Tools.ReadAllAssignments(a => a.TutorId == tutor.Id);

        var currenCallId = tutorAssignments
            .Where(a => a.EndTime == null)
            .Select(a => a.StudentCallId)
            .FirstOrDefault();

        var currentCall = StudentCallManager.Read(currenCallId);

        return new BO.TutorInList
        {
            Id = tutor.Id,
            FullName = tutor.FullName,
            IsActive = tutor.Active,

            TotalHandledCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.Treated),
            TotalCancelledCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.ManagerCancel ||
                                                               a.EndOfTreatment == DO.EndOfTreatment.SelfCancel),
            TotalExpiredCalls = tutorAssignments.Count(a => a.EndOfTreatment == DO.EndOfTreatment.Expired),

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
        return Tools.ReadAllAssignments(a =>
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
        // Validate role presence
        if (boTutor.Role == BO.Role.None)
            throw new BO.BlValidationException("Role cannot be None. Please select a valid role.");

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
    internal static string HashPassword(string password) => BCrypt.Net.BCrypt.HashPassword(password);

    /// <summary>
    /// Verifies if a password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain password.</param>
    /// <param name="hashedPassword">The hashed password.</param>
    /// <returns>True if the password matches the hashed password, otherwise false.</returns>
    internal static bool VerifyPassword(string password, string hashedPassword) => BCrypt.Net.BCrypt.Verify(password, hashedPassword);

    internal static void Create(DO.Tutor item)
    {
        lock (AdminManager.BlMutex)
            s_dal.Tutor.Create(item);
    }
    internal static DO.Tutor? Read(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Tutor.Read(id);
    }
    internal static IEnumerable<DO.Tutor> ReadAll(Func<DO.Tutor, bool>? filter = null)
    {
        lock (AdminManager.BlMutex)
            return s_dal.Tutor.ReadAll(filter).ToList();
    }
    internal static void Update(DO.Tutor item)
    {
        lock (AdminManager.BlMutex)
            s_dal.Tutor.Update(item);
    }
    internal static void Delete(int id)
    {
        lock (AdminManager.BlMutex)
            s_dal.Tutor.Delete(id);
    }



    internal static void TutorSimulator()
    {
        BlApi.IStudentCall StudentCallImpl = new StudentCallImplementation();
        BlApi.ITutor TutorImpl = new TutorImplementation();
        Random rnd = new Random();
        int toCancel = rnd.Next(1, 6);
        int toAssign = rnd.Next(1, 6);
        var activeDOTutors = ReadAll(tutor => tutor.Active);
        IEnumerable<BO.Tutor> activeTutors = activeDOTutors.Select(t => TutorImpl.Read(t.Id));
        foreach (var tutor in activeTutors)
        {
            BO.CallInProgress currentCallInProgress = tutor.CurrentCallInProgress;
            if (currentCallInProgress == null)
            {
                toAssign = rnd.Next(1, 11);
                if (toAssign == 1)
                {
                    var mostCommonSubject = StudentCallImpl.GetClosedCallsForTutor(tutor.Id)
                     .GroupBy(c => c.Subject)
                     .OrderByDescending(g => g.Count())
                     .Select(g => (BO.Subjects?)g.Key)  
                     .FirstOrDefault();

                    IEnumerable<BO.OpenCallInList> openCalls;
                    if (mostCommonSubject != null)
                        openCalls = StudentCallImpl.FilterOpenCalls(tutor.Id, BO.OpenCallField.Subject, mostCommonSubject);
                    else
                        openCalls = StudentCallImpl.FilterOpenCalls(tutor.Id);

                    openCalls.OrderBy(c => c.DistanceFromTutor);
                    if(openCalls.Any())
                        StudentCallManager.AssignCallToTutor(tutor.Id, openCalls.FirstOrDefault().Id);

                }
            }
            else
            {
                DateTime entryTime = currentCallInProgress.EntryTime;
                int addTime = 7 + (int)Math.Floor(currentCallInProgress.Distance) / 2;
                if (entryTime.AddDays(addTime) <= AdminManager.Now)
                {
                    StudentCallManager.UpdateTreatmentCompletion(tutor.Id,currentCallInProgress.Id);
                }
                else
                {
                    toCancel = rnd.Next(1, 11);
                    if (toCancel == 1)
                        StudentCallManager.UpdateTreatmentCancellation(currentCallInProgress.Id, tutor.Id);
                }
                

            }

        }
    }

}
