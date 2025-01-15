namespace BO;

public class CallAssignInList
{
    // Volunteer ID
    public int? VolunteerId { get; set; } 

    // Volunteer name
    public string? VolunteerName { get; set; } 

    // Time when the call was assigned
    public DateTime AssignmentTime { get; set; } 

    // Actual end time of handling the call
    public DateTime? ActualEndTime { get; set; }

    // Type of end handling for the call
    public EndOfTreatment? EndType { get; set; } 
}
