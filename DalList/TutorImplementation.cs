using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class TutorImplementation : ITutor
{
    /// <summary>
    /// Creates a new tutor and adds it to the data source.
    /// Throws an exception if a tutor with the same ID already exists.
    /// </summary>
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Tutor with ID={item.Id} already exists");
        DataSource.Tutors.Add(item);
    }

    /// <summary>
    /// Deletes a tutor by its ID from the data source.
    /// Throws an exception if the tutor does not exist.
    /// </summary>
    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} is not exists");
        DataSource.Tutors.Remove(tutor);
    }

    /// <summary>
    /// Deletes all tutors from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Tutors.Clear();
    }

    /// <summary>
    /// Reads a tutor by its ID.
    /// Returns null if the tutor does not exist.
    /// </summary>
    public Tutor? Read(int id)
    {
        return DataSource.Tutors.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Reads a tutor based on a filter.
    /// Returns null if no matching tutor is found.
    /// </summary>
    public Tutor? Read(Func<Tutor, bool> filter)
    {
        return DataSource.Tutors.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all tutors, optionally filtered by a predicate.
    /// If no filter is provided, all tutors are returned.
    /// </summary>
    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        return filter == null
            ? new List<Tutor>(DataSource.Tutors)
            : new List<Tutor>(DataSource.Tutors.Where(filter));
    }

    /// <summary>
    /// Updates an existing tutor in the data source.
    /// Throws an exception if the tutor does not exist.
    /// </summary>
    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new DalDoesNotExistException($"An object of type Tutor with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
