using DalApi;
using DO;

namespace Dal;

internal class StudentCallImplementation : IStudentCall
{
    public void Create(StudentCall item)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        int id = Config.NextStudentCallId;
        StudentCall copy = item with { Id = id };
        StudentCalls.Add(copy);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);
    }

    public void Delete(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCall studentCall = Read(id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        StudentCalls.Remove(studentCall);
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);

    }

    public void DeleteAll()
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        StudentCalls.Clear();
        XMLTools.SaveListToXMLSerializer(StudentCalls, Config.s_studentcalls_xml);

    }

    public StudentCall? Read(int id)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(x => x.Id == id);
    }
    public StudentCall? Read(Func<StudentCall, bool> filter)
    {
        List<StudentCall> StudentCalls = XMLTools.LoadListFromXMLSerializer<StudentCall>(Config.s_studentcalls_xml);
        return StudentCalls.FirstOrDefault(filter);
    }
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

    public void Update(StudentCall item)
    {
        StudentCall studentCall = Read(item.Id);
        if (studentCall == null)
            throw new DalDoesNotExistException($"An object of type student call with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
