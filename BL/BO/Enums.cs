namespace BO;
public enum Role { MasterTutor, BeginnerTutor, Manager };
public enum DistanceType { Air, Walking, Driving };
public enum Subjects { English, Math, Grammar, Programming, History }
public enum EndOfTreatment { Treated, SelfCancel, ManagerCancel, Expired };
public enum CallStatus { Open, InProgress, Closed, Expired, OpenInRisk, InProgressAtRisk }

// ENUM for sorting tutors
public enum TutorSortField { Id, FullName, IsActive, TotalHandledCalls, TotalCancelledCalls, TotalExpiredCalls, CurrentCallId, CurrentCallType }

public enum StudentCallField{ Id, CallId, Subject, OpeningTime, RemainingTime, LastVolunteerName, CompletionTime, Status, TotalAssignments }


// עבור OpenCallInList
public enum OpenCallField
{
    Id,
    CallType,
    Description,
    FullAddress,
    OpeningTime,
    MaxCompletionTime,
    DistanceFromVolunteer
}

// עבור ClosedCallInList
public enum ClosedCallField
{
    Id,
    CallType,
    FullAddress,
    OpeningTime,
    AssignmentTime,
    ActualEndTime,
    EndType
}

public enum TimeUnit { Minute, Hour, Day, Month, Year }




