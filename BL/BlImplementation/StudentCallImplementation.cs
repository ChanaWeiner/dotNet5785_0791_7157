using BlApi;

namespace BlImplementation;
internal class StudentCallImplementation : IStudentCall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AssignCallToTutor(int tutorId, int callId)
    {
        throw new NotImplementedException();
    }

    public void Create(BO.StudentCall call)
    {
        throw new NotImplementedException();
    }

    public void Delete(int callId)
    {
        throw new NotImplementedException();
    }

    public int[] GetCallsByStatus()
    {
        var calls = _dal.StudentCall.ReadAll();
        int[] subjectCountsArray = calls
     .GroupBy(c => c.Subject)
     .Select(g => g.Count())
     .ToArray();
        return subjectCountsArray;
    }

    public IEnumerable<BO.CallInList> GetCallsList(BO.StudentCallField? filterField, object filterValue, BO.StudentCallField sortField=BO.StudentCallField.Id)
    {


        throw new NotImplementedException();
    }

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(string tutorId, BO.Subjects? callType, BO.ClosedCallField? sortField)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(string tutorId, BO.Subjects? callType, BO.OpenCallField? sortField)
    {
        throw new NotImplementedException();
    }

    public BO.StudentCall Read(int callId)
    {
        throw new NotImplementedException();
    }

    public void Update(BO.StudentCall call)
    {
        throw new NotImplementedException();
    }

    public void UpdateTreatmentCancellation(int tutorId, int assignmentId)
    {
        throw new NotImplementedException();
    }

    public void UpdateTreatmentCompletion(int tutorId, int assignmentId)
    {
        throw new NotImplementedException();
    }
}
