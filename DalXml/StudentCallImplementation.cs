using DalApi;
using DO;
namespace Dal;

internal class StudentCallImplementation : IStudentCall
{
    /// Adds a new student call to the data storage.
    public void Create(StudentCall item)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        int id = Config.NextStudentCallId;
        StudentCall copy = item with { Id = id };
        StudentCalls.Add(copy);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// Deletes a student call by its ID.
    public void Delete(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={id} does not exist");
        StudentCalls.Remove(studentCall);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// Deletes all student calls from the data storage.
    public void DeleteAll()
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCalls.Clear();
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    /// Reads a specific student call by its ID.
    public StudentCall? Read(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(x => x.Id == id);
    }

    /// Reads a specific student call based on a filter condition.
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(filter);
    }

    /// Reads all student calls or filters them based on a provided condition.
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

    /// Updates an existing student call in the data storage.
    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
