using DalApi;
using DO;

namespace Dal;

public class AssignmentImplementation : IAssignment
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
            throw new Exception($"Student call with ID={id} is not exists");
        DataSource.Assignments.Remove(assignment);
    }

    public void DeleteAll()
    {
        DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.Find(x => x.Id == id);
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
        Assignment assignment = Read(item.Id);
        if (assignment == null)
            throw new Exception($"An object of type assignment with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
