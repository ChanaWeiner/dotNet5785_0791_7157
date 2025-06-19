using DalApi;
using DO;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Dal;

internal class TutorImplementation : ITutor
{
    /// <summary>
    /// Creates a new tutor and adds it to the data storage.
    /// This method checks if the tutor already exists by ID, then adds the new tutor to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Tutor with ID={item.Id} already exists");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Add(item);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// <summary>
    /// Deletes a tutor by its ID.
    /// This method removes the tutor with the specified ID from the data storage.
    /// Throws an exception if the tutor is not found.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} does not exist");
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Remove(tutor);
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// <summary>
    /// Deletes all tutors from the data storage.
    /// This method clears all tutor records in the storage.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void DeleteAll()
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        Tutors.Clear();
        XMLTools.SaveListToXMLSerializer(Tutors, Config.s_tutors_xml);
    }

    /// <summary>
    /// Reads a specific tutor by its ID.
    /// This method retrieves the tutor with the specified ID from the data storage.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Tutor? Read(int id)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Reads a specific tutor based on a filter condition.
    /// This method retrieves a tutor that matches the provided filter condition.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public Tutor? Read(Func<Tutor, bool> filter)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return Tutors.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all tutors or filters them based on a provided condition.
    /// This method retrieves all tutors or applies the given filter to the list.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        List<Tutor> Tutors = XMLTools.LoadListFromXMLSerializer<Tutor>(Config.s_tutors_xml);
        return filter == null
            ? new List<Tutor>(Tutors)
            : new List<Tutor>(Tutors.Where(filter));
    }

    /// <summary>
    /// Updates an existing tutor in the data storage.
    /// This method deletes the existing tutor and creates a new one with updated data.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
