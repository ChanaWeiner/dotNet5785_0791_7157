namespace Dal;

internal static class Config
{
    /// <summary>
    /// The starting ID for student calls.
    /// </summary>
    internal const int startStudentCallId = 1000;

    /// <summary>
    /// The next available ID for student calls.
    /// </summary>
    private static int nextStudentCallId = startStudentCallId;

    /// <summary>
    /// Gets the next available student call ID and increments it.
    /// </summary>
    internal static int NextStudentCallId { get => nextStudentCallId++; }

    /// <summary>
    /// The starting ID for assignments.
    /// </summary>
    internal const int startAssignmentId = 1000;

    /// <summary>
    /// The next available ID for assignments.
    /// </summary>
    private static int nextAssignmentId = startAssignmentId;

    /// <summary>
    /// Gets the next available assignment ID and increments it.
    /// </summary>
    internal static int NextAssignmentId { get => nextAssignmentId++; }

    /// <summary>
    /// The current clock time.
    /// </summary>
    internal static DateTime Clock { get; set; } = DateTime.Now;

    /// <summary>
    /// The current risk time span, initialized to zero.
    /// </summary>
    internal static TimeSpan RiskTimeSpan { get; set; } = TimeSpan.Zero;

    /// <summary>
    /// Resets the student call and assignment IDs, and the clock.
    /// </summary>
    internal static void Reset()
    {
        nextStudentCallId = startStudentCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;
        RiskTimeSpan = TimeSpan.Zero;
    }
}