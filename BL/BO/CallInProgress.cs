//using Helpers;
using Microsoft.VisualBasic;

namespace BO; 

public class CallInProgress
{
    /// Unique ID of the assignment entity.
    public int Id { get; private set; }

    /// Unique running ID of the call entity.
    public int CallId { get; private set; }

    /// Type of the call.
    public CallType CallType { get; private set; }

    /// Description of the call.
    public string Description { get; private set; }

    /// Full address of the call.
    public string FullAddress { get; private set; }

    /// Opening time of the call.
    public DateTime OpenTime { get; private set; }

    /// Maximum time to complete the call (can be null).
    public DateTime? MaxEndTime { get; private set; }

    /// Time when the call was assigned to the volunteer.
    public DateTime EntryTime { get; private set; }

    /// Distance of the call from the volunteer.
    public double Distance { get; private set; }

    /// Status of the call in progress.
    public CallStatus Status { get; private set; }
}