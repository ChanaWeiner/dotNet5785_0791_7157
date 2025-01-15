namespace BlApi;

public interface IStudentCall
{
    public int[] GetCallsByStatus();
    public IEnumerable<BO.CallInList> GetCallsList(BO.StudentCallField? filterField, object filterValue, BO.StudentCallField? sortField);
    public BO.StudentCall Read(int callId);
    public void Update(BO.StudentCall call);
    public void Delete(int callId);
    public void Create(BO.StudentCall call);
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(string tutorId, BO.Subjects? callType, BO.ClosedCallField? sortField);
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(string tutorId, BO.Subjects? callType, BO.OpenCallField? sortField);
    public void UpdateTreatmentCompletion(int tutorId, int assignmentId);
    public void UpdateTreatmentCancellation(int tutorId, int assignmentId);
    public void AssignCallToTutor(int tutorId, int callId);

}

