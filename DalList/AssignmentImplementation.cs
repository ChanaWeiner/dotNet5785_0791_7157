using DalApi;
using DO;

namespace Dal;

internal class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);

    }

    public void Delete(int id)
    {
        Assignment assignment = Read(id);
        if (assignment == null)
            throw new DalDoesNotExistException($"Student call with ID={id} is not exists");
        DataSource.Assignments.Remove(assignment);
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(x => x.Id == id);
    }
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return (DataSource.Assignments).FirstOrDefault(filter);
    }
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

    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new DalDoesNotExistException($"An object of type assignment with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
