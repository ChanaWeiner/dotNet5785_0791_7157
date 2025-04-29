using DalApi;
using DO;
namespace Dal;

internal class StudentCallImplementation : IStudentCall
{
    /// <summary>
    /// Adds a new student call to the data storage.
    /// This method creates a new student call and saves it to the XML file.
    /// If the ID is 0, a new ID is generated.
    /// </summary>
    public void Create(StudentCall item)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        int id = Config.NextStudentCallId;
        if (item.Id != 0)
        {
            id = item.Id;
        }
        StudentCall copy = item with { Id = id };
        StudentCalls.Add(copy);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// <summary>
    /// Deletes a student call by its ID.
    /// This method removes the student call with the specified ID from the data storage.
    /// Throws an exception if the student call is not found.
    /// </summary>
    public void Delete(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={id} does not exist");
        StudentCalls.Remove(studentCall);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// <summary>
    /// Deletes all student calls from the data storage.
    /// This method clears all student call records in the storage.
    /// </summary>
    public void DeleteAll()
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCalls.Clear();
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// <summary>
    /// Reads a specific student call by its ID.
    /// This method retrieves the student call with the specified ID from the data storage.
    /// </summary>
    public StudentCall? Read(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Reads a specific student call based on a filter condition.
    /// This method retrieves a student call that matches the provided filter condition.
    /// </summary>
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all student calls or filters them based on a provided condition.
    /// This method retrieves all student calls or applies the given filter to the list.
    /// </summary>
    public IEnumerable<StudentCall> ReadAll(Func<StudentCall, bool>? filter = null)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        if (filter != null)
        {
            return from item in StudentCalls
                   where filter(item)
                   select item;
        }
        else
        {
            return from item in StudentCalls
                   select item;
        }
    }

    /// <summary>
    /// Updates an existing student call in the data storage.
    /// This method deletes the existing student call and creates a new one with updated data.
    /// </summary>
    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
