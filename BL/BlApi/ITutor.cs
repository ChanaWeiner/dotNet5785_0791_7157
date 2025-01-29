

namespace BlApi;

public interface ITutor
{
    public BO.Role LogIn(string name,string password);
    public IEnumerable<BO.TutorInList> SortTutorsInList(bool isActive, BO.TutorSortField sortField);
    public BO.Tutor Read(int id);
    public void Update(int id, BO.Tutor tutor);
    public void Delete(int id);
    public void Create(BO.Tutor tutor);
}
