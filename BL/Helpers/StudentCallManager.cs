

using BlApi;
using BlImplementation;
using DalApi;

namespace Helpers;

internal class StudentCallManager
{
    private static IDal s_dal = DalApi.Factory.Get; //stage 4
    private static IAdmin manager = new AdminImplementation();
    internal static BO.CallStatus GetCallStatus(DO.StudentCall studentCall)
    {
        DateTime? FinalTime = studentCall.FinalTime;
        DateTime clock = ClockManager.Now;
        TimeSpan? range = manager.GetRiskTimeRange();
        if (FinalTime - clock <= range)
        {
            return BO.CallStatus.InProgressAtRisk;

        }
        return BO.CallStatus.InProgress;
    }
    internal static List<BO.CallInList> GetCallInList(BO.StudentCallField? filterField, object filterValue)
    {
        List<DO.StudentCall> doCalls = s_dal.StudentCall.ReadAll((DO.StudentCall c) =>
        {
            if (filterField == null)
                return true;

            var propertyInfo = c.GetType().GetProperty(filterField.ToString()!);
            if (propertyInfo == null)
                throw new ArgumentException($"Property '{filterField}' not found on {c.GetType().Name}");

            var propertyValue = propertyInfo.GetValue(c);
            return propertyValue != null && propertyValue.Equals(filterValue);
        }).ToList() ?? throw new Exception("No StudentCall records found matching the filter criteria.");

        List<BO.CallInList> callInList = doCalls.Select(ConvertFromDoToBo).ToList();
        return callInList;
    }

    private static BO.CallInList ConvertFromDoToBo(DO.StudentCall studentCall)
    {
        var maxCompletionTime = studentCall.OpenTime.AddHours(2); // לדוגמה, זמן מקסימלי להשלמת קריאה הוא 2 שעות

        // מציאת ההקצאה האחרונה עבור הקריאה

        var lastAssignment = s_dal.Assignment
          .ReadAll(a => a.StudentCallId == studentCall.Id)
          .OrderByDescending(a => a.EntryTime)
          .FirstOrDefault();

        // חישוב סטטוס הקריאה
        var status = CalculateCallStatus(studentCall);

        // חישוב סך ההקצאות לקריאה
        var totalAssignments = s_dal.Assignment
            .ReadAll(a => a.StudentCallId == studentCall.Id)
            .Count();

        // מיפוי האובייקט
        return new BO.CallInList
        {
            Id = lastAssignment?.Id, // מזהה ישות ההקצאה האחרונה (אם קיימת)
            CallId = studentCall.Id, // מזהה ישות הקריאה
            Subject = (BO.Subjects)studentCall.Subject, // סוג הקריאה (ENUM)
            OpeningTime = studentCall.OpenTime, // זמן פתיחה
            RemainingTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - ClockManager.Now // זמן שנותר לסיום
                : maxCompletionTime - ClockManager.Now, // או זמן מקסימלי
            LastTutorName = lastAssignment != null
                ? GetTutorName(lastAssignment.TutorId) // שם המתנדב האחרון
                : null, // או null אם אין הקצאה
            CompletionTime = studentCall.FinalTime.HasValue
                ? studentCall.FinalTime.Value - studentCall.OpenTime // זמן השלמת הטיפול
                : (TimeSpan?)null, // או null אם לא הושלם
            Status = status, // סטטוס הקריאה
            TotalAssignments = totalAssignments // סך כל ההקצאות לקריאה זו
        };
    }

    private static string GetTutorName(int tutorId)
    {
        var tutor = s_dal.Tutor.Read(tutorId);
        return tutor != null ? tutor.FullName : "Unknown";
    }
    internal static BO.CallStatus CalculateCallStatus(DO.StudentCall studentCall)
    {
        var maxCompletionTime = studentCall.OpenTime.AddHours(2); // לדוגמה, זמן מקסימלי להשלמת קריאה הוא 2 שעות
        var lastAssignment = s_dal.Assignment
           .ReadAll(a => a.StudentCallId == studentCall.Id)
           .OrderByDescending(a => a.EntryTime)
           .FirstOrDefault();
        if (studentCall.FinalTime.HasValue)
        {
            return lastAssignment?.EndOfTreatment switch
            {
                DO.EndOfTreatment.Treated => BO.CallStatus.Closed,
                DO.EndOfTreatment.SelfCancel => BO.CallStatus.Open,
                DO.EndOfTreatment.ManagerCancel => BO.CallStatus.Open,
                DO.EndOfTreatment.Expired => BO.CallStatus.Expired,
                _ => BO.CallStatus.Open
            };
        }

        if (ClockManager.Now >= maxCompletionTime)
        {
            return BO.CallStatus.Expired;
        }

        if (lastAssignment != null)
        {
            return DateTime.Now >= maxCompletionTime.AddMinutes(-30) // טווח זמן סיכון
           ? BO.CallStatus.InProgressAtRisk
           : BO.CallStatus.InProgress;
        }

        return DateTime.Now >= maxCompletionTime.AddMinutes(-30) // טווח זמן סיכון
            ? BO.CallStatus.OpenInRisk
            : BO.CallStatus.Open;
    }

    internal static void Validation(BO.StudentCall call)
    {
        // בדיקת תקינות ערכים
        if (call.OpenTime >= call.FinalTime)
            throw new ArgumentException("זמן פתיחה חייב להיות קטן מזמן הסיום");

        try
        {
            (call.Latitude, call.Longitude) = Tools.GetCoordinates(call.FullAddress!);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    internal static void PeriodicStudentCallsUpdates(DateTime oldClock, DateTime newClock)
    {
                throw new NotImplementedException();

    }
    internal static void updateStatusCalls()
    {
        var calls = s_dal.StudentCall.ReadAll(c => c.FinalTime > ClockManager.Now);
        foreach (var call in calls)
        {
            var callAssignments = s_dal.Assignment.ReadAll(a=>a.StudentCallId==call.Id);
            foreach (var callAssignment in callAssignments)
            {
                var updateAssignment=callAssignment with { EndOfTreatment= DO.EndOfTreatment.Expired };
                s_dal.Assignment.Update(updateAssignment);
            }
        }

    }
}
