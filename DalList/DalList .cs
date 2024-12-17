namespace Dal;
using DalApi;

sealed public class DalList : IDal
{
    /// Provides functionality for managing student calls.
    public IStudentCall StudentCall { get; } = new StudentCallImplementation();

    /// Provides functionality for managing assignments.
    public IAssignment Assignment { get; } = new AssignmentImplementation();

    /// Provides functionality for managing tutors.
    public ITutor Tutor { get; } = new TutorImplementation();

    /// Provides configuration management functionality.
    public IConfig Config { get; } = new ConfigImplementation();

    /// Resets the database by deleting all entries in student calls, tutors, assignments, and resetting configuration.
    public void ResetDB()
    {
        StudentCall.DeleteAll();
        Tutor.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }
}
