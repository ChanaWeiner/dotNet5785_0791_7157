namespace Dal;

internal static class config
{
    internal const int startCourseId = 1000;
    private static int nextCourseId = startCourseId;
    internal static int NextCourseId { get => nextCourseId++; }

    internal static DateTime Clock { get; set; } = DateTime.Now;

    internal static void Reset()
    {
        nextCourseId = startCourseId;
        Clock = DateTime.Now;
    }


}
