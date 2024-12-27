using DalApi;
using DO;
namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// Creates a new assignment and adds it to the data source.
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    /// Deletes an assignment by its ID from the data source.
    /// Throws an exception if the assignment does not exist.
    public void Delete(int id)
    {
        Assignment assignment = Read(id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        DataSource.Assignments.Remove(assignment);
    }

    /// Deletes all assignments from the data source.
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// Reads an assignment by its ID.
    /// Returns null if the assignment does not exist.
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(x => x.Id == id);
    }

    /// Reads an assignment based on a filter.
    /// Returns null if no matching assignment is found.
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return (DataSource.Assignments).FirstOrDefault(filter);
    }

    /// Reads all assignments, optionally filtered by a predicate.
    /// If no filter is provided, all assignments are returned.
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        if (filter != null)
        {
            return from item in DataSource.Assignments
                   where filter(item)
                   select item;
        }
        else
        {
            return from item in DataSource.Assignments
                   select item;
        }
    }

    /// Updates an existing assignment in the data source.
    /// Throws an exception if the assignment does not exist.
    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new DalDoesNotExistException($"An object of type assignment with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
