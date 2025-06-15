namespace BO;
public enum Role { MasterTutor, BeginnerTutor, Manager, None };
public enum DistanceType { Air, Walking, Driving };
public enum Subjects { English, Math, Grammar, Programming, History, None }
public enum EndOfTreatment { Treated, SelfCancel, ManagerCancel, Expired,None };
public enum CallStatus { Open, InProgress, Closed, Expired, OpenInRisk, InProgressAtRisk,None }

// ENUM for sorting tutors
public enum TutorField { Id, FullName, IsActive, TotalHandledCalls, TotalCancelledCalls, TotalExpiredCalls, CurrentCallId, CurrentCallType, Role }

public enum StudentCallField {
    Id,
    CallId,
    Subject,
    OpeningTime,
    RemainingTime,
    LastTutorName,
    CompletionTime,
    Status,
    TotalAssignments
}


// for OpenCallInList
public enum OpenCallField
{
    Id,
    Subject,
    Description,
    FullAddress,
    OpeningTime,
    MaxCompletionTime,
    DistanceFromTutor,
    None
}

// for ClosedCallInList
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




