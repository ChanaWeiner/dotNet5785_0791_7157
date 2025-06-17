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
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;

        #region Stage 5
        public void AddObserver(Action listObserver) =>
        StudentCallManager.Observers.AddListObserver(listObserver); //stage 5
        public void AddObserver(int id, Action observer) =>
    StudentCallManager.Observers.AddObserver(id, observer); //stage 5
        public void RemoveObserver(Action listObserver) =>
    StudentCallManager.Observers.RemoveListObserver(listObserver); //stage 5
        public void RemoveObserver(int id, Action observer) =>
    StudentCallManager.Observers.RemoveObserver(id, observer); //stage 5
        #endregion Stage 5


        public void AssignCallToTutor(int tutorId, int callId)
        {
            //בדיקה האם למורה יש כבר קריאה בטיפול
            var existingAssignments = _dal.Assignment.ReadAll(a => a.TutorId == tutorId && a.EndTime == null);
            if (existingAssignments.Any())
            {
                throw new BO.BlCanNotAssignCall($"Tutor with ID={tutorId} already has an open call in treatment.");
            }
            // Check if the call has already been handled or expired.
            var callAssignments = _dal.Assignment.ReadAll(a =>
        a.StudentCallId == callId &&
        (a.EndOfTreatment == DO.EndOfTreatment.Treated || a.EndOfTreatment == DO.EndOfTreatment.Expired));

            if (callAssignments.Any())
                throw new BO.BlCanNotAssignCall($"Call with ID={callId} has already been handled or has expired.");

            // Create a new assignment for the tutor.
            DO.Assignment newAssignment = new(0, callId, tutorId, AdminManager.Now, null, null);
            _dal.Assignment.Create(newAssignment);
            StudentCallManager.Observers.NotifyListUpdated(); //stage 5
            TutorManager.Observers.NotifyItemUpdated(tutorId);
        }

        public void Create(BO.StudentCall call)
        {
            call.OpenTime = AdminManager.Now;
            try
            {
                // Validate the student call data.
                StudentCallManager.Validation(ref call);
            }
            catch (BO.BlValidationException)
            {
                throw; // Rethrow the exception if validation fails.
            }

            // Map BO object to DO object for the database.
            DO.StudentCall studentCall = new(call.Id, (DO.Subjects)call.Subject, call.Description, call.FullAddress, call.FullName, call.CellNumber, call.Email, call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);

            try
            {
                // Attempt to create the student call in the database.
                _dal.StudentCall.Create(studentCall);
                StudentCallManager.Observers.NotifyListUpdated(); //stage 5                                                    

            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw ex; // Rethrow the exception if the call already exists.
            }
            StudentCallManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }


        public void Delete(int callId)
        {
            // Retrieve the student call from the database.
            var doCall = _dal.StudentCall.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");

            // Check if there are assignments for the call and if it can be deleted.
            var callAssignments = _dal.Assignment.ReadAll(a => a.StudentCallId == callId);

            var statusCall = StudentCallManager.CalculateCallStatus(doCall);
            // Check if the call is open before deleting.
            if (statusCall != BO.CallStatus.Open && statusCall != BO.CallStatus.OpenInRisk)
                throw new BO.BlCanNotBeDeletedException("Cannot delete the call because it is not open.");

            if (callAssignments.Count() > 0)
                throw new BO.BlCanNotBeDeletedException("Cannot delete the call because there are assignments related to it.");

            // Attempt to delete the student call from the database.
            try
            {
                _dal.StudentCall.Delete(callId);
                StudentCallManager.Observers.NotifyListUpdated(); //stage 5                                                    
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw ex; // Rethrow the exception if the call does not exist.
            }
        }

        public int[] GetCallsByStatus()
        {
            // Retrieve all student calls from the database.
            var calls = _dal.StudentCall.ReadAll();

            // Group the calls by their subject and count them.
            int[] subjectCountsArray = calls
             .GroupBy(c => c.Subject)
             .Select(g => g.Count())
             .ToArray();

            return subjectCountsArray;
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCallsForTutor(int tutorId, Func<BO.ClosedCallInList, bool>? predicate = null)
        {
            // Retrieve all closed calls for the tutor from the assignments table.  
            var closedCalls = _dal.Assignment.ReadAll(a => a.TutorId == tutorId && a.EndOfTreatment!=null)
                .Join(_dal.StudentCall.ReadAll(),
                a => a.StudentCallId, c => c.Id,
                (a, c) => new BO.ClosedCallInList
                {
                    Id = c.Id,
                    Subject = (BO.Subjects)c.Subject,
                    FullAddress = c.FullAddress,
                    OpeningTime = c.OpenTime,
                    AssignmentTime = a.EntryTime,
                    ActualEndTime = a.EndTime,
                    EndType = a.EndOfTreatment!=null ? (BO.EndOfTreatment)a.EndOfTreatment : null
                });

            // Apply the predicate filter if specified.  
            if (predicate != null)
            {
                closedCalls = closedCalls.Where(predicate);
            }

            return closedCalls;
        }

        public IEnumerable<BO.OpenCallInList> GetOpenCallsForTutor(int tutorId, Func<BO.OpenCallInList, bool> predicate = null)
        {
            var tutor = _dal.Tutor.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");

            // Retrieve all open or open-in-risk calls for the tutor.  
            var openCalls = _dal.StudentCall.ReadAll(c => StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.Open ||
                StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.OpenInRisk)
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

            // Apply predicate filter if specified.  
            if (predicate != null)
                openCalls = openCalls.Where(predicate);

            openCalls = openCalls.Where(c => c.DistanceFromTutor <= tutor.Distance); // Filter out calls that are too far away.  

            return openCalls;
        }

        public BO.StudentCall Read(int callId)
        {
            // Retrieve the student call from the database.
            var doStudentCall = _dal.StudentCall.Read(callId) ?? throw new BO.BlDoesNotExistException($"Call with ID={callId} does not exist");

            // Retrieve the assignments for the student call.
            var doAssignments = _dal.Assignment.ReadAll((DO.Assignment a) => a.StudentCallId == callId);

            // Map the assignments to BO objects.
            var firstAssignment = doAssignments.FirstOrDefault();
            List<CallAssignInList> CallsAssignInList = doAssignments.Select((DO.Assignment a) => new BO.CallAssignInList()
            {
                TutorId = a.TutorId,
                TutorName = _dal.Tutor.Read((DO.Tutor t) => t.Id == a.TutorId)!.FullName,
                AssignmentTime = (DateTime)a.EntryTime!,
                ActualEndTime = a.EndTime,
                EndType = a.EndOfTreatment.HasValue ? (BO.EndOfTreatment)a.EndOfTreatment : null
            }).ToList();

            // Return the full student call details, including the assignments.
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
                Status = StudentCallManager.CalculateCallStatus(doStudentCall),
                CallsAssignInList = CallsAssignInList
            };
        }

        public void Update(BO.StudentCall call)
        {
            try
            {
                // Validate the updated student call data.
                StudentCallManager.Validation(ref call);
            }
            catch (BO.BlValidationException)
            {
                throw; // Rethrow the exception if validation fails.
            }

            // Map BO object to DO object for the database update.
            DO.StudentCall studentCall = new(call.Id, (DO.Subjects)call.Subject, call.Description, call.FullAddress, call.FullName, call.CellNumber, call.Email, call.Latitude, call.Longitude, call.OpenTime, call.FinalTime);

            try
            {
                // Attempt to update the student call in the database.
                _dal.StudentCall.Update(studentCall);
                StudentCallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5
                StudentCallManager.Observers.NotifyListUpdated();  //stage 5
            }
            catch (DO.DalDoesNotExistException)
            {
                throw; // Rethrow the exception if the call does not exist.
            }
            StudentCallManager.Observers.NotifyListUpdated(); //stage 5                                                    
            StudentCallManager.Observers.NotifyItemUpdated(call.Id);  //stage 5

        }

        public void UpdateTreatmentCancellation(int assignmentId, int tutorId)
        {
            DO.Assignment? assignment = null;


            // Retrieve the assignment from the database.
            assignment = _dal.Assignment.Read(assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment's tutor with ID={tutorId}, which its ID={assignmentId} does not exist");
            // Ensure the tutor has permission to cancel the treatment.
            if (assignment.TutorId != tutorId && !Tools.IsManagerId(tutorId))
                throw new BlCanNotUpdateTreatment("Tutor cannot cancel treatment for this assignment.");

            // Ensure the assignment has not already been completed or canceled.
            if (assignment!.EndTime != null)
                throw new BlCanNotUpdateTreatment("Assignment treatment has already been completed, canceled or expired.");

            // Update the assignment with cancellation details.
            DO.Assignment updateAssignment = assignment with
            {
                EndOfTreatment = assignment.TutorId == tutorId ? DO.EndOfTreatment.SelfCancel : DO.EndOfTreatment.ManagerCancel,
                EndTime = AdminManager.Now
            };

            try
            {
                // Attempt to update the assignment in the database.
                _dal.Assignment.Update(updateAssignment);
                //StudentCallManager.Observers.NotifyItemUpdated(assignmentId); //stage 5
                TutorManager.Observers.NotifyItemUpdated(tutorId);
                StudentCallManager.Observers.NotifyListUpdated();

            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw ex; // Rethrow the exception if the assignment does not exist.
            }
        }

        public void UpdateTreatmentCompletion(int tutorId, int assignmentId)
        {
            DO.Assignment? assignment = null;

            // Retrieve the assignment from the database.
            assignment = _dal.Assignment.Read(a => a.TutorId == tutorId && a.Id == assignmentId) ?? throw new BO.BlDoesNotExistException($"Assignment's tutor with ID={tutorId}, which its ID={assignmentId} does not exist");

            // Ensure the assignment has not already been completed or canceled.
            if (assignment!.EndTime != null)
                throw new BlCanNotUpdateTreatment("Assignment treatment has already been completed or canceled.");

            // Update the assignment with treatment completion details.
            DO.Assignment updateAssignment = assignment with
            {
                EndOfTreatment = (DO.EndOfTreatment)BO.EndOfTreatment.Treated,
                EndTime = AdminManager.Now
            };

            try
            {
                // Attempt to update the assignment in the database.
                _dal.Assignment.Update(updateAssignment);
                //StudentCallManager.Observers.NotifyItemUpdated(assignmentId); //stage 5
                TutorManager.Observers.NotifyItemUpdated(tutorId);
                StudentCallManager.Observers.NotifyListUpdated();
            }
            catch (DO.DalDoesNotExistException ex)
            {
                throw ex; // Rethrow the exception if the assignment does not exist.
            }
        }

        public IEnumerable<BO.CallInList> SortCallsInList(BO.StudentCallField? sortField = BO.StudentCallField.Id)
        {
            // Retrieve tutors from the DAL based on the active status filter.
            IEnumerable<DO.StudentCall> doTutor = _dal.StudentCall.ReadAll();

            // Convert the retrieved tutors from DAL objects to BO objects.
            IEnumerable<BO.CallInList> callsInList = doTutor.Select(StudentCallManager.ConvertFromDoToBo).ToList();

            return callsInList.OrderBy(item =>
                item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
        }

        public IEnumerable<BO.CallInList> FilterCallsInList(BO.StudentCallField? filterField = null, object? filterValue = null)
        {
            var doCalls = _dal.StudentCall.ReadAll();
            IEnumerable<BO.CallInList> callsInList = doCalls.Select(StudentCallManager.ConvertFromDoToBo);
            if (filterValue != null)
                callsInList = callsInList.Where(call =>
                {
                    var prop = call.GetType().GetProperty(filterField.ToString());
                    var val = prop?.GetValue(call);

                    Console.WriteLine($"Checking: {val} == {filterValue} → {val?.ToString() == filterValue?.ToString()}");

                    return val?.ToString() == filterValue?.ToString();
                }).ToList();
            return callsInList;
        }

        public IEnumerable<BO.OpenCallInList> SortOpenCalls(int tutorId, BO.OpenCallField? sortField = BO.OpenCallField.Id)
        {
            // Retrieve tutors from the DAL based on the active status filter.
            var tutor = _dal.Tutor.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");

            // Retrieve all open or open-in-risk calls for the tutor.  
            var openCalls = _dal.StudentCall.ReadAll(c => StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.Open ||
                StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.OpenInRisk)
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
            return openCalls.OrderBy(item =>
                item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
        }

        public IEnumerable<BO.OpenCallInList> FilterOpenCalls(int tutorId,BO.OpenCallField? filterField = null, object? filterValue = null)
        {
            var tutor = _dal.Tutor.Read(tutorId) ?? throw new BO.BlDoesNotExistException($"Tutor with ID={tutorId} does not exist");

            // Retrieve all open or open-in-risk calls for the tutor.  
            var openCalls = _dal.StudentCall.ReadAll(c => StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.Open ||
                StudentCallManager.CalculateCallStatus(c) == BO.CallStatus.OpenInRisk)
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
                openCalls = openCalls.Where(call =>
                {
                    var prop = call.GetType().GetProperty(filterField.ToString());
                    var val = prop?.GetValue(call);

                    Console.WriteLine($"Checking: {val} == {filterValue} → {val?.ToString() == filterValue?.ToString()}");

                    return val?.ToString() == filterValue?.ToString();
                }).ToList();
            return openCalls;
        }
        //אני רוצה פונקציה שתבדוק האם לקריאה מסויימת יש הקצאות
        public bool hasAssignments(int callId)=> _dal.Assignment.ReadAll(a => a.StudentCallId == callId).Any();
    }
}
