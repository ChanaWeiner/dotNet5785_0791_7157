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
        DataSource.Tutors.Add(item);
    }

    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new DalDoesNotExistException($"Tutor with ID={id} is not exists");
        DataSource.Tutors.Remove(tutor);
    }

    public void DeleteAll()
    {
        DataSource.Tutors.Clear();

    }

    public Tutor? Read(int id)
    {
        return DataSource.Tutors.FirstOrDefault(x => x.Id == id);
    }

    public Tutor? Read(Func<Tutor, bool> filter)
    {
        return (DataSource.Tutors).FirstOrDefault(filter);
    }

    public IEnumerable<Tutor> ReadAll(Func<Tutor, bool>? filter = null)
    {
        return filter == null
            ? new List<Tutor>(DataSource.Tutors)
            : new List<Tutor>(DataSource.Tutors.Where(filter));
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
