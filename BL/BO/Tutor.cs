//using Helpers;
namespace BO;

public class Tutor
{
    public int Id { get; init; }
    public string FullName { get; set; }
    public string CellNumber { get; set; }
    public string Email { get; set; }
    public string? Password { get; set; }
    public string? CurrentAddress { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Role Role { get; set; }
    public bool Active { get; set; }
    public double Distance { get; set; }
    public DistanceType DistanceType { get; set; }

    // Total calls handled by the tutor
    public int TotalCallsHandled { get; private set; }

    // Total calls in the allocation entity for the tutor with "Handled" status
    public int TotalCallsHandledStatus { get; private set; }

    // Total calls canceled by the tutor
    public int TotalCallsCanceled { get; private set; }

    // Total calls in the allocation entity for the tutor with "Self Cancellation" status
    public int TotalCallsSelfCanceled { get; private set; }

    // Total calls selected for handling that expired
    public int TotalCallsExpired { get; private set; }

    // Call currently in progress for the tutor
    public BO.CallInProgress? CurrentCallInProgress { get; set; }

    //public override string ToString() => this.ToStringProperty();
}
