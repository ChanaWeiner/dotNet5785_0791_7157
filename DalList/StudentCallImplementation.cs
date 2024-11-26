using DalApi;
using DO;

namespace Dal;

internal class StudentCallImplementation : IStudentCall
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
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        DataSource.StudentCalls.Remove(studentCall);
    }

    public void DeleteAll()
    {
        DataSource.StudentCalls.Clear();
    }

    public StudentCall? Read(int id)
    {
        return DataSource.StudentCalls.FirstOrDefault(x => x.Id == id);
    }
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        return (DataSource.StudentCalls).FirstOrDefault(filter);
    }
    public IEnumerable<StudentCall> ReadAll(Func<StudentCall, bool>? filter = null)
    {
        if (filter != null)
        {
            return from item in DataSource.StudentCalls
                   where filter(item)
                   select item;
        }
        else
        {
            return from item in DataSource.StudentCalls
                   select item;
        }
    }

    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"An object of type student call with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
