namespace BlApi;

public interface IStudentCall: IObservable
{
    
    public int[] GetCallsByStatus();
    /// <summary>
    /// Retrieves a list of open calls for a specific tutor, with optional filtering and sorting.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve the open calls for.</param>
    /// <param name="subjectFilter">An optional filter by subject.</param>
    /// <param name="sortField">An optional field to sort the open calls by.</param>
    /// <returns>A list of open calls for the tutor.</returns>
    public IEnumerable<BO.CallInList> GetCallsList(BO.StudentCallField? filterField, object? filterValue, BO.StudentCallField? sortField = BO.StudentCallField.Id);
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
    
    public void Delete(int callId);
    
    public void Create(BO.StudentCall call);
    /// <summary>
    /// Retrieves a list of closed calls for a specific tutor, with optional filtering and sorting.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve the closed calls for.</param>
    /// <param name="subjectFilter">An optional filter by subject.</param>
    /// <param name="sortField">An optional field to sort the closed calls by.</param>
    /// <returns>A list of closed calls for the tutor.</returns>
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.ClosedCallField? sortField = BO.ClosedCallField.Id);
    /// <summary>
    /// Retrieves a list of open calls for a specific tutor, with optional filtering and sorting.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor to retrieve the open calls for.</param>
    /// <param name="subjectFilter">An optional filter by subject.</param>
    /// <param name="sortField">An optional field to sort the open calls by.</param>
    /// <returns>A list of open calls for the tutor.</returns>
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.OpenCallField? sortField = BO.OpenCallField.Id);
    
    public void UpdateTreatmentCompletion(int tutorId, int assignmentId);
    /// <summary>
    /// Updates the treatment cancellation status of an assignment.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor who is canceling the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void UpdateTreatmentCancellation(int tutorId, int assignmentId);
    /// <summary>
    /// Updates the treatment completion status of an assignment.
    /// </summary>
    /// <param name="tutorId">The ID of the tutor completing the treatment.</param>
    /// <param name="assignmentId">The ID of the assignment to be updated.</param>
    public void AssignCallToTutor(int tutorId, int callId);
    public List<BO.CallInList> FilterCallsInList(BO.StudentCallField? callField = null, object? filterValue = null);


}

