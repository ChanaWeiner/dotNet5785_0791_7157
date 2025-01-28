namespace BO;

public class CallAssignInList
{
    // Tutor ID
    public int? TutorId { get; set; } 

    // Tutor name
    public string? TutorName { get; set; } 

    // Time when the call was assigned
    public DateTime AssignmentTime { get; set; } 

    // Actual end time of handling the call
    public DateTime? ActualEndTime { get; set; }

    // Type of end handling for the call
    public EndOfTreatment? EndType { get; set; } 
}
