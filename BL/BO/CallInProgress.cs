//using Helpers;
using Helpers;
using Microsoft.VisualBasic;

namespace BO; 

public class CallInProgress
{
    // Unique ID of the assignment entity.
    public int Id { get; set; }

    // Unique running ID of the call entity.
    public int CallId { get; set; }

    // Type of the call.
    public Subjects Subject { get; set; }

    // Description of the call.
    public string Description { get; set; }

    // Full address of the call.
    public string FullAddress { get; set; }

    // Opening time of the call.
    public DateTime OpenTime { get; set; }

    // Maximum time to complete the call (can be null).
    public DateTime? MaxEndTime { get; set; }

    // Time when the call was assigned to the volunteer.
    public DateTime EntryTime { get; set; }

    // Distance of the call from the volunteer.
    public double Distance { get; set; }

    // Status of the call in progress.
    public CallStatus Status { get; set; }
    public override string ToString() => Tools.ToStringProperty(this);

}