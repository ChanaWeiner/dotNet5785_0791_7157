namespace BO;
public enum Role { MasterTutor, BeginnerTutor, Manager };
public enum DistanceType { Air, Walking, Driving };
public enum Subjects { English, Math, Grammar, Programming, History }
public enum EndOfTreatment { Treated, SelfCancel, ManagerCancel, Expired };
public enum CallStatus {InProgress, InProgressAtRisk }

// ENUM for sorting tutors
public enum TutorSortField
{
    Id, // Sort by Tutor ID
    FullName, // Sort by Full Name
    IsActive, // Sort by Active Status
    TotalHandledCalls, // Sort by Total Handled Calls
    TotalCancelledCalls, // Sort by Total Cancelled Calls
    TotalExpiredCalls, // Sort by Total Expired Calls
    CurrentCallId, // Sort by Current Call ID (if any)
    CurrentCallType // Sort by Type of Current Call being handled
}

public enum StudentCallField
{
    Id,              
    CallId,          
    CallType,        
    OpeningTime,     
    RemainingTime,   
    LastVolunteerName,
    CompletionTime,  
    Status,          
    TotalAssignments 
}


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

public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}




