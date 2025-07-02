using BO;
using System.Threading.Tasks;

namespace BlApi;

public interface IStudentCall: IObservable
{
    /// <summary>
    /// Retrieves the number of calls by their status (grouped by subject).
    /// </summary>
    /// <returns>An array of integers representing the number of calls per subject.</returns>
    public IEnumerable<object> GetCallStatusSummaries();
    /// <summary>
    /// Retrieves detailed information about a specific student call.
    /// </summary>
    /// <param name="callId">The ID of the student call to retrieve.</param>
    /// <returns>The student call object with its details and assignments.</returns>
    public BO.StudentCall Read(int callId);
    /// <summary>
    /// Updates the details of an existing student call.
    /// </summary>
    /// <param name="call">The updated student call object.</param>
    
    public void Update(BO.StudentCall call);

    /// <summary>
    /// Creates a new student call and validates it before storing.
    /// </summary>
    /// <param name="call">The student call object to be created.</param>
    public void Delete(int tutorId, int callId);

    /// <summary>
    /// Creates a new student call and validates it before storing.
    /// </summary>
    /// <param name="call">The student call object to be created.</param>
    public void Create(BO.StudentCall call);

    /// <summary>
    /// Retrieves a list of closed calls for a specific tutor, with optional filtering and sorting.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve the closed calls for.</param>
    /// <param name="subjectFilter">An optional filter by subject.</param>
    /// <param name="sortField">An optional field to sort the closed calls by.</param>
    /// <returns>A list of closed calls for the tutor.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, Func<BO.ClosedCallInList, bool> predicate=null);

    /// <summary>
    /// Retrieves a list of open calls for a specific tutor, with optional filtering and sorting.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve the open calls for.</param>
    /// <param name="subjectFilter">An optional filter by subject.</param>
    /// <param name="sortField">An optional field to sort the open calls by.</param>
    /// <returns>A list of open calls for the tutor.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, Func<BO.OpenCallInList, bool> predicate = null);

    /// <summary>
    /// Updates the treatment completion status of an assignment.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor completing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void UpdateTreatmentCompletion(int tutorId, int assignmentId);

    /// <summary>
    /// Updates the treatment cancellation status of an assignment.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor who is canceling the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void UpdateTreatmentCancellation(int assignmentId, int tutorId = 0);

    /// <summary>
    /// Updates the treatment completion status of an assignment.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor completing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void AssignCallToTutor(int tutorId, int callId);

    public IEnumerable<BO.CallInList> FilterCallsInList(BO.StudentCallField? filterField = null, object? filterValue = null);
    public IEnumerable<BO.CallInList> SortCallsInList(BO.StudentCallField? sortField = BO.StudentCallField.Id);

    public IEnumerable<BO.OpenCallInList> FilterOpenCalls(int tutorId, BO.OpenCallField? filterField = null, object? filterValue = null);
    public IEnumerable<BO.OpenCallInList> SortOpenCalls(int tutorId, BO.OpenCallField? sortField = BO.OpenCallField.Id);

    /// <summary>
    /// Check if there are assignments for specific call
    /// </summary>
    /// <param name="callId"></param>
    /// <returns></returns>
    public bool hasAssignments(int callId);
    /// <summary>
    /// Updates the coordinates of the specified student call.
    /// </summary>
    /// <param name="currentStudentCall">The student call object containing the updated coordinates.  This parameter cannot be null.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public void UpdateCoordinates(StudentCall currentStudentCall);
}

