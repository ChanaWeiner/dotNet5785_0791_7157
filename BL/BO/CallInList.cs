

namespace BO;

public class CallInList
{
    // Assignment ID
    public int? Id { get; set; }

    // Sequential ID of the call entity
    public int CallId { get; set; }

    // Type of the call
    public Subjects Subject { get; set; }

    // Opening time of the call
    public DateTime OpeningTime { get; set; }

    // Remaining time to complete the call
    public TimeSpan? RemainingTime { get; set; }

    // Last volunteer name assigned to the call
    public string? LastVolunteerName { get; set; }

    // Total time taken to complete the call
    public TimeSpan? CompletionTime { get; set; }

    // Status of the call
    public CallStatus Status { get; set; } // Cannot be null, computed based on end type, max completion time, and system clock

    // Total assignments made for the call
    public int TotalAssignments { get; set; } // Cannot be null, total count of assignments in DO.Assignment for the current call
}


