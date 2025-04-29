using DalApi;
using System.Diagnostics;
namespace Dal;

sealed internal class DalXml : IDal
{
    /// <summary>
    /// Singleton instance of DalXml.
    /// Provides a centralized access point to the data access layer.
    /// </summary>
    public static IDal Instance { get; } = new DalXml();

    // Private constructor to prevent instantiating multiple instances.
    private DalXml() { }

    /// <summary>
    /// Access to student call operations.
    /// Provides methods for managing student calls.
    /// </summary>
    public IStudentCall StudentCall { get; } = new StudentCallImplementation();

    /// <summary>
    /// Access to assignment operations.
    /// Provides methods for managing assignments.
    /// </summary>
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// <summary>
    /// Access to tutor operations.
    /// Provides methods for managing tutors.
    /// </summary>
    public ITutor Tutor { get; } = new TutorImplementation();

    /// <summary>
    /// Access to configuration operations.
    /// Provides methods for managing system configurations.
    /// </summary>
    public IConfig Config { get; } = new ConfigImplementation();

    /// <summary>
    /// Resets the database by clearing all data and resetting configurations to defaults.
    /// This method deletes all student calls, tutors, and assignments, and resets the configurations.
    /// </summary>
    public void ResetDB()
    {
        StudentCall.DeleteAll();
        Tutor.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
