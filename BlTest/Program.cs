using System;
using System.Linq;
using BlApi;
using BO;

class Program
{
    // Initialize the BL interface through the Factory class
    static readonly IBl s_bl = BlApi.Factory.Get();

    enum MainMenu { TutorManagement = 1, CallManagement, SystemManagement, Exit };
    enum CallMenuChoice
    {
        GetCallsByStatus = 1, GetCallsList, ReadCall, UpdateCall, DeleteCall, CreateCall,
        GetClosedCallsForTutor, GetOpenCallsForTutor, UpdateTreatmentCompletion,
        UpdateTreatmentCancellation, AssignCallToTutor, Exit
    };
    enum TutorMenuChoice { CreateTutor = 1, ReadTutor, UpdateTutor, DeleteTutor, SortTutors, LogIn, Exit }

    enum AdminMenuChoice { GetSystemClock = 1, AdvanceSystemClock, GetRiskTimeRange, SetRiskTimeRange, ResetDatabase, InitializeDatabase, Exit }
    static void Main(string[] args)
    {
        bool exit = false;
        while (!exit)
        {
            try
            {
                Console.WriteLine("Main Menu:");
                int i = 1;
                foreach (var option in Enum.GetValues(typeof(MainMenu)))
                    Console.WriteLine(i++ + ". " + option);
                Console.Write("Enter choice: ");

                MainMenu choice = (MainMenu)int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case MainMenu.TutorManagement:
                        TutorMenu(); break; // Go to Tutor Management Menu
                    case MainMenu.CallManagement:
                        StudentCallMenu(); break; // Go to Call Management Menu
                    case MainMenu.SystemManagement:
                        AdminMenu(); break; // Go to System Management Menu
                    case MainMenu.Exit:
                        exit = true; break; // Exit the application
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
            }
        }
    }

