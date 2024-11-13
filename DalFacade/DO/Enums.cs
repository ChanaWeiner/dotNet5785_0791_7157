namespace DO;
//enum Role { MASTER_TUTOR , BEGINER_TUTOR, MANEGAR};
public record Volunteer(
    int Id,
    string FullName,
    string CellNumber,
    string Email,
    string Password,
    string CurrentAddres,
    //Role role
    bool Active

 )
{
    public Volunteer() : this(0,"","","","","",false) { }
}