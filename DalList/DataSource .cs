namespace Dal;

internal static class DataSource
{
    /// A list of all student calls.
    internal static List<DO.StudentCall> StudentCalls { get; } = new();

    /// A list of all tutors.
    internal static List<DO.Tutor> Tutors { get; } = new();

    /// A list of all assignments.
    internal static List<DO.Assignment> Assignments { get; } = new();
}