    // Tutor Management Menu
    private static void TutorMenu()
    {
        bool exit = false;
        while (!exit)
        {
            try
            {
                Console.WriteLine("Tutor Management Menu");
                int i = 1;
                foreach (var option in Enum.GetValues(typeof(TutorMenuChoice)))
                    Console.WriteLine(i++ + ". " + option);
                Console.Write("Enter choice: ");

                TutorMenuChoice choice = (TutorMenuChoice)int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case TutorMenuChoice.CreateTutor:
                        CreateTutor();
                        break;
                    case TutorMenuChoice.ReadTutor:
                        ReadEntity(id => s_bl.Tutor.Read(id));
                        break;
                    case TutorMenuChoice.UpdateTutor:
                        UpdateTutor();
                        break;
                    case TutorMenuChoice.DeleteTutor:
                        DeleteEntity<BO.Tutor>(id => s_bl.Tutor.Delete(id));
                        break;
                    case TutorMenuChoice.SortTutors:
                        SortTutors();
                        break;
                    case TutorMenuChoice.LogIn:
                        LogIn();
                        break;
                    case TutorMenuChoice.Exit:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private static void LogIn()
    {
        Console.Write("Enter Your ID: ");
        if (int.TryParse(Console.ReadLine(), out int tutorId))
        {
            Console.Write("Enter Your Password: ");
            string password = Console.ReadLine();
            try
            {
                var tutorRole = s_bl.Tutor.LogIn(tutorId, password);
                Console.WriteLine(tutorRole.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Tutor ID.");
        }
    }

    private static void SortTutors()
    {
        Console.Write("Sort filter: Is Active (true/false): ");
        if (!bool.TryParse(Console.ReadLine(), out bool isActive))
            Console.WriteLine("Invalid input");

        Console.WriteLine("Sort field: (Id, FullName, IsActive, TotalHandledCalls, TotalCancelledCalls, TotalExpiredCalls, CurrentCallId, CurrentCallType)");
        if (!Enum.TryParse(Console.ReadLine(), out TutorSortField sortField))
            Console.WriteLine("Invalid input");

        var tutorsInList = s_bl.Tutor.SortTutorsInList(isActive, sortField);
        Console.WriteLine(string.Join("\n-----\n", tutorsInList));
    }

    private static void CreateTutor()
    {
        Console.Write("Enter id: ");
        int.TryParse(Console.ReadLine(), out int id);

        Console.Write("Enter  name: ");
        string? name = Console.ReadLine();

        Console.Write("Enter  cell number: ");
        string cellNumber = Console.ReadLine();

        Console.Write("Enter  email: ");
        string email = Console.ReadLine();

        Console.Write("Enter  password: ");
        string? password = Console.ReadLine();

        Console.Write("Enter  current address: ");
        string? currentAddress = Console.ReadLine();

        //Console.Write("Enter latitude: ");
        //double.TryParse(Console.ReadLine(), out double latitude);

        //Console.Write("Enter longitude: ");
        //double.TryParse(Console.ReadLine(), out double longitude);

        Console.Write("Enter role (MasterTutor, BeginnerTutor, Manager): ");
        Enum.TryParse(Console.ReadLine(), out BO.Role role);

        Console.Write("Is Active (true/false): ");
        bool.TryParse(Console.ReadLine(), out bool active);

        Console.Write("Enter distance: ");
        double.TryParse(Console.ReadLine(), out double distance);

        Console.Write("Enter distance type (Air, Walking, Driving): ");
        Enum.TryParse(Console.ReadLine(), out DistanceType distanceType);

        var newTutor = new BO.Tutor
        {
            Id = id,
            FullName = name,
            CellNumber = cellNumber,
            Email = email,
            Password = password,
            CurrentAddress = currentAddress,
            //Latitude = latitude,
            //Longitude = longitude,
            Role = role,
            Active = active,
            Distance = distance,
            DistanceType = distanceType
        };

        s_bl.Tutor.Create(newTutor);
        Console.WriteLine("Tutor created successfully.");

    }

    // Read Tutor details
    private static void ReadEntity<T>(Func<int, T> readEntity)
    {
        Console.Write("Enter entity ID: ");
        if (int.TryParse(Console.ReadLine(), out int entityId))
        {
            T entity = readEntity(entityId);
            Console.WriteLine(entity);
        }
        else
        {
            Console.WriteLine("Invalid Entity ID.");
        }
    }

    // Update Tutor details
    private static void UpdateTutor()
    {
        Console.Write("Enter Tutor ID for update: ");
        if (int.TryParse(Console.ReadLine(), out int tutorId))
        {
            Console.Write("Enter new name: ");
            string? name = Console.ReadLine();

            Console.Write("Enter new cell number: ");
            string cellNumber = Console.ReadLine();

            Console.Write("Enter new email: ");
            string email = Console.ReadLine();

            Console.Write("Enter new password: ");
            string? password = Console.ReadLine();

            Console.Write("Enter new current address: ");
            string? currentAddress = Console.ReadLine();

            //Console.Write("Enter latitude: ");
            //double.TryParse(Console.ReadLine(), out double latitude);

            //Console.Write("Enter longitude: ");
            //double.TryParse(Console.ReadLine(), out double longitude);

            Console.Write("Enter role (MasterTutor, BeginnerTutor, Manager): ");
            Enum.TryParse(Console.ReadLine(), out Role role);

            Console.Write("Is Active (true/false): ");
            bool.TryParse(Console.ReadLine(), out bool active);

            Console.Write("Enter distance: ");
            double.TryParse(Console.ReadLine(), out double distance);

            Console.Write("Enter distance type (Air, Walking, Driving): ");
            Enum.TryParse(Console.ReadLine(), out DistanceType distanceType);

            var tutor = new BO.Tutor
            {
                Id = tutorId,
                FullName = name,
                CellNumber = cellNumber,
                Email = email,
                Password = password,
                CurrentAddress = currentAddress,
                //Latitude = latitude,
                //Longitude = longitude,
                Role = role,
                Active = active,
                Distance = distance,
                DistanceType = distanceType
            };

            s_bl.Tutor.Update(tutorId, tutor);
            Console.WriteLine("Tutor updated successfully.");
        }
        else
        {
            Console.WriteLine("Invalid Tutor ID.");
        }
    }

    // Delete Tutor details
    private static void DeleteEntity<T>(Action<int> delete)
    {
        Console.Write("Enter entity ID for deletion: ");
        if (int.TryParse(Console.ReadLine(), out int entityId))
        {
            delete(entityId);
            Console.WriteLine("Entity deleted successfully.");
        }
        else
        {
            Console.WriteLine("Invalid Entity ID.");
        }
    }

    // Call Management Menu
    private static void StudentCallMenu()
    {
        bool exit = false;
        while (!exit)
        {
            try
            {
                Console.WriteLine("Call Management Menu");
                int i = 1;
                foreach (var option in Enum.GetValues(typeof(CallMenuChoice)))
                    Console.WriteLine(i++ + ". " + option);
                Console.Write("Enter choice: ");

                if (!Enum.TryParse(Console.ReadLine(), true, out CallMenuChoice choice) && Enum.IsDefined(typeof(CallMenuChoice), choice))
                    Console.WriteLine("Invalid choice");

                switch (choice)
                {
                    case CallMenuChoice.GetCallsByStatus:
                        GetCallsByStatus();
                        break;
                    case CallMenuChoice.GetCallsList:
                        GetCallsList();
                        break;
                    case CallMenuChoice.ReadCall:
                        ReadEntity(id => s_bl.StudentCall.Read(id));
                        break;
                    case CallMenuChoice.UpdateCall:
                        UpdateCall();
                        break;
                    case CallMenuChoice.DeleteCall:
                        DeleteEntity<BO.StudentCall>(id => s_bl.StudentCall.Delete(id));
                        break;
                    case CallMenuChoice.CreateCall:
                        CreateCall();
                        break;
                    case CallMenuChoice.GetClosedCallsForTutor:
                        GetCallsForTutor<ClosedCallField, ClosedCallInList>((tutorId, filterField, sortField) => s_bl.StudentCall.GetClosedCallsForTutor(tutorId, filterField, sortField));
                        break;
                    case CallMenuChoice.GetOpenCallsForTutor:
                        GetCallsForTutor<OpenCallField, OpenCallInList>((tutorId, filterField, sortField) => s_bl.StudentCall.GetOpenCallsForTutor(tutorId, filterField, sortField));
                        break;
                    case CallMenuChoice.UpdateTreatmentCompletion:
                        UpdateTreatment((callId, assignmentId) => s_bl.StudentCall.UpdateTreatmentCompletion(callId, assignmentId));
                        break;
                    case CallMenuChoice.UpdateTreatmentCancellation:
                        UpdateTreatment((callId, assignmentId) => s_bl.StudentCall.UpdateTreatmentCancellation(callId, assignmentId));
                        break;
                    case CallMenuChoice.AssignCallToTutor:
                        AssignCallToTutor();
                        break;
                    case CallMenuChoice.Exit:
                        exit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    static void GetCallsByStatus()
    {
        int[] calls = s_bl.StudentCall.GetCallsByStatus();
        Console.WriteLine(string.Join(", ", calls));
    }

    static void GetCallsList()
    {
        Console.Write("Filter field: (0-Id, CallId, CallType, OpeningTime, RemainingTime, LastVolunteerName, CompletionTime, Status, TotalAssignments)");
        if (!Enum.TryParse(Console.ReadLine(), out StudentCallField filterField))
        {
            Console.WriteLine("Invalid input");
            return;
        }

        Console.Write("Filter value:");
        object filterValue = Console.ReadLine();

        Console.WriteLine("Sort field: (0-Id, CallId, CallType, OpeningTime, RemainingTime, LastVolunteerName, CompletionTime, Status, TotalAssignments");
        if (!Enum.TryParse(Console.ReadLine(), out StudentCallField sortField))
        {
            Console.WriteLine("Invalid input");
            return;
        }

        var calls = s_bl.StudentCall.GetCallsList(filterField, filterValue, sortField);
        foreach (var call in calls)
        {
            Console.WriteLine(call);
            Console.WriteLine("---------------");
        }
    }

    static void UpdateCall()
    {
        Console.Write("Enter Call ID for update: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.WriteLine("Invalid Call ID.");
            return;
        }

        Console.Write("Enter new Subject (e.g., Math, Science, etc.): ");
        string subjectInput = Console.ReadLine();
        if (!Enum.TryParse(subjectInput, true, out Subjects subject))
        {
            Console.WriteLine("Invalid subject.");
            return;
        }

        Console.Write("Enter new Description: ");
        string description = Console.ReadLine();

        Console.Write("Enter new Full Address: ");
        string fullAddress = Console.ReadLine();

        Console.Write("Enter new Full Name: ");
        string fullName = Console.ReadLine();

        Console.Write("Enter new Cell Number: ");
        string cellNumber = Console.ReadLine();

        Console.Write("Enter new Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter new Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
        {
            Console.WriteLine("Invalid latitude.");
            return;
        }

        Console.Write("Enter new Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
        {
            Console.WriteLine("Invalid longitude.");
            return;
        }

        Console.Write("Enter new Open Time (format: yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
        {
            Console.WriteLine("Invalid open time format.");
            return;
        }

        Console.Write("Enter new Final Time (format: yyyy-MM-dd HH:mm) or leave empty if not applicable: ");
        string finalTimeInput = Console.ReadLine();
        DateTime? finalTime = null;
        if (!string.IsNullOrEmpty(finalTimeInput))
        {
            if (DateTime.TryParse(finalTimeInput, out DateTime parsedFinalTime))
            {
                finalTime = parsedFinalTime;
            }
            else
            {
                Console.WriteLine("Invalid final time format.");
                return;
            }
        }

        Console.Write("Enter new Status (e.g., Open, Closed): ");
        string statusInput = Console.ReadLine();
        if (!Enum.TryParse(statusInput, true, out CallStatus status))
        {
            Console.WriteLine("Invalid status.");
            return;
        }

        var updatedCall = new BO.StudentCall
        {
            Id = callId,
            Subject = subject,
            Description = description,
            FullAddress = fullAddress,
            FullName = fullName,
            CellNumber = cellNumber,
            Email = email,
            Latitude = latitude,
            Longitude = longitude,
            OpenTime = openTime,
            FinalTime = finalTime,
            Status = status,
        };

        s_bl.StudentCall.Update(updatedCall);
        Console.WriteLine("Call updated successfully.");
    }

    static void CreateCall()
    {
        // קריאת הנתונים מהמשתמש עבור כל התכונות של הקריאה
        Console.Write("Enter Call ID: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.WriteLine("Invalid Call ID.");
            return;
        }

        Console.Write("Enter Subject (e.g., Math, Science, etc.): ");
        string subjectInput = Console.ReadLine();
        if (!Enum.TryParse(subjectInput, true, out Subjects subject))
        {
            Console.WriteLine("Invalid subject.");
            return;
        }

        Console.Write("Enter Description: ");
        string description = Console.ReadLine();

        Console.Write("Enter Full Address: ");
        string fullAddress = Console.ReadLine();

        Console.Write("Enter Full Name: ");
        string fullName = Console.ReadLine();

        Console.Write("Enter Cell Number: ");
        string cellNumber = Console.ReadLine();

        Console.Write("Enter Email: ");
        string email = Console.ReadLine();

        Console.Write("Enter Latitude: ");
        if (!double.TryParse(Console.ReadLine(), out double latitude))
        {
            Console.WriteLine("Invalid latitude.");
            return;
        }

        Console.Write("Enter Longitude: ");
        if (!double.TryParse(Console.ReadLine(), out double longitude))
        {
            Console.WriteLine("Invalid longitude.");
            return;
        }

        Console.Write("Enter Open Time (format: yyyy-MM-dd HH:mm): ");
        if (!DateTime.TryParse(Console.ReadLine(), out DateTime openTime))
        {
            Console.WriteLine("Invalid open time format.");
            return;
        }

        Console.Write("Enter Final Time (format: yyyy-MM-dd HH:mm) or leave empty if not applicable: ");
        string finalTimeInput = Console.ReadLine();
        DateTime? finalTime = null;
        if (!string.IsNullOrEmpty(finalTimeInput))
        {
            if (DateTime.TryParse(finalTimeInput, out DateTime parsedFinalTime))
            {
                finalTime = parsedFinalTime;
            }
            else
            {
                Console.WriteLine("Invalid final time format.");
                return;
            }
        }

        Console.Write("Enter Status (e.g., Open, Closed): ");
        string statusInput = Console.ReadLine();
        if (!Enum.TryParse(statusInput, true, out CallStatus status))
        {
            Console.WriteLine("Invalid status.");
            return;
        }

        var newCall = new BO.StudentCall
        {
            Id = callId,
            Subject = subject,
            Description = description,
            FullAddress = fullAddress,
            FullName = fullName,
            CellNumber = cellNumber,
            Email = email,
            Latitude = latitude,
            Longitude = longitude,
            OpenTime = openTime,
            FinalTime = finalTime,
            Status = status,
            CallsAssignInList = new List<BO.CallAssignInList>()
        };


        s_bl.StudentCall.Create(newCall);
        Console.WriteLine("Call created successfully.");
    }

    static void GetCallsForTutor<T, U>(Func<int, Subjects, T, IEnumerable<U>> getCalls) where T : struct, Enum
    {
        Console.Write("Enter Tutor ID: ");
        if (!int.TryParse(Console.ReadLine(), out int tutorId))
        {
            Console.WriteLine("Invalid Tutor ID");
            return;
        }

        Console.Write("Enter sort field: ");
        if (!Enum.TryParse(Console.ReadLine(), out T sortField))
        {
            Console.WriteLine("Invalid sort Field");
            return;
        }

        Console.Write("Enter a subject for filtering: ");
        if (!Enum.TryParse(Console.ReadLine(), out Subjects filterField))
        {
            Console.WriteLine("Invalid sort Field");
            return;
        }

        var calls = getCalls(tutorId, filterField, sortField);
        foreach (var call in calls)
        {
            Console.WriteLine(call);
            Console.WriteLine("---------------");
        }
    }

    static void UpdateTreatment(Action<int, int> updateTreatment)
    {
        Console.Write("Enter Call ID to update treatment: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.WriteLine("Invalid Call ID.");
            return;
        }

        Console.Write("Enter Assignment ID to update treatment: ");
        if (!int.TryParse(Console.ReadLine(), out int assignmentId))
        {
            Console.WriteLine("Invalid assignment ID.");
            return;
        }

        updateTreatment(callId, assignmentId);
        Console.WriteLine("Treatment updated successfully.");

    }

    static void AssignCallToTutor()
    {
        Console.Write("Enter Call ID: ");
        if (!int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.WriteLine("Invalid Call ID.");
            return;
        }

        Console.Write("Enter Tutor ID to assign the call to: ");
        if (!int.TryParse(Console.ReadLine(), out int tutorId))
        {
            Console.WriteLine("Invalid Tutor ID.");
            return;
        }

        s_bl.StudentCall.AssignCallToTutor(callId, tutorId);
        Console.WriteLine("Call assigned to tutor successfully.");
    }

    // System Management Menu
    private static void AdminMenu()
    {
        bool exit = false;
        while (!exit)
        {
            try
            {
                Console.WriteLine("Admin Management Menu");
                int i = 1;
                foreach (var option in Enum.GetValues(typeof(AdminMenuChoice)))
                    Console.WriteLine(i++ + ". " + option);
                Console.Write("Enter choice: ");
                if (!Enum.TryParse(Console.ReadLine(), true, out AdminMenuChoice choice) && Enum.IsDefined(typeof(AdminMenuChoice), choice))
                    Console.WriteLine("Invalid choice");

                switch (choice)
                {
                    case AdminMenuChoice.GetSystemClock:
                        GetSystemClock(); break;
                    case AdminMenuChoice.AdvanceSystemClock:
                        AdvanceSystemClock(); break;
                    case AdminMenuChoice.GetRiskTimeRange:
                        GetRiskTimeRange(); break;
                    case AdminMenuChoice.SetRiskTimeRange:
                        SetRiskTimeRange(); break;
                    case AdminMenuChoice.ResetDatabase:
                        ResetDatabase(); break;
                    case AdminMenuChoice.InitializeDatabase:
                        InitializeDatabase(); break;
                    case AdminMenuChoice.Exit:
                        exit = true; break;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }

    private static void InitializeDatabase()
    {
        s_bl.Admin.InitializeDatabase();
        Console.WriteLine("Database initialized successfully.");
    }

    private static void SetRiskTimeRange()
    {
        Console.WriteLine("Enter a risk time range:");
        if (TimeSpan.TryParse(Console.ReadLine(), out TimeSpan timeRange))
        {
            s_bl.Admin.SetRiskTimeRange(timeRange);
            Console.WriteLine("Risk time range updated successfully");
        }
        else
            Console.WriteLine("Invalid time range");
    }

    private static void GetRiskTimeRange()
    {
        var timeRange = s_bl.Admin.GetRiskTimeRange();
        Console.WriteLine($"System risk time range: {timeRange}");
    }

    // Get the current system clock
    private static void GetSystemClock()
    {
        var clock = s_bl.Admin.GetSystemClock();
        Console.WriteLine($"System Clock: {clock}");
    }

    // Advance the system clock
    private static void AdvanceSystemClock()
    {
        Console.WriteLine("Select time unit (Minute, Hour, Day, Month, Year):");
        var input = Console.ReadLine();

        if (Enum.TryParse(input, out BO.TimeUnit timeUnit))
        {
            s_bl.Admin.AdvanceClock(timeUnit);
            Console.WriteLine("Clock advanced successfully.");
        }
        else
        {
            Console.WriteLine("Invalid time unit.");
        }
    }

    // Reset the database
    private static void ResetDatabase()
    {
        s_bl.Admin.ResetDatabase();
        Console.WriteLine("Database reset successfully.");
    }
}