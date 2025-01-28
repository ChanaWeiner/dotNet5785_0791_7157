

using BlImplementation;
using BO;
using DalApi;

namespace Helpers;

internal class StudentCallManager
{
    private static IDal s_dal = Factory.Get; //stage 4

    internal static BO.CallStatus GetCallStatus(DO.StudentCall studentCall)
    {
        DateTime? FinalTime = studentCall.FinalTime;
        DateTime clock = AdminImplementation.GetSystemClock();
        TimeSpan? range = AdminImplementation.GetRiskTimeRange();
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

    private static BO.CallInList ConvertFromDoToBo(DO.StudentCall tutorCall)
    {
        // תחילת המיפוי
        var boCall = new BO.CallInList
        {
            // מזהה הקצאה
            Id = null,  // בהנחה שב-StudentCall אין שדה AssignmentId ישירות, ולכן נניח שצריך להשאיר את זה null (לפי ההסבר שלך)

            // מזהה קריאה (CallId) מקבל ערך מ-StudentCall
            CallId = tutorCall.Id,

            // סוג הקריאה - מיפוי לפי סוג הקריאה ב-DO
            CallType = tutorCall.Subject,

            // זמן פתיחה של הקריאה
            OpeningTime = tutorCall.OpenTime,

            // זמן שנותר להשלמת הקריאה - חישוב הזמן הנותר עד המועד הסיום, אם יש (בהנחה שיש לך משתנים כמו maxTimeCompletion)
            RemainingTime = tutorCall.FinalTime.HasValue
                ? tutorCall.FinalTime.Value - DateTime.Now
                : (TimeSpan?)null, // אם אין תאריך סיום, נחזיר null

            // שם המתנדב האחרון שהוקצה לקריאה - במקרה שאין הקצאה עדיין (Assignment לא נמצא), זה null
            LastVolunteerName = GetLastVolunteerName(tutorCall.Id),

            // זמן סיום הטיפול - חישוב זמן ההשלמה, אם הקריאה כבר סויימה
            CompletionTime = tutorCall.FinalTime.HasValue
                ? tutorCall.FinalTime.Value - tutorCall.OpenTime
                : (TimeSpan?)null,

            // סטטוס הקריאה - לפי המאפיינים שהוזכרו (כגון טווח זמן סיכון או האם קריאה פתוחה/בטיפול/סגורה)
            Status = CalculateCallStatus(tutorCall.Id),

            // סך ההקצאות לקריאה - ניתן לחשב לפי כמות ההקצאות שנעשו לקריאה זו ב-DO.Assignment
            TotalAssignments = GetTotalAssignmentsForCall(tutorCall.Id)
        };

        return boCall;
    }


}
