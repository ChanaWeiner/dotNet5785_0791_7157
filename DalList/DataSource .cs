namespace Dal;

/// <summary>
/// Data source for storing student calls, tutors, and assignments.
/// </summary>
internal static class DataSource
{
    /// <summary>
    /// A list of all student calls.
    /// </summary>
    internal static List<DO.StudentCall> StudentCalls { get; } = new();

    /// <summary>
    /// A list of all tutors.
    /// </summary>
    internal static List<DO.Tutor> Tutors { get; } = new();

    /// <summary>
    /// A list of all assignments.
    /// </summary>
    internal static List<DO.Assignment> Assignments { get; } = new();
}