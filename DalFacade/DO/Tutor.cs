namespace DO;

public record Tutor(
    int Id,
    string FullName,
    string CellNumber,
    string Email,
    string? Password,
    string? CurrentAddress,
    double? Latitude,
    double? Longitude,
    Role Role,
    bool Active,
    double Distance,
    DistanceType DistanceType
 )
{
    public Tutor() : this(0, "", "", "", "", "",0,0,0, false,0,0) { }
}
