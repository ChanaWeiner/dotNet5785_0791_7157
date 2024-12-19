namespace Dal;

internal static class Config
{
    /// The starting ID for student calls.
    internal const int startStudentCallId = 1000;

    /// The next available ID for student calls.
    private static int nextStudentCallId = startStudentCallId;
    /// Gets the next available student call ID and increments it.
    internal static int NextStudentCallId { get => nextStudentCallId++; }

    /// The starting ID for assignments.
    internal const int startAssignmentId = 1000;

    /// The next available ID for assignments.
    private static int nextAssignmentId = startAssignmentId;
    /// Gets the next available assignment ID and increments it.
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    /// The current clock time.
    internal static DateTime Clock { get; set; } = DateTime.Now;

    /// The current risk time span, initialized to zero.
    internal static TimeSpan RiskTimeSpan{ get; set; }= TimeSpan.Zero;

    /// Resets the student call and assignment IDs, and the clock.
    internal static void Reset()
    {
        nextStudentCallId = startStudentCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskTimeSpan=TimeSpan.Zero;
    }
}
