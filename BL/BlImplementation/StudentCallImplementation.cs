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
        try
        {
            var callAssignments = _dal.Assignment.ReadAll(a => 
            a.StudentCallId == callId &&
            a.EndOfTreatment != DO.EndOfTreatment.Treated &&
            a.EndOfTreatment != DO.EndOfTreatment.Expired);

            if (callAssignments.Any())
                throw new Exception();
            DO.Assignment newAssignment = new(0,callId,tutorId,ClockManager.Now,null,null);
            _dal.Assignment.Create(newAssignment);
        }
        catch (Exception ex) {
            throw ex;
        }

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
        DO.StudentCall doCall;
        try
        {
            doCall = _dal.StudentCall.Read(callId);
        }
        catch(Exception e)
        {
            throw e;
        }
        var callAssignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.StudentCallId == callId);
        if (StudentCallManager.CalculateCallStatus(doCall) != BO.CallStatus.Open)
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

    public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.ClosedCallField? sortField=BO.ClosedCallField.Id)
    {
        var closedCalls = _dal.Assignment.ReadAll(a => a.TutorId == tutorId)
            .Join(_dal.StudentCall.ReadAll(c=>StudentCallManager.CalculateCallStatus(c)==BO.CallStatus.Closed),
            a => a.StudentCallId, c => c.Id,
            (a, c) => new BO.ClosedCallInList
            {
                Id = c.Id,
                Subject = (BO.Subjects)c.Subject,
                FullAddress = c.FullAddress,
                OpeningTime = c.OpenTime,
                AssignmentTime = a.EntryTime ?? c.OpenTime,
                ActualEndTime = a.EndTime,
                EndType = (BO.EndOfTreatment)a.EndOfTreatment
            });

        if (subjectFilter.HasValue)
        {
            closedCalls = closedCalls.Where(c => c.Subject == subjectFilter.Value).ToList();
        }

        Tools.SortByField(closedCalls.ToList(), sortField.ToString()!);

        return closedCalls;
    }

    public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, BO.Subjects? subjectFilter, BO.OpenCallField? sortField = BO.OpenCallField.Id)
    {
        var openCalls = _dal.StudentCall.ReadAll(c=>StudentCallManager.CalculateCallStatus(c)==BO.CallStatus.Open ||
        StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.OpenInRisk)
            .Select(c => new BO.OpenCallInList
            {
                Id = c.Id,
                Subject = (BO.Subjects)c.Subject,
                Description = c.Description,
                FullAddress = c.FullAddress,
                OpeningTime = c.OpenTime,
                MaxCompletionTime = c.FinalTime,
                DistanceFromVolunteer = Tools.CalculateDistance(tutorId, c.Latitude, c.Longitude)
            });

        if (subjectFilter.HasValue)
            openCalls = openCalls.Where(c => c.Subject == subjectFilter.Value);

        Tools.SortByField(openCalls.ToList(), sortField.ToString()!);

        return openCalls;
    }

    public BO.StudentCall Read(int callId)
    {

        var doStudentCall = _dal.StudentCall.Read(callId) ?? throw new Exception();
        var doAssignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.StudentCallId == callId);
        var firstAssignment = doAssignments.FirstOrDefault();
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
            Status = StudentCallManager.CalculateCallStatus(doStudentCall),
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
        DO.StudentCall studentCall = new(call.Id, (DO.Subjects)call.Subject, call.Description, call.FullAddress, call.FullName, call.CellNumber, call.Email, call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);
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
        DO.Assignment? assignment = null;
        try
        {
            assignment = _dal.Assignment.Read(assignmentId);
        }
        catch (Exception e)
        {
            throw e;
        }

        if (assignment.TutorId != tutorId && tutorId != managerId())
            throw new Exception();

        if (assignment!.EndOfTreatment!=null)
            throw new Exception();

        DO.Assignment updateAssignment = assignment with
        {
            EndOfTreatment = assignment.TutorId == tutorId?DO.EndOfTreatment.SelfCancel: DO.EndOfTreatment.ManagerCancel,
            EndTime = ClockManager.Now
        };

        try
        {
            _dal.Assignment.Update(updateAssignment);
        }

        catch (Exception e)
        {
            throw e;
        }
    }

    public void UpdateTreatmentCompletion(int tutorId, int assignmentId)
    {
        DO.Assignment? assignment=null;
        try
        {
           assignment = _dal.Assignment.Read(a => a.TutorId == tutorId);
        }
        catch (Exception e) {
            throw e;
        }
        if (assignment!.EndOfTreatment != null)
            throw new Exception();
        DO.Assignment updateAssignment= assignment with 
        { 
            EndOfTreatment=(DO.EndOfTreatment)BO.EndOfTreatment.Treated,
            EndTime=ClockManager.Now
        };

        try
        {
            _dal.Assignment.Update(updateAssignment);
        }
        catch (Exception e) 
        {
             throw e;
        }
    }
}
