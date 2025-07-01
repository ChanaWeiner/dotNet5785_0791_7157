using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace BlImplementation
{
    internal class StudentCallImplementation : BlApi.IStudentCall
    {
        #region Stage 5
        public void AddObserver(Action listObserver) =>
            StudentCallManager.Observers.AddListObserver(listObserver);

        public void AddObserver(int id, Action observer) =>
            StudentCallManager.Observers.AddObserver(id, observer);

        public void RemoveObserver(Action listObserver) =>
            StudentCallManager.Observers.RemoveListObserver(listObserver);

        public void RemoveObserver(int id, Action observer) =>
            StudentCallManager.Observers.RemoveObserver(id, observer);
        #endregion

        public void AssignCallToTutor(int tutorId, int callId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            StudentCallManager.AssignCallToTutor(tutorId, callId);


        }

        public void Create(BO.StudentCall call)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            call.OpenTime = AdminManager.Now;
            StudentCallManager.Validation(ref call);

            var studentCall = new DO.StudentCall(call.Id, (DO.Subjects)call.Subject, call.Description,
                call.FullAddress, call.FullName, call.CellNumber, call.Email,
                call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);

            StudentCallManager.Create(studentCall);
            StudentCallManager.Observers.NotifyListUpdated();
        }

        public void Delete(int tutorId, int callId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            var doCall = StudentCallManager.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");
            var callAssignments = Tools.ReadAllAssignments(a => a.StudentCallId == callId);
            var statusCall = StudentCallManager.CalculateCallStatus(doCall);
            if (!Tools.IsManagerId(tutorId))
                throw new BO.BlAccessDeniedException("Only managers can delete calls.");
            if (statusCall != BO.CallStatus.Open && statusCall != BO.CallStatus.OpenInRisk)
                throw new BO.BlCanNotBeDeletedException("Cannot delete the call because it is not open.");

            if (callAssignments.Any())
                throw new BO.BlCanNotBeDeletedException("Cannot delete the call because there are assignments related to it.");

            StudentCallManager.Delete(callId);
            StudentCallManager.Observers.NotifyListUpdated();
        }

        public IEnumerable<object> GetCallStatusSummaries()
        {
            var calls = StudentCallManager.ReadAll();

            var grouped = calls
                .GroupBy(c => StudentCallManager.CalculateCallStatus(c))
                .ToDictionary(g => g.Key, g => g.Count());

            return Enum.GetValues(typeof(CallStatus))
                .Cast<CallStatus>()
                 .Where(c => c != BO.CallStatus.None)
                .Select(status => new
                {
                    Status = status,
                    Amount = grouped.ContainsKey(status) ? grouped[status] : 0
                })
                .ToList();
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, Func<BO.ClosedCallInList, bool>? predicate = null)
        {
            var closedCalls = Tools.ReadAllAssignments(a => a.TutorId == tutorId && a.EndOfTreatment != null)
                .Join(StudentCallManager.ReadAll(), a => a.StudentCallId, c => c.Id,
                    (a, c) => new BO.ClosedCallInList
                    {
                        Id = c.Id,
                        Subject = (BO.Subjects)c.Subject,
                        FullAddress = c.FullAddress,
                        OpeningTime = c.OpenTime,
                        AssignmentTime = a.EntryTime,
                        ActualEndTime = a.EndTime,
                        EndType = a.EndOfTreatment.HasValue ? (BO.EndOfTreatment)a.EndOfTreatment : null
                    });
            return predicate != null ? closedCalls.Where(predicate) : closedCalls;
        }

        public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, Func<BO.OpenCallInList, bool> predicate = null)
        {
            var tutor = TutorManager.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");
            var openCalls = StudentCallManager.ReadAll(c => StudentCallManager.CalculateCallStatus(c) is BO.CallStatus.Open or BO.CallStatus.OpenInRisk)
                .Select(c => new BO.OpenCallInList
                {
                    Id = c.Id,
                    Subject = (BO.Subjects)c.Subject,
                    Description = c.Description,
                    FullAddress = c.FullAddress,
                    OpeningTime = c.OpenTime,
                    MaxCompletionTime = c.FinalTime,
                    DistanceFromTutor = Tools.CalculateDistance(tutorId, c.Latitude, c.Longitude)
                });

            if (predicate != null)
                openCalls = openCalls.Where(predicate);

            return openCalls.Where(c => c.DistanceFromTutor <= tutor.Distance);
        }

        public BO.StudentCall Read(int callId)
        {
            var doStudentCall = StudentCallManager.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");
            var doAssignments = Tools.ReadAllAssignments(a => a.StudentCallId == callId);

            var callsAssignInList = doAssignments.Select(a => new BO.CallAssignInList
            {
                TutorId = a.TutorId,
                TutorName = TutorManager.Read(a.TutorId)?.FullName,
                AssignmentTime = a.EntryTime,
                ActualEndTime = a.EndTime,
                EndType = a.EndOfTreatment.HasValue ? (BO.EndOfTreatment)a.EndOfTreatment : null
            }).ToList();

            var status = StudentCallManager.CalculateCallStatus(doStudentCall);

            return new BO.StudentCall
            {
                Id = callId,
                Subject = (BO.Subjects)doStudentCall.Subject,
                Description = doStudentCall.Description,
                Email = doStudentCall.Email,
                CellNumber = doStudentCall.CellNumber,
                FullAddress = doStudentCall.FullAddress,
                FullName = doStudentCall.FullName,
                Latitude = doStudentCall.Latitude,
                Longitude = doStudentCall.Longitude,
                OpenTime = doStudentCall.OpenTime,
                FinalTime = doStudentCall.FinalTime,
                Status = status,
                CallsAssignInList = callsAssignInList
            };
        }

        public void Update(BO.StudentCall call)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            StudentCallManager.Validation(ref call);

            var studentCall = new DO.StudentCall(call.Id, (DO.Subjects)call.Subject, call.Description,
                call.FullAddress, call.FullName, call.CellNumber, call.Email,
                call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);

            StudentCallManager.Update(studentCall);
            StudentCallManager.Observers.NotifyItemUpdated(call.Id);
            StudentCallManager.Observers.NotifyListUpdated();
        }

        public void UpdateTreatmentCancellation(int assignmentId, int tutorId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            StudentCallManager.UpdateTreatmentCancellation(assignmentId, tutorId);
        }

        public void UpdateTreatmentCompletion(int tutorId, int assignmentId)
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            StudentCallManager.UpdateTreatmentCompletion(assignmentId, tutorId);

        }

        public IEnumerable<BO.CallInList> SortCallsInList(BO.StudentCallField? sortField = BO.StudentCallField.Id)
        {
            var doCalls = StudentCallManager.ReadAll();
            var callsInList = doCalls.Select(StudentCallManager.ConvertFromDoToBo);
            return callsInList.OrderBy(item => item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
        }

        public IEnumerable<BO.CallInList> FilterCallsInList(BO.StudentCallField? filterField = null, object? filterValue = null)
        {
            var doCalls = StudentCallManager.ReadAll();
            var callsInList = doCalls.Select(StudentCallManager.ConvertFromDoToBo);

            if (filterValue != null)
            {
                callsInList = callsInList.Where(call =>
                {
                    var prop = call.GetType().GetProperty(filterField.ToString());
                    var val = prop?.GetValue(call);
                    return val?.ToString() == filterValue?.ToString();
                });
            }

            return callsInList;
        }

        public IEnumerable<BO.OpenCallInList> SortOpenCalls(int tutorId, BO.OpenCallField? sortField = BO.OpenCallField.Id)
        {
            var tutor = TutorManager.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");
            var openCalls = StudentCallManager.ReadAll(c => StudentCallManager.CalculateCallStatus(c) is BO.CallStatus.Open or BO.CallStatus.OpenInRisk)
                .Select(c => new BO.OpenCallInList
                {
                    Id = c.Id,
                    Subject = (BO.Subjects)c.Subject,
                    Description = c.Description,
                    FullAddress = c.FullAddress,
                    OpeningTime = c.OpenTime,
                    MaxCompletionTime = c.FinalTime,
                    DistanceFromTutor = Tools.CalculateDistance(tutorId, c.Latitude, c.Longitude)
                });

            return openCalls.Where(c => c.DistanceFromTutor <= tutor.Distance).OrderBy(item => item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
        }

        public IEnumerable<BO.OpenCallInList> FilterOpenCalls(int tutorId, BO.OpenCallField? filterField = null, object? filterValue = null)
        {
            var tutor = TutorManager.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");
            var openCalls = StudentCallManager.ReadAll(c => StudentCallManager.CalculateCallStatus(c) is BO.CallStatus.Open or BO.CallStatus.OpenInRisk)
                .Select(c => new BO.OpenCallInList
                {
                    Id = c.Id,
                    Subject = (BO.Subjects)c.Subject,
                    Description = c.Description,
                    FullAddress = c.FullAddress,
                    OpeningTime = c.OpenTime,
                    MaxCompletionTime = c.FinalTime,
                    DistanceFromTutor = Tools.CalculateDistance(tutorId, c.Latitude, c.Longitude)
                });

            if (filterValue != null)
            {
                openCalls = openCalls.Where(call =>
                {

                    var prop = call.GetType().GetProperty(filterField.ToString());
                    var val = prop?.GetValue(call);
                    return val?.ToString() == filterValue?.ToString() && call.DistanceFromTutor<=tutor.Distance;
                });
            }

            return openCalls;
        }

        public bool hasAssignments(int callId) => Tools.ReadAllAssignments(a => a.StudentCallId == callId).Any();
    }
}
