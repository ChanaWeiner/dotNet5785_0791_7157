
namespace DalApi;

public interface IDal
{
    IStudentCall StudentCall { get; }
    IAssignment Assignment { get; }
    ITutor Tutor { get; }
    IConfig Config { get; }
    void ResetDB();
}
