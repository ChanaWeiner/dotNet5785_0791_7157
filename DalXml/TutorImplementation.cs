using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class TutorImplementation : ITutor
{
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Tutor with ID={item.Id} already exists");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Add(item);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} is not exists");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Remove(tutor);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    public void DeleteAll()
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Clear();
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    public Tutor? Read(int id)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(x => x.Id == id);
    }

    public Tutor? Read(Func<Tutor, bool> filter)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(filter);
    }

    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return filter == null
            ? new List<Tutor>(Tutors)
            : new List<Tutor>(Tutors.Where(filter));
    }

    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new DalDoesNotExistException($"An object of type Tutor with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
