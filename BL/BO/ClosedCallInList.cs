
namespace BO;

public class ClosedCallInList
{
    // Sequential ID of the call entity
    public int Id { get; set; } 

    // Type of the call
    public CallType CallType { get; set; } 

    // Full address of the call
    public string FullAddress { get; set; }

    // Opening time of the call
    public DateTime OpeningTime { get; set; }

    // Time when the call was assigned
    public DateTime AssignmentTime { get; set; }

    // Actual end time of handling the call
    public DateTime? ActualEndTime { get; set; } 

    // Type of end handling for the call
    public EndOfTreatment? EndType { get; set; }
}
