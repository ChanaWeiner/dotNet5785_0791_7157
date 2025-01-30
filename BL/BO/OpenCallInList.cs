

using Helpers;

namespace BO;

public class OpenCallInList
{
    // Sequential ID of the call entity
    public int Id { get; set; }

    // Type of the call
    public Subjects Subject { get; set; }


    // Description of the call
    public string? Description { get; set; } 

    // Full address of the call
    public string FullAddress { get; set; } 

    // Opening time of the call
    public DateTime OpeningTime { get; set; }

    // Maximum time for call completion
    public DateTime? MaxCompletionTime { get; set; } 

    // Distance of the call from the volunteer
    public double DistanceFromVolunteer { get; set; }
    public override string ToString() => Tools.ToStringProperty(this);

}
