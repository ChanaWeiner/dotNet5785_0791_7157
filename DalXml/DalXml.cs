using DalApi;

namespace Dal;

sealed public class DalXml : IDal
{
    // Access to student call operations.
    public IStudentCall StudentCall { get; } = new StudentCallImplementation();

    // Access to assignment operations.
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    // Access to tutor operations.
    public ITutor Tutor { get; } = new TutorImplementation();

    // Access to configuration operations.
    public IConfig Config { get; } = new ConfigImplementation();

    // Resets the database by clearing all data and resetting configurations to defaults.
    public void ResetDB()
    {
        StudentCall.DeleteAll();
        Tutor.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
