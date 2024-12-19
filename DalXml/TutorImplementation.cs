using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class TutorImplementation : ITutor
{
    /// Creates a new tutor and adds it to the data storage.
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Tutor with ID={item.Id} already exists");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Add(item);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// Deletes a tutor by its ID.
    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} does not exist");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Remove(tutor);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// Deletes all tutors from the data storage.
    public void DeleteAll()
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Clear();
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// Reads a specific tutor by its ID.
    public Tutor? Read(int id)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(x => x.Id == id);
    }

    /// Reads a specific tutor based on a filter condition.
    public Tutor? Read(Func<Tutor, bool> filter)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(filter);
    }

    /// Reads all tutors or filters them based on a provided condition.
    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return filter == null
            ? new List<Tutor>(Tutors)
            : new List<Tutor>(Tutors.Where(filter));
    }

    /// Updates an existing tutor in the data storage.
    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
