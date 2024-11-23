namespace DO;

public record StudentCall
(
    int Id,
    Subjects subject,
    string Description,
    string FullAddress,
    string FullName,
    string CellNumber,
    string Email,
    double Latitude,
    double Longitude,
    DateTime? OpenTime,
    DateTime? FinalTime

 )

{
    public StudentCall() : this(0, 0, "", "", "", "", "", 0, 0, null, null) { }
}
