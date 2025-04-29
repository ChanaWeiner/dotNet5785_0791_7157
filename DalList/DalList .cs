
namespace Dal;
using DalApi;

/// <summary>
/// Singleton implementation of the data access layer (DAL).
/// Provides access to student calls, assignments, tutors, and configuration management.
/// </summary>
sealed internal class DalList : IDal
{
    /// <summary>
    /// Singleton instance of DalList.
    /// </summary>
    public static IDal Instance { get; } = new DalList();
    private DalList() { }

    /// <summary>
    /// Provides functionality for managing student calls.
    /// </summary>
    public IStudentCall StudentCall { get; } = new StudentCallImplementation();

    /// <summary>
    /// Provides functionality for managing assignments.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Provides functionality for managing tutors.
    /// </summary>
    public ITutor Tutor { get; } = new TutorImplementation();

    /// <summary>
    /// Provides configuration management functionality.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by deleting all entries in student calls, tutors, assignments, and resetting configuration.
    /// </summary>
    public void ResetDB()
    {
        StudentCall.DeleteAll();
        Tutor.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}