using DalApi;
using DO;

namespace Dal;

public class StudentCallImplementation : IStudentCall
{
    public void Create(StudentCall item)
    {
        int id = Config.NextStudentCallId;
        StudentCall copy = item with { Id = id };
        DataSource.StudentCalls.Add(copy);

    }

    public void Delete(int id)
    {
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new Exception($"Student call with ID={id} is not exists");
        DataSource.StudentCalls.Remove(studentCall);
    }

    public void DeleteAll()
    {
        DataSource.StudentCalls.Clear();
    }

    public StudentCall? Read(int id)
    {
        return DataSource.StudentCalls.Find(x => x.Id == id);
    }

    public List<StudentCall> ReadAll()
    {
        return new List<StudentCall>(DataSource.StudentCalls);
    }

    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new Exception($"An object of type student call with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
