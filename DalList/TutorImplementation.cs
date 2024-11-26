using DalApi;
using DO;
using System.ComponentModel;

namespace Dal;

internal class TutorImplementation : ITutor
{
    public void Create(Tutor item)
    {
        if (Read(item.Id) is not null)
            throw new Exception($"Tutor with ID={item.Id} already exists");
        DataSource.Tutors.Add(item);
    }

    public void Delete(int id)
    {
        Tutor tutor = Read(id);
        if (tutor == null)
            throw new Exception($"Tutor with ID={id} is not exists");
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

    public List<Tutor> ReadAll()
    {
        return new List<Tutor>(DataSource.Tutors);
    }



    public void Update(Tutor item)
    {
        Tutor tutor = Read(item.Id);
        if (tutor == null)
            throw new Exception($"An object of type Tutor with such an {item.Id} does not exist");
        Delete(item.Id);
        Create(item);
    }
}
