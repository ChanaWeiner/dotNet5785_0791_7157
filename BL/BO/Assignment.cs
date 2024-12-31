namespace BO;

internal class Assignment
{
    public int Id { get; init; } /// Unique identifier, set only during object creation.
    public int StudentCallId { get; init; } /// Identifier for the student call, immutable after initialization.
    public int TutorId { get; init; } /// Identifier for the tutor, immutable after initialization.
    public DateTime? EntryTime { get; set; } /// Entry time, can be updated if needed.
    public DateTime? EndTime { get; set; } /// End time, can be updated if needed.
    public EndOfTreatment EndOfTreatment { get; set; } /// Status of the treatment, may change over time.

}
