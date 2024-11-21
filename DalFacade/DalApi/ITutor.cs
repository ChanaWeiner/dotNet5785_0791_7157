

using DO;

namespace DalApi;

public interface ITutor
{
    void Create(Tutor item); //Creates new entity object in DAL
    Tutor? Read(int id); //Reads entity object by its ID 
    List<Tutor> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(Tutor item); //Updates entity object
    void Delete(int id); //Deletes an object by its Id
    void DeleteAll(); //Delete all entity objects

}
