

using BO;

namespace BlApi;

public interface ITutor: IObservable
{
    /// <summary>
    /// Allows a tutor to log in by verifying their credentials.
    /// </summary>
    /// <param name="id">The ID of the tutor attempting to log in.</param>
    /// <param name="password">The password entered by the tutor.</param>
    /// <returns>The role of the logged-in tutor.</returns>
    public BO.Role LogIn(int id,string password);

    /// <summary>
    /// Sorts the tutors list based on the specified criteria and returns the sorted list.
    /// </summary>
    /// <param name="isActive">The active status of the tutors to filter by. Can be null to ignore this filter.</param>
    /// <param name="sortField">The field to sort the tutors by.</param>
    /// <returns>A sorted list of tutors based on the given criteria.</returns>
    public IEnumerable<BO.TutorInList> SortTutorsInList(BO.TutorField? sortField);

    /// <summary>
    /// Retrieves detailed information about a specific tutor.
    /// </summary>
    /// <param name="id">The ID of the tutor to be retrieved.</param>
    /// <returns>A business object containing detailed tutor information.</returns>
    public BO.Tutor Read(int id);

    /// <summary>
    /// Updates the details of a tutor, ensuring that only authorized users can perform the update.
    /// </summary>
    /// <param name="id">The ID of the tutor to be updated.</param>
    /// <param name="boTutor">The business object containing the updated tutor information.</param>
    public void Update(int id, BO.Tutor tutor);

    /// <summary>
    /// Deletes a tutor from the system. Throws an exception if the tutor has assignments.
    /// </summary>
    /// <param name="id">The ID of the tutor to be deleted.</param>
    public void Delete(int id);

    /// <summary>
    /// Creates a new tutor in the system after validating the provided tutor object.
    /// </summary>
    /// <param name="boTutor">The business object representing the tutor to be created.</param>
    public void Create(BO.Tutor tutor);
    public IEnumerable<BO.TutorInList> FilterTutorsInList(BO.TutorField? tutorField = null, object? filterValue = null);

}
