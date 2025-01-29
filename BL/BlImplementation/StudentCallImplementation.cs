using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BlImplementation;
internal class StudentCallImplementation : BlApi.IStudentCall
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AssignCallToTutor(int tutorId, int callId)
    {
        throw new NotImplementedException();
    }

    public void Create(BO.StudentCall call)
    {
        try
        {
            StudentCallManager.Validation(call);
        }
        catch (Exception e)
        {
            throw e;
        }
        DO.StudentCall studentCall = new(call.Id, (DO.Subjects)call.Subject, call.Description, call.FullAddress, call.FullName, "", "", call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);
        try
        {
            _dal.StudentCall.Create(studentCall);
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    public void Delete(int callId)
    {
        var doCall = _dal.StudentCall.Read(callId);
        if (doCall != null)
        {
            throw new Exception();
        }
        var callAssignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.StudentCallId == callId);
        if (StudentCallManager.CalculateCallStatus(doCall,,) != BO.CallStatus.Open)
            throw new Exception();
        if (callAssignments.Count() > 0)
            throw new Exception();
        try
        {
            _dal.StudentCall.Delete(callId);
        }
        catch (Exception e)
        {
            throw e;
        }
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

    public IEnumerable<BO.CallInList> GetCallsList(BO.StudentCallField? filterField, object filterValue, BO.StudentCallField sortField = BO.StudentCallField.Id)
    {
        try
        {
            List<BO.CallInList> CallInList = StudentCallManager.GetCallInList(filterField, filterValue);
            return Tools.SortByField<BO.CallInList>(CallInList, sortField.ToString());
        }

        catch (Exception ex)
        {
            throw ex;
        }
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

        var doStudentCall = _dal.StudentCall.Read(callId) ?? throw new Exception();
        var doAssignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.StudentCallId == callId);
        var firstAssignment = doAssignments.FirstOrDefault();
        var maxCompletionTime = doStudentCall.OpenTime.AddHours(2); // לדוגמה, זמן מקסימלי להשלמת קריאה הוא 2 שעות
        List<CallAssignInList> CallsAssignInList = doAssignments.Select((DO.Assignment a) => new BO.CallAssignInList()
        {
            TutorId = a.TutorId,
            TutorName = _dal.Tutor.Read((DO.Tutor t) => t.Id == a.TutorId)!.FullName,
            AssignmentTime = (DateTime)a.EntryTime!,
            ActualEndTime = a.EndTime,
            EndType = (BO.EndOfTreatment)a.EndOfTreatment

        }).ToList();
        return new BO.StudentCall
        {
            Id = callId,
            Subject = (BO.Subjects)doStudentCall.Subject,
            Description = doStudentCall.Description,
            FullAddress = doStudentCall.FullAddress,
            FullName = doStudentCall.FullName,
            Latitude = doStudentCall.Latitude,
            Longitude = doStudentCall.Longitude,
            OpenTime = doStudentCall.OpenTime,
            FinalTime = doStudentCall.FinalTime,
            Status = StudentCallManager.CalculateCallStatus(doStudentCall, firstAssignment, maxCompletionTime),
            CallsAssignInList = CallsAssignInList

        };

    }

    public void Update(BO.StudentCall call)
    {
        try
        {
            StudentCallManager.Validation(call);
        }
        catch (Exception e)
        {
            throw e;
        }
        DO.StudentCall studentCall = new(call.Id, (DO.Subjects)call.Subject, call.Description, call.FullAddress, call.FullName, "", "", call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);
        try
        {
            _dal.StudentCall.Update(studentCall);
        }
        catch (Exception e)
        {
            throw e;
        }
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
