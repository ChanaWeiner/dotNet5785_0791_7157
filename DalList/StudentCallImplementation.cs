using System.Runtime.CompilerServices;
using DalApi;
using DO;

namespace Dal;

internal class StudentCallImplementation : IStudentCall
{
    /// <summary>
    /// Creates a new student call and adds it to the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(StudentCall item)
    {
        int id = Config.NextStudentCallId;
        StudentCall copy = item with { Id = id };
        DataSource.StudentCalls.Add(copy);
    }

    /// <summary>
    /// Deletes a student call by its ID from the data source.
    /// Throws an exception if the student call does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        DataSource.StudentCalls.Remove(studentCall);
    }

    /// <summary>
    /// Deletes all student calls from the data source.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        DataSource.StudentCalls.Clear();
    }

    /// <summary>
    /// Reads a student call by its ID.
    /// Returns null if the student call does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public StudentCall? Read(int id)
    {
        return DataSource.StudentCalls.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Reads a student call based on a filter.
    /// Returns null if no matching student call is found.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        return DataSource.StudentCalls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all student calls, optionally filtered by a predicate.
    /// If no filter is provided, all student calls are returned.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
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

    /// <summary>
    /// Updates an existing student call in the data source.
    /// Throws an exception if the student call does not exist.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"An object of type student call with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
