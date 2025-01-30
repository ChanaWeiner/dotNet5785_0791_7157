

namespace BlApi;

public interface IBl
{
    ITutor Tutor { get; }
    IStudentCall StudentCall { get; }
    IAdmin Admin { get; }

}
