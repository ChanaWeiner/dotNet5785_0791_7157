using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class TutorImplementation : ITutor
{
    /// Creates a new tutor and adds it to the data source.
    /// Throws an exception if a tutor with the same ID already exists.
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new DalAlreadyExistsException($"Tutor with ID={item.Id} already exists");
        DataSource.Tutors.Add(item);
    }

    /// Deletes a tutor by its ID from the data source.
    /// Throws an exception if the tutor does not exist.
    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} is not exists");
        DataSource.Tutors.Remove(tutor);
    }

    /// Deletes all tutors from the data source.
    public void DeleteAll()
    {
        DataSource.Tutors.Clear();
    }

    /// Reads a tutor by its ID.
    /// Returns null if the tutor does not exist.
    public Tutor? Read(int id)
    {
        return DataSource.Tutors.FirstOrDefault(x => x.Id == id);
    }

    /// Reads a tutor based on a filter.
    /// Returns null if no matching tutor is found.
    public Tutor? Read(Func<Tutor, bool> filter)
    {
        return DataSource.Tutors.FirstOrDefault(filter);
    }

    /// Reads all tutors, optionally filtered by a predicate.
    /// If no filter is provided, all tutors are returned.
    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        return filter == null
            ? new List<Tutor>(DataSource.Tutors)
            : new List<Tutor>(DataSource.Tutors.Where(filter));
    }

    /// Updates an existing tutor in the data source.
    /// Throws an exception if the tutor does not exist.
    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new DalDoesNotExistException($"An object of type Tutor with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
