
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
    public string? Password { get; set; }
    public string? CurrentAddress { get; set; }
    public Role Role { get; set; }
    public bool Active { get; set; }
    public double Distance { get; set; }
    public DistanceType DistanceType { get; set; }
}
