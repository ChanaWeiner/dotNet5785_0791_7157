namespace Dal;

internal static class Config
{
    internal const int startStudentCallId = 1000;
    private static int nextStudentCallId = startStudentCallId;
    internal static int NextStudentCallId { get => nextStudentCallId++; }
    internal const int startAssignmentId = 1000;
    private static int nextAssignmentId = startAssignmentId;
    internal static int NextAssignmentId { get => nextAssignmentId++; }
    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static void Reset()
    {
        nextStudentCallId = startStudentCallId;
        nextAssignmentId = startAssignmentId;
        Clock = DateTime.Now;

    }


}
