

using DalApi;

namespace Dal;

sealed public class DalXml : IDal
{

    public IStudentCall StudentCall { get; } = new StudentCallImplementation();
    public IAssignment Assignment { get; } = new AssignmentImplementation();
    public ITutor Tutor { get; } = new TutorImplementation();
    public IConfig Config { get; } = new ConfigImplementation();

    public void ResetDB()
    {
        StudentCall.DeleteAll();
        Tutor.DeleteAll();
        Assignment.DeleteAll();
        Config.Reset();
    }

}
