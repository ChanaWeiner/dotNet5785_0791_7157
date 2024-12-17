using DalApi;
using DO;

namespace Dal;

internal class StudentCallImplementation : IStudentCall
{
    /// Creates a new student call and adds it to the data source.
    public void Create(StudentCall item)
    {
        int id = Config.NextStudentCallId;
        StudentCall copy = item with { Id = id };
        DataSource.StudentCalls.Add(copy);
    }

    /// Deletes a student call by its ID from the data source.
    /// Throws an exception if the student call does not exist.
    public void Delete(int id)
    {
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        DataSource.StudentCalls.Remove(studentCall);
    }

    /// Deletes all student calls from the data source.
    public void DeleteAll()
    {
        DataSource.StudentCalls.Clear();
    }

    /// Reads a student call by its ID.
    /// Returns null if the student call does not exist.
    public StudentCall? Read(int id)
    {
        return DataSource.StudentCalls.FirstOrDefault(x => x.Id == id);
    }

    /// Reads a student call based on a filter.
    /// Returns null if no matching student call is found.
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        return DataSource.StudentCalls.FirstOrDefault(filter);
    }

    /// Reads all student calls, optionally filtered by a predicate.
    /// If no filter is provided, all student calls are returned.
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

    /// Updates an existing student call in the data source.
    /// Throws an exception if the student call does not exist.
    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"An object of type student call with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
