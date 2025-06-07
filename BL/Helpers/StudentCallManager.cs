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
    /// Determines the call status based on the final time and the current time.
    /// </summary>
    /// <param name="studentCall">The student call to check the status for.</param>
    /// <returns>The status of the student call.</returns>
    internal static BO.CallStatus GetCallStatus(DO.StudentCall studentCall)
    {
        DateTime? FinalTime = studentCall.FinalTime;
        DateTime clock = AdminManager.Now;
        TimeSpan? range = manager.GetRiskTimeRange();

        if (FinalTime - clock <= range)
        {
            return BO.CallStatus.InProgressAtRisk; // The call is nearing its end and is at risk.
        }
        return BO.CallStatus.InProgress; // The call is in progress without risk.
    }

    /// <summary>
    /// Retrieves a list of student calls, filtered by the given criteria.
    /// </summary>
    /// <param name="filterField">The field to filter the calls by (optional).</param>
    /// <param name="filterValue">The value to filter by (optional).</param>
    /// <returns>A list of student calls matching the filter criteria.</returns>
    internal static List<BO.CallInList> GetCallInList(BO.StudentCallField? filterField, object? filterValue)
    {
        // Retrieve all student calls from the database, applying the filter if needed.
        List<DO.StudentCall> doCalls = s_dal.StudentCall.ReadAll((DO.StudentCall c) =>
        {
            if (filterField == null)
                return true;

            var propertyInfo = c.GetType().GetProperty(filterField.ToString()!);
            if (propertyInfo == null)
                throw new BO.BlValidationException($"Property '{filterField}' not found on {c.GetType().Name}");

            var propertyValue = propertyInfo.GetValue(c);
            return propertyValue != null && propertyValue.ToString() == filterValue.ToString();
        }).ToList() ?? throw new BO.BlDoesNotExistException("No StudentCall records found matching the filter criteria.");

        // Convert the data to BO objects and return the list.
        List<BO.CallInList> callInList = doCalls.Select(ConvertFromDoToBo).ToList();
        return callInList;
    }

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

    /// <summary>
    /// Calculates the status of a student call based on its properties and the current time.
    /// </summary>
    /// <param name="studentCall">The student call to calculate the status for.</param>
    /// <returns>The calculated status of the student call.</returns>
    internal static BO.CallStatus CalculateCallStatus(DO.StudentCall studentCall)
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
    /// Validates a student call object before creating or updating it.
    /// </summary>
    /// <param name="call">The student call object to validate.</param>
    internal static void Validation(BO.StudentCall call)
    {
         
        // Validate subject (Enum)
        if (!Enum.IsDefined(typeof(BO.Subjects), call.Subject))

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
        // Retrieve all calls that have a final time later than the current time.
        var calls = s_dal.StudentCall.ReadAll(c => c.FinalTime > AdminManager.Now);
        foreach (var call in calls)
        {
            // Retrieve all assignments for the call and update their status.
            var callAssignments = s_dal.Assignment.ReadAll(a => a.StudentCallId == call.Id);
            foreach (var callAssignment in callAssignments)
            {
                var updateAssignment = callAssignment with { EndOfTreatment = DO.EndOfTreatment.Expired };
                s_dal.Assignment.Update(updateAssignment); // Mark assignment as expired.
            }
        }
    }
}
