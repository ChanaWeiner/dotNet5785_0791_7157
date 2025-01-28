
namespace BO;

public class TutorInList
{
    // Volunteer ID
    public int Id { get; set; }

    // Full name (first and last)
    public string FullName { get; set; } 

    // Active status
    public bool IsActive { get; set; } 

    // Total calls handled by the volunteer
    public int TotalHandledCalls { get; set; } 

    // Total calls canceled by the volunteer
    public int TotalCancelledCalls { get; set; } 

    // Total calls expired for the volunteer
    public int TotalExpiredCalls { get; set; } 

    // ID of the current call being handled (if any)
    public int? CurrentCallId { get; set; }

    // Type of the current call being handled
    public Subjects CurrentSubject { get; set; }
}



