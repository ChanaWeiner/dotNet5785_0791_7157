
namespace Dal;

internal static class DataSource
{
    internal static List<DO.StudentCall> StudentCalls { get; } = new();
    internal static List<DO.Tutor> Tutors { get; } = new();
    internal static List<DO.Assignment> Assignments { get; } = new();

}
