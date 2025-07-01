using BlApi;
using BlImplementation;
using BO;
using DalApi;

namespace Helpers;

internal class StudentCallManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    private static IAdmin manager = new AdminImplementation();
    internal static ObserverManager Observers = new(); //stage 5


    internal static BO.CallInList ConvertFromDoToBo(DO.StudentCall studentCall)
    {
        //var maxCompletionTime = AdminManager.Now - manager.GetRiskTimeRange();

        var assignments = Tools.ReadAllAssignments(a => a.StudentCallId == studentCall.Id)
                                      .OrderBy(a => a.EntryTime)
                                      .ToList();

        var lastAssignment = assignments.LastOrDefault();

        var status = CalculateCallStatus(studentCall);

        var totalAssignments = assignments.Count;

        return new BO.CallInList
        {
            Id = lastAssignment?.Id,
            CallId = studentCall.Id,
            Subject = (BO.Subjects)studentCall.Subject,
            OpeningTime = studentCall.OpenTime,
            RemainingTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - AdminManager.Now
                : null,
            LastTutorName = lastAssignment != null
                ? TutorManager.Read(lastAssignment.TutorId)?.FullName
                : null,
            CompletionTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - studentCall.OpenTime
                : (TimeSpan?)null,
            Status = status,
            TotalAssignments = totalAssignments
        };
    }

    /// <summary>
    /// Converts a DO.StudentCall object to a BO.CallInList object.
    /// </summary>
    /// <param name="studentCall">The DO.StudentCall object to convert.</param>
    /// <returns>A BO.CallInList object representing the student call.</returns>
    internal static BO.CallStatus CalculateCallStatus(DO.StudentCall studentCall)
    {
        var lastAssignment = Tools.ReadAllAssignments(a => a.StudentCallId == studentCall.Id)
                                         .OrderBy(a => a.EntryTime)
                                         .LastOrDefault();

        bool isCallExpired = studentCall.FinalTime.HasValue && studentCall.FinalTime < AdminManager.Now;
        bool isCallInRisk = studentCall.FinalTime.HasValue && studentCall.FinalTime - AdminManager.Now < AdminManager.RiskTimeSpan;

        if (isCallExpired)
            return BO.CallStatus.Expired;

        if (lastAssignment == null)
            return isCallInRisk ? BO.CallStatus.OpenInRisk : BO.CallStatus.Open;

        if (isCallInRisk)
            return BO.CallStatus.InProgressAtRisk;

        return lastAssignment.EndOfTreatment switch
        {
            DO.EndOfTreatment.Treated => BO.CallStatus.Closed,
            DO.EndOfTreatment.SelfCancel => BO.CallStatus.Open,
            DO.EndOfTreatment.ManagerCancel => BO.CallStatus.Open,
            DO.EndOfTreatment.Expired => BO.CallStatus.Expired,
            _ or null => BO.CallStatus.InProgress
        };
    }



    /// <summary>
    /// Validates a student call object before creating or updating it.
    /// </summary>
    /// <param name="call">The student call object to validate.</param>
    internal static void Validation(ref BO.StudentCall call)
    {

        // Validate subject (Enum)
        if (!Enum.IsDefined(typeof(BO.Subjects), call.Subject))
            throw new BO.BlValidationException($"Subject '{call.Subject}' is not valid. Please select a valid subject.");
        // Ensure subject is not 'None'
        if (call.Subject == BO.Subjects.None)
            throw new BO.BlValidationException("Subject cannot be 'None'. Please select a valid subject.");
        // Validate full name
        if (string.IsNullOrWhiteSpace(call.FullName))
                throw new BO.BlValidationException("Full name is required.");
        if (call.FullName.Length < 2 || call.FullName.Length > 100)
            throw new BO.BlValidationException($"Full name '{call.FullName}' must be between 2 and 100 characters.");

        // Validate phone number
        if (string.IsNullOrWhiteSpace(call.CellNumber))
            throw new BO.BlValidationException("Phone number is required.");
        if (!Tools.IsValidPhoneNumber(call.CellNumber))
            throw new BO.BlValidationException($"Phone number '{call.CellNumber}' is invalid.");

        // Validate email
        if (string.IsNullOrWhiteSpace(call.Email))
            throw new BO.BlValidationException("Email address is required.");
        if (!Tools.IsValidEmail(call.Email))
            throw new BO.BlValidationException($"Email address '{call.Email}' is invalid.");

        // Validate address and get coordinates
        if (string.IsNullOrWhiteSpace(call.FullAddress))
            throw new BO.BlValidationException("Address is required.");
        UpdateCoordinatesAsync(call);

        // Validate open/final time
        if (call.OpenTime >= call.FinalTime)
            throw new BO.BlValidationException("Start time must be earlier than end time.");

        // Optional: validate description length if needed
        if (call.Description != null && call.Description.Length > 500)
            throw new BO.BlValidationException("Description is too long. Maximum 500 characters allowed.");
    }

    internal async static void UpdateCoordinatesAsync(BO.StudentCall call)
    {
        try
        {
            (call.Latitude, call.Longitude) = await Tools.GetCoordinatesAsync(call.FullAddress);
            s_dal.StudentCall.Update(new DO.StudentCall
            {
                Id = call.Id,
                Subject = (DO.Subjects)call.Subject,
                FullName = call.FullName,
                CellNumber = call.CellNumber,
                Email = call.Email,
                FullAddress = call.FullAddress,
                Latitude = call.Latitude,
                Longitude = call.Longitude,
                OpenTime = call.OpenTime,
                FinalTime = call.FinalTime,
                Description = call.Description
            });
        }
        catch(DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Failed to update coordinates for the address '{call.FullAddress}': {ex.Message}");
        }
        catch (Exception ex)
        {
            throw new BO.BlValidationException($"Failed to get coordinates for the address: {ex.Message}");
        }
    }


    /// <summary>
    /// Updates the status of calls that have passed their final time.
    /// </summary>
    internal static void PeriodicStudentcallStatusUpdates(DateTime oldClock, DateTime newClock)
    {
        var calls = StudentCallManager.ReadAll(c => c.FinalTime.HasValue && c.FinalTime <= newClock);

        foreach (var call in calls)
        {
            var assignments = Tools.ReadAllAssignments(a => a.StudentCallId == call.Id).ToList();

            if (assignments == null || !assignments.Any())
            {
                var newAssignment = new DO.Assignment
                {
                    StudentCallId = call.Id,
                    TutorId = 0,
                    EntryTime = newClock,
                    EndOfTreatment = DO.EndOfTreatment.Expired,
                    EndTime = newClock
                };

                Tools.CreateAssignment(newAssignment);
                Observers.NotifyItemUpdated(newAssignment.StudentCallId);
                Observers.NotifyListUpdated();
                TutorManager.Observers.NotifyListUpdated();
                
            }
            else
            {
                foreach (var assignment in assignments.Where(a => a.EndTime == null))
                {
                    var updated = assignment with
                    {
                        EndTime = newClock,
                        EndOfTreatment = DO.EndOfTreatment.Expired
                    };

                    Tools.UpdateAssignment(updated);
                    Observers.NotifyItemUpdated(updated.StudentCallId);
                    TutorManager.Observers.NotifyItemUpdated(updated.TutorId);
                    TutorManager.Observers.NotifyListUpdated();
                    Observers.NotifyListUpdated();
                }
            }
        }

        Observers.NotifyListUpdated();
    }


    internal static void UpdateTreatmentCancellation(int assignmentId, int tutorId)
    {
        var assignment = Tools.ReadAssignment(assignmentId) ??
    throw new BO.BlDoesNotExistException($"Assignment's tutor with ID={tutorId}, which its ID={assignmentId} does not exist");

        if (assignment.TutorId != tutorId && !Tools.IsManagerId(tutorId))
            throw new BlCanNotUpdateTreatment("Tutor cannot cancel treatment for this assignment.");

        if (assignment.EndTime != null)
            throw new BlCanNotUpdateTreatment("Assignment treatment has already been completed, canceled or expired.");

        var updateAssignment = assignment with
        {
            EndOfTreatment = assignment.TutorId == tutorId ? DO.EndOfTreatment.SelfCancel : DO.EndOfTreatment.ManagerCancel,
            EndTime = AdminManager.Now
        };

        Tools.UpdateAssignment(updateAssignment);
        TutorManager.Observers.NotifyItemUpdated(tutorId);
        StudentCallManager.Observers.NotifyListUpdated();
        TutorManager.Observers.NotifyListUpdated();
        StudentCallManager.Observers.NotifyItemUpdated(assignment.StudentCallId);
    }

    internal static void UpdateTreatmentCompletion(int tutorId, int assignmentId)
    {
        var assignment = Tools.ReadAssignment(a => a.TutorId == tutorId && a.Id == assignmentId) ??
    throw new BO.BlDoesNotExistException($"Assignment's tutor with ID={tutorId}, which its ID={assignmentId} does not exist");

        if (assignment.EndTime != null)
            throw new BlCanNotUpdateTreatment("Assignment treatment has already been completed or canceled.");

        var updateAssignment = assignment with
        {
            EndOfTreatment = DO.EndOfTreatment.Treated,
            EndTime = AdminManager.Now
        };

        Tools.UpdateAssignment(updateAssignment);
        TutorManager.Observers.NotifyItemUpdated(tutorId);
        StudentCallManager.Observers.NotifyListUpdated();
        TutorManager.Observers.NotifyListUpdated();
        StudentCallManager.Observers.NotifyItemUpdated(assignment.StudentCallId);
    }


    internal static void AssignCallToTutor(int tutorId, int callId)
    {
        var existingAssignments = Tools.ReadAllAssignments(a => a.TutorId == tutorId && a.EndTime == null);
        if (existingAssignments.Any())
            throw new BO.BlCanNotAssignCall($"Tutor with ID={tutorId} already has an open call in treatment.");

        var callAssignments = Tools.ReadAllAssignments(a =>
            a.StudentCallId == callId &&
            (a.EndOfTreatment == DO.EndOfTreatment.Treated || a.EndOfTreatment == DO.EndOfTreatment.Expired));

        if (callAssignments.Any())
            throw new BO.BlCanNotAssignCall($"Call with ID={callId} has already been handled or has expired.");

        var newAssignment = new DO.Assignment(0, callId, tutorId, AdminManager.Now, null, null);
        Tools.CreateAssignment(newAssignment);

        StudentCallManager.Observers.NotifyListUpdated();
        TutorManager.Observers.NotifyItemUpdated(tutorId);
        TutorManager.Observers.NotifyListUpdated();
        StudentCallManager.Observers.NotifyItemUpdated(callId);
    }

    internal static void Create(DO.StudentCall item)
    {
        lock (AdminManager.BlMutex)
            s_dal.StudentCall.Create(item);
    }
    internal static DO.StudentCall? Read(int id)
    {
        lock (AdminManager.BlMutex)
            return s_dal.StudentCall.Read(id);
    }
    internal static IEnumerable<DO.StudentCall> ReadAll(Func<DO.StudentCall, bool>? filter = null)
    {
        lock (AdminManager.BlMutex)
            return s_dal.StudentCall.ReadAll(filter).ToList();
    }
    internal static void Update(DO.StudentCall item)
    {
        lock (AdminManager.BlMutex)
            s_dal.StudentCall.Update(item);
    }
    internal static void Delete(int id)
    {
        lock (AdminManager.BlMutex)
            s_dal.StudentCall.Delete(id);
    }

}
