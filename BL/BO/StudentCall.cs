
using Helpers;

namespace BO;

public class StudentCall
{
    public int Id { get; init; }
    public Subjects Subject { get; set; }
    public string? Description { get; set; }
    public string FullAddress { get; set; }
    public string FullName { get; set; }
    public string CellNumber { get; set; }
    public string Email { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public DateTime OpenTime { get; set; }
    public DateTime? FinalTime { get; set; }
    public CallStatus Status { get; set; }
    public List<BO.CallAssignInList> CallsAssignInList { get; set; }= new List<BO.CallAssignInList>();
    public override string ToString() => Tools.ToStringProperty(this);

}
