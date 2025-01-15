

namespace BlApi;

public interface IBl
{
    ITutor Tutor { get; }
    IStudentCall Student { get; }
    IAdmin Admin { get; }

}
