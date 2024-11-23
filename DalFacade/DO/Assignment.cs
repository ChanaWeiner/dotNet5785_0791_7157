namespace DO;

public record Assignment(
    int Id,
    int StudentCallId,
    int TutorId,
    DateTime?EntryTime,
    DateTime?EndTime,
    EndOfTreatment EndOfTreatment
)
{
    public Assignment() : this(0, 0, 0, null, null, 0) { }

}
