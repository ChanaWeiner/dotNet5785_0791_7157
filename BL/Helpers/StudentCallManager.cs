using BlApi;
using BlImplementation;
using DalApi;

namespace Helpers;

internal class StudentCallManager
{
    private static IDal s_dal = DalApi.Factory.Get;
    private static IAdmin manager = new AdminImplementation();
    internal static ObserverManager Observers = new(); //stage 5
   
    /// <summary>
    /// Converts a DO.StudentCall object to a BO.CallInList object.
    /// </summary>
    /// <param name="studentCall">The DO.StudentCall object to convert.</param>
    /// <returns>A BO.CallInList object representing the student call.</returns>
    internal static BO.CallInList ConvertFromDoToBo(DO.StudentCall studentCall)
    {
        var maxCompletionTime = AdminManager.Now - manager.GetRiskTimeRange();

        var lastAssignment = s_dal.Assignment
          .ReadAll(a => a.StudentCallId == studentCall.Id)
          .OrderByDescending(a => a.EntryTime)
          .FirstOrDefault();

        var status = CalculateCallStatus(studentCall);

        var totalAssignments = s_dal.Assignment
            .ReadAll(a => a.StudentCallId == studentCall.Id)
            .Count();

        return new BO.CallInList
        {
            Id = lastAssignment?.Id,
            CallId = studentCall.Id,
            Subject = (BO.Subjects)studentCall.Subject,
            OpeningTime = studentCall.OpenTime,
            RemainingTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - AdminManager.Now
                : maxCompletionTime - AdminManager.Now,
            LastTutorName = lastAssignment != null
                ? s_dal.Tutor.Read(lastAssignment.TutorId)!.FullName
                : null,
            CompletionTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - studentCall.OpenTime
                : (TimeSpan?)null,
            Status = status,
            TotalAssignments = totalAssignments
        };
    }
    
    internal static BO.CallStatus CalculateCallStatus2(DO.StudentCall studentCall)
    {
        var maxCompletionTime = studentCall.OpenTime.AddHours(2); // Example: maximum time to complete the call is 2 hours.
        var lastAssignment = s_dal.Assignment
           .ReadAll(a => a.StudentCallId == studentCall.Id)
           .OrderByDescending(a => a.EntryTime)
           .FirstOrDefault();

        // If the call has a final time, determine its status based on the end treatment.
        if (studentCall.FinalTime.HasValue)
        {
            return lastAssignment?.EndOfTreatment switch
            {
                DO.EndOfTreatment.Treated => BO.CallStatus.Closed, // Call is treated and closed.
                DO.EndOfTreatment.SelfCancel => BO.CallStatus.Open, // Self-canceled, still open.
                DO.EndOfTreatment.ManagerCancel => BO.CallStatus.Open, // Manager canceled, still open.
                DO.EndOfTreatment.Expired => BO.CallStatus.Expired, // Call expired.
                _ => BO.CallStatus.Open // Default to open if no other status.
            };
        }

        // If the maximum completion time has passed, mark the call as expired.
        if (AdminManager.Now >= maxCompletionTime)
        {
            return BO.CallStatus.Expired;
        }

        // If the call is in progress, check if it is at risk of missing the deadline.
        if (lastAssignment != null)
        {
            return maxCompletionTime - AdminManager.Now <= AdminManager.RiskTimeSpan // 30-minute risk range
           ? BO.CallStatus.InProgressAtRisk
           : BO.CallStatus.InProgress;
        }

        // If the call is open, check if it's at risk of missing the deadline.
        return maxCompletionTime - AdminManager.Now <= AdminManager.RiskTimeSpan// 30-minute risk range
            ? BO.CallStatus.OpenInRisk
            : BO.CallStatus.Open;
    }

    /// <summary>
    /// Calculates the status of a student call based on its properties and the current time.
    /// </summary>
    /// <param name="studentCall">The student call to calculate the status for.</param>
    /// <returns>The calculated status of the student call.</returns>
    internal static BO.CallStatus CalculateCallStatus(DO.StudentCall studentCall)
    {
        var lastAssignment = s_dal.Assignment
           .ReadAll(a => a.StudentCallId == studentCall.Id)
           .OrderByDescending(a => a.EntryTime)
           .FirstOrDefault();
        bool isCallExpired = studentCall.FinalTime.HasValue && studentCall.FinalTime < AdminManager.Now;
        bool isCallInRisk = studentCall.FinalTime.HasValue && studentCall.FinalTime - AdminManager.Now < AdminManager.RiskTimeSpan;
        if (isCallExpired)
        {
            return BO.CallStatus.Expired;
        }

        if (lastAssignment == null)
            return isCallInRisk ? BO.CallStatus.OpenInRisk : BO.CallStatus.Open;

        if (isCallInRisk)
            return BO.CallStatus.InProgressAtRisk;

        return lastAssignment?.EndOfTreatment switch
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
        try
        {
            (call.Latitude, call.Longitude) = Tools.GetCoordinates(call.FullAddress);
        }
        catch (Exception ex)
        {
            throw new BO.BlValidationException($"Failed to get coordinates for the address: {ex.Message}");
        }

        // Validate open/final time
        if (call.OpenTime >= call.FinalTime)
            throw new BO.BlValidationException("Start time must be earlier than end time.");

        // Optional: validate description length if needed
        if (call.Description != null && call.Description.Length > 500)
            throw new BO.BlValidationException("Description is too long. Maximum 500 characters allowed.");
    }


    /// <summary>
    /// Updates the status of calls that have passed their final time.
    /// </summary>
    internal static void UpdateStatusCalls()
    {
        var now = AdminManager.Now;
        var calls = s_dal.StudentCall.ReadAll(c => c.FinalTime.HasValue && c.FinalTime <= now);

        foreach (var call in calls)
        {
            var assignments = s_dal.Assignment.ReadAll(a => a.StudentCallId == call.Id);

            if (assignments == null)
            {
                var newAssignment = new DO.Assignment
                {
                    StudentCallId = call.Id,
                    TutorId = 0,
                    EntryTime = now,
                    EndOfTreatment = DO.EndOfTreatment.Expired,
                    EndTime = now
                };
                s_dal.Assignment.Create(newAssignment);
                Observers.NotifyItemUpdated(newAssignment.StudentCallId);
            }
            else
            {
                foreach (var assignment in assignments)
                {
                    if (assignment.EndTime == null)
                    {
                        var updated = assignment with
                        {
                            EndTime = now,
                            EndOfTreatment = DO.EndOfTreatment.Expired
                        };
                        s_dal.Assignment.Update(updated);
                        Observers.NotifyItemUpdated(updated.StudentCallId);
                    }
                }
            }
        }

        Observers.NotifyListUpdated();
    }

}
