using DalApi;
using DO;
namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Creates a new assignment and adds it to the data source.
    /// </summary>
    /// <param name="item">The assignment to add.</param>
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }

    /// <summary>
    /// Deletes an assignment by its ID from the data source.
    /// Throws an exception if the assignment does not exist.
    /// </summary>
    /// <param name="id">The ID of the assignment to delete.</param>
    public void Delete(int id)
    {
        Assignment assignment = Read(id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Student call with ID={id} does not exist");
        DataSource.Assignments.Remove(assignment);
    }

    /// <summary>
    /// Deletes all assignments from the data source.
    /// </summary>
    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    /// <summary>
    /// Reads an assignment by its ID.
    /// Returns null if the assignment does not exist.
    /// </summary>
    /// <param name="id">The ID of the assignment to read.</param>
    /// <returns>The assignment if found, otherwise null.</returns>
    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(x => x.Id == id);
    }

    /// <summary>
    /// Reads an assignment based on a filter.
    /// Returns null if no matching assignment is found.
    /// </summary>
    /// <param name="filter">A predicate function to filter assignments.</param>
    /// <returns>The first matching assignment if found, otherwise null.</returns>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all assignments, optionally filtered by a predicate.
    /// If no filter is provided, all assignments are returned.
    /// </summary>
    /// <param name="filter">An optional predicate to filter assignments.</param>
    /// <returns>An enumerable collection of assignments.</returns>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        return filter != null ? DataSource.Assignments.Where(filter) : DataSource.Assignments;
    }

    /// <summary>
    /// Updates an existing assignment in the data source.
    /// Throws an exception if the assignment does not exist.
    /// </summary>
    /// <param name="item">The assignment to update.</param>
    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new DalDoesNotExistException($"An object of type assignment with ID {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
