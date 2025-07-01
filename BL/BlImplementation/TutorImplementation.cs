using Helpers;

internal class TutorImplementation : BlApi.ITutor
{
    #region Stage 5
    public void AddObserver(Action listObserver) =>
        TutorManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) =>
        TutorManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) =>
        TutorManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) =>
        TutorManager.Observers.RemoveObserver(id, observer);
    #endregion

    public void Create(BO.Tutor boTutor)
    {
        AdminManager.ThrowOnSimulatorIsRunning();
        try { TutorManager.Validation(ref boTutor); }
        catch (BO.BlValidationException error) { throw error; }

        DO.Tutor doTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email,
                         TutorManager.HashPassword(boTutor.Password), boTutor.CurrentAddress,
                         boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
                         boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            TutorManager.Create(doTutor);
            TutorManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BlAlreadyExistsException($"Tutor with ID={doTutor.Id} already exists", ex);
        }
    }

    public void Delete(int id)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        if (Tools.ReadAllAssignments(a => a.TutorId == id).Any())
        {
            throw new BO.BlCanNotBeDeletedException($"Tutor with ID={id} has assignment/s, they can't be deleted");
        }

        try
        {
            TutorManager.Delete(id);
            TutorManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist", ex);
        }
    }

    public BO.Role LogIn(int id, string password)
    {
        DO.Tutor doTutor = TutorManager.ReadAll(t => t.Id == id).FirstOrDefault()
            ?? throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist");

        if (!TutorManager.VerifyPassword(password, doTutor.Password))
            throw new BO.BlValidationException("Password isn't correct");

        return (BO.Role)doTutor.Role;
    }

    public BO.Tutor Read(int id)
    {
        DO.Tutor doTutor = TutorManager.Read(id)
            ?? throw new BO.BlDoesNotExistException($"Tutor with ID={id} does not exist");

        DO.Assignment doAssignment = Tools.ReadAssignment(a => a.TutorId == id && a.EndTime == null);

        BO.CallStatus? status = null;
        double distance = 0.0;
        DO.StudentCall doStudentCall = null;

        if (doAssignment != null)
        {
            doStudentCall = StudentCallManager.Read(doAssignment.StudentCallId);
            status = StudentCallManager.CalculateCallStatus(doStudentCall);
            distance = Tools.CalculateDistance(id, doStudentCall.Latitude, doStudentCall.Longitude);
        }

        bool hasCallInProgress = status is BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk;

        int totalCallsSelfCanceled = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.SelfCancel);
        int totalCallsHandled = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.Treated);
        int totalCallsExpired = TutorManager.CountCallsByEndStatus(id, BO.EndOfTreatment.Expired);

        return new BO.Tutor()
        {
            Id = id,
            FullName = doTutor.FullName,
            CellNumber = doTutor.CellNumber,
            Email = doTutor.Email,
            Password = doTutor.Password,
            CurrentAddress = doTutor.CurrentAddress,
            Latitude = doTutor.Latitude,
            Longitude = doTutor.Longitude,
            Role = (BO.Role)doTutor.Role,
            Active = doTutor.Active,
            Distance = doTutor.Distance,
            DistanceType = (BO.DistanceType)doTutor.DistanceType,
            TotalCallsSelfCanceled = totalCallsSelfCanceled,
            TotalCallsHandled = totalCallsHandled,
            TotalCallsExpired = totalCallsExpired,
            CurrentCallInProgress = hasCallInProgress ? new BO.CallInProgress()
            {
                Id = doAssignment.Id,
                CallId = doAssignment.StudentCallId,
                Subject = (BO.Subjects)doStudentCall.Subject,
                Description = doStudentCall.Description,
                FullAddress = doStudentCall.FullAddress,
                OpenTime = doStudentCall.OpenTime,
                MaxEndTime = doStudentCall.FinalTime,
                EntryTime = doAssignment!.EntryTime,
                Distance = distance,
                Status = (BO.CallStatus)status!
            } : null
        };
    }

    public IEnumerable<BO.TutorInList> SortTutorsInList(BO.TutorField? sortField = BO.TutorField.Id)
    {
        var doTutors = TutorManager.ReadAll();
        var tutorsInList = doTutors.Select(TutorManager.ConvertFromDoToBo).ToList();

        return tutorsInList.OrderBy(item =>
            item.GetType().GetProperty(sortField.ToString())?.GetValue(item));
    }

    public void Update(int id, BO.Tutor boTutor)
    {
        AdminManager.ThrowOnSimulatorIsRunning();

        var oldDoTutor = TutorManager.Read(boTutor.Id)
            ?? throw new BO.BlDoesNotExistException($"Tutor with ID={boTutor.Id} does not exist");
        bool isManager = Tools.IsManagerId(id);

        if (oldDoTutor.Role != (DO.Role)boTutor.Role)
        {
            if (!isManager)
                throw new BO.BlValidationException("You are not authorized to change the tutor's role.");
            if (oldDoTutor.Role == DO.Role.Manager)
            {
                int countOfManagers = TutorManager.ReadAll(tutor=>tutor.Active==true&&tutor.Role==(DO.Role) BO.Role.Manager).Count();
                if (countOfManagers == 1) {
                    throw new BO.BlValidationException("You can not change the tutor's role, he is the single manager.");
                }
            }
        }
        if(Tools.IsManagerId(boTutor.Id)&&boTutor.Active==false&& oldDoTutor.Active == true)
        {
            int countOfManagers = TutorManager.ReadAll(tutor => tutor.Active == true && tutor.Role == (DO.Role)BO.Role.Manager).Count();
            if (countOfManagers == 1)
            {
                throw new BO.BlValidationException("You can not change the manager to be not active, he is the single manager.");
            }
        }

        if (id != boTutor.Id && !isManager)
            throw new BO.BlValidationException("You are not authorized to update the tutor.");
        if (boTutor.CurrentCallInProgress != null && !boTutor.Active)
            throw new BO.BlValidationException("You cannot set the tutor to inactive while there is a call in progress.");

        TutorManager.Validation(ref boTutor);

        DO.Tutor newDoTutor = new(boTutor.Id, boTutor.FullName, boTutor.CellNumber, boTutor.Email, boTutor.Password,
            boTutor.CurrentAddress, boTutor.Latitude, boTutor.Longitude, (DO.Role)boTutor.Role,
            boTutor.Active, boTutor.Distance, (DO.DistanceType)boTutor.DistanceType);

        try
        {
            TutorManager.Update(newDoTutor);
            TutorManager.Observers.NotifyItemUpdated(boTutor.Id);
            TutorManager.Observers.NotifyListUpdated();
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDoesNotExistException($"Tutor with ID={boTutor.Id} does not exist", ex);
        }
    }

    public IEnumerable<BO.TutorInList> FilterTutorsInList(BO.TutorField? tutorField = null, object? filterValue = null)
    {
        var doTutors = TutorManager.ReadAll();
        var tutorsInList = doTutors.Select(TutorManager.ConvertFromDoToBo);

        if (filterValue != null)
            tutorsInList = tutorsInList.Where(tutor =>
                tutor.GetType().GetProperty(tutorField.ToString()).GetValue(tutor)?.ToString() == filterValue.ToString());

        return tutorsInList;
    }
}
