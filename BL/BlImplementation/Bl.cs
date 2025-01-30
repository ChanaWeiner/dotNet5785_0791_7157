namespace BlImplementation;
using BlApi;
internal class Bl : IBl
{
    public ITutor Tutor { get; } = new TutorImplementation();

    public IStudentCall StudentCall { get; } = new StudentCallImplementation();

    public IAdmin Admin { get; } = new AdminImplementation();

}