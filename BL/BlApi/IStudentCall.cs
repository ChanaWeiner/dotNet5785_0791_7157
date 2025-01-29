namespace BlApi;

public interface IStudentCall
{
    public int[] GetCallsByStatus();
    public IEnumerable<BO.CallInList> GetCallsList(BO.StudentCallField? filterField, object filterValue, BO.StudentCallField sortField = BO.StudentCallField.Id);
    public BO.StudentCall Read(int callId);
    public void Update(BO.StudentCall call);
    public void Delete(int callId);
    public void Create(BO.StudentCall call);
    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.ClosedCallField? sortField = BO.ClosedCallField.Id);
    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.OpenCallField? sortField = BO.OpenCallField.Id);
    public void UpdateTreatmentCompletion(int tutorId, int assignmentId);
    public void UpdateTreatmentCancellation(int tutorId, int assignmentId);
    public void AssignCallToTutor(int tutorId, int callId);

}

