using Dal;
using DalApi;
using DalTest;
using DO;

internal class Program
{
    enum mainMenu { EXIT = 1, DISPLAY_TUTORS, DISPLAY_STUDENT_CALLS, DISPLAY_ASSIGNMENT, INITIALIZATION, DISPLAY_ALL_DATA, DISPLAY_CONFIG, RESET };
    enum subMenue { EXIT = 1, CREATE, READ, READ_ALL, UPDATE, DELETE, DELETE_ALL };
    enum configSubMenu { EXIT = 1, PROMOTE_MINUTE, PROMOTE_HOUR, DISPLAY_TIME, SET_CONFIG_VARIABLE, DISPLAY_VALUE, RESET }
    private static ITutor? s_dalTutor = new TutorImplementation();
    private static IStudentCall? s_dalStudentCall = new StudentCallImplementation();
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static IConfig? s_dalConfig = new ConfigImplementation();
    private static void displayMainMenu()
    {
        while (true)
        {
            try
            {

                Console.WriteLine("Select an action from the main menu:");
                Console.WriteLine("1. Exit the main menu");
                Console.WriteLine("2. Display submenu for Tutors");
                Console.WriteLine("3. Display submenu for Student Calls");
                Console.WriteLine("4. Display submenu for Assignments");
                Console.WriteLine("5. Initialize data (call the Initialization.Do method)");
                Console.WriteLine("6. Display all data in the database (for all entities)");
                Console.WriteLine("7. Display submenu for Configuration");
                Console.WriteLine("8. Reset the database and configuration data");
                Console.WriteLine("Select the appropriate number from the options above:");

                mainMenu option = (mainMenu)int.Parse(Console.ReadLine());

                switch (option)
                {
                    case mainMenu.EXIT:
                        return;
                    case mainMenu.DISPLAY_TUTORS:
                        displayEntityMenu("Tutor");
                        break;
                    case mainMenu.DISPLAY_STUDENT_CALLS:
                        displayEntityMenu("StudentCall");
                        break;
                    case mainMenu.DISPLAY_ASSIGNMENT:
                        displayEntityMenu("Assignment");
                        break;
                    case mainMenu.INITIALIZATION:
                        Initialization.Do(s_dalTutor, s_dalStudentCall, s_dalAssignment, s_dalConfig);
                        break;
                    case mainMenu.DISPLAY_ALL_DATA:
                        displayAllDataMenu();
                        break;
                    case mainMenu.DISPLAY_CONFIG:
                        displayConfigMenu();
                        break;
                    case mainMenu.RESET:
                        s_dalTutor!.DeleteAll();
                        s_dalAssignment!.DeleteAll();
                        s_dalConfig!.Reset();
                        s_dalStudentCall!.DeleteAll();
                        break;

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }
    }
    private static void displayEntityMenu(string entity)
    {

        while (true)
        {
            try
            {
                Console.WriteLine("Select the method you want to perform:");
                Console.WriteLine("1. Exit the submenu");
                Console.WriteLine("2. Add a new object (Create)");
                Console.WriteLine("3. Display an object by ID (Read)");
                Console.WriteLine("4. Display a list of all objects (ReadAll)");
                Console.WriteLine("5. Update an object (Update)");
                Console.WriteLine("6. Delete an object (Delete)");
                Console.WriteLine("7. Delete all objects (DeleteAll)");

                subMenue option = (subMenue)int.Parse(Console.ReadLine());

                switch (option)
                {
                    case subMenue.EXIT: // יציאה מתת-תפריט
                        return;

                    case subMenue.CREATE:
                        createOrUpdateObject(entity, true);
                        break;

                    case subMenue.READ: // תצוגת אובייקט ע"פ מזהה (READ)
                        readEntityById(entity);
                        break;

                    case subMenue.READ_ALL: // תצוגת כל האובייקטים (READ_ALL)
                        readAllEntities(entity);
                        break;

                    case subMenue.UPDATE: // עדכון אובייקט (UPDATE)
                        createOrUpdateObject(entity, false);
                        break;

                    case subMenue.DELETE: // מחיקת אובייקט (DELETE)
                        deleteEntity(entity);
                        break;

                    case subMenue.DELETE_ALL: // מחיקת כל האובייקטים (DELETE_ALL)
                        deleteAllEntities(entity);
                        break;

                    default:
                        Console.WriteLine("choose validate choise.");
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }



    }
    private static void createOrUpdateObject(string entity, bool isCreate)
    {
        if (isCreate)
            Console.WriteLine($"Create an {entity}");
        else
            Console.WriteLine($"Update an {entity} enter null in fields you don't want to change");
        int id;
        string fullName, cellNumber, email, password, currentAddress;
        double latitude, longitude;
        switch (entity)
        {
            case "Tutor":
                Console.WriteLine("Enter the following details:");

                Console.Write("Id: ");
                id = int.Parse(Console.ReadLine());

                Console.Write("Full Name: ");
                fullName = Console.ReadLine();
                Console.Write("Cell Number: ");
                cellNumber = Console.ReadLine();

                Console.Write("Email: ");
                email = Console.ReadLine();

                Console.Write("Password: ");
                password = Console.ReadLine();

                Console.Write("Current Address: ");
                currentAddress = Console.ReadLine();

                Console.Write("Latitude: ");
                latitude = double.Parse(Console.ReadLine());

                Console.Write("Longitude: ");
                longitude = double.Parse(Console.ReadLine());

                Console.Write("Role: ");
                Role role = (Role)int.Parse(Console.ReadLine());

                Console.Write("Active (true/false): ");
                bool isActive = bool.Parse(Console.ReadLine());

                Console.Write("Distance: ");
                double distance = double.Parse(Console.ReadLine());

                Console.Write("Distance Type: ");
                DistanceType distanceType = (DistanceType)int.Parse(Console.ReadLine());
                if (isCreate)
                    s_dalTutor!.Create(new Tutor(id, fullName, cellNumber, email, password, currentAddress, latitude, longitude, role, isActive, distance, distanceType));
                else
                {
                    var tutor = s_dalTutor.Read(id);
                    var updateEntity = tutor with
                    {
                        FullName = fullName ?? tutor.FullName,
                        CellNumber = cellNumber ?? tutor.CellNumber,
                        Email = email ?? tutor.Email,
                        Password = password ?? tutor.Password,
                        CurrentAddress = currentAddress ?? tutor.CurrentAddress,
                        Latitude = latitude != 0 ? latitude : tutor.Latitude,
                        Longitude = longitude != 0 ? longitude : tutor.Longitude,
                        Role = role > 0 ? role : tutor.Role,
                        Active = isActive != null ? isActive : tutor.Active,
                        Distance = distance != 0 ? distance : tutor.Distance,
                        DistanceType = distanceType > 0 ? distanceType : tutor.DistanceType
                    };
                    s_dalTutor!.Update(updateEntity);
                }
                break;
            case "StusentCall":
                Console.WriteLine("Enter the following details:");

                StudentCall? studentCall = null;
                if (!isCreate)
                {
                    Console.Write("Id: ");
                    id = int.Parse(Console.ReadLine());
                    studentCall = (StudentCall)s_dalStudentCall.Read(id);
                }

                Console.WriteLine("Subject (choose one of the following: Math, Science, History, Literature, Art): ");
                Subjects subject = (Subjects)Enum.Parse(typeof(Subjects), Console.ReadLine(), true);

                Console.Write("Description: ");
                string description = Console.ReadLine();

                Console.Write("Full Address: ");
                string fullAddress = Console.ReadLine();

                Console.Write("Full Name: ");
                fullName = Console.ReadLine();

                Console.Write("Cell Number: ");
                cellNumber = Console.ReadLine();

                Console.Write("Email: ");
                email = Console.ReadLine();

                Console.Write("Latitude: ");
                latitude = double.Parse(Console.ReadLine());

                Console.Write("Longitude: ");
                longitude = double.Parse(Console.ReadLine());

                Console.Write("Open Time (format: yyyy-MM-dd HH:mm, or leave empty): ");
                string openTimeInput = Console.ReadLine();
                DateTime? openTime = string.IsNullOrWhiteSpace(openTimeInput)
                    ? (DateTime?)null
                    : DateTime.Parse(openTimeInput);

                Console.Write("Final Time (format: yyyy-MM-dd HH:mm, or leave empty): ");
                string finalTimeInput = Console.ReadLine();
                DateTime? finalTime = string.IsNullOrWhiteSpace(finalTimeInput)
                    ? (DateTime?)null
                    : DateTime.Parse(finalTimeInput);
                s_dalStudentCall!.Create(new StudentCall(0, subject, description, fullAddress, fullName, cellNumber, email, latitude, longitude, openTime, finalTime));
                if (isCreate)
                {
                    s_dalStudentCall.Create(new StudentCall(0, subject, description, fullAddress, fullName, cellNumber, email,
                        latitude, longitude, openTime, finalTime));
                }
                else
                {
                    var updateStudentCall = studentCall with
                    {
                        Subject = subject != null ? subject : studentCall.Subject,
                        Description = description ?? studentCall.Description,
                        FullAddress = fullAddress ?? studentCall.FullAddress,
                        FullName = fullName ?? studentCall.FullName,
                        CellNumber = cellNumber ?? studentCall.CellNumber,
                        Email = email ?? studentCall.Email,
                        Latitude = latitude != null ? latitude : studentCall.Latitude,
                        Longitude = longitude != null ? longitude : studentCall.Longitude,
                        OpenTime = openTime ?? studentCall.OpenTime,
                        FinalTime = finalTime ?? studentCall.FinalTime
                    };
                    s_dalStudentCall.Update(updateStudentCall);
                }
                break;
            case "Assignment":
                Console.WriteLine("Enter the following details:");
                Assignment? assignment = null;
                if (!isCreate)
                {
                    Console.Write("Id: ");
                    id = int.Parse(Console.ReadLine());
                    assignment = s_dalAssignment.Read(id);
                }
                Console.Write("Student Call Id: ");
                int studentCallId = int.Parse(Console.ReadLine());

                Console.Write("Tutor Id: ");
                int tutorId = int.Parse(Console.ReadLine());

                Console.Write("Entry Time (format: yyyy-MM-dd HH:mm, or leave empty): ");
                string entryTimeInput = Console.ReadLine();
                DateTime? entryTime = string.IsNullOrWhiteSpace(entryTimeInput)
                    ? (DateTime?)null
                    : DateTime.Parse(entryTimeInput);

                Console.Write("End Time (format: yyyy-MM-dd HH:mm, or leave empty): ");
                string endTimeInput = Console.ReadLine();
                DateTime? endTime = string.IsNullOrWhiteSpace(endTimeInput)
                    ? (DateTime?)null
                    : DateTime.Parse(endTimeInput);

                s_dalAssignment!.Create(new Assignment(0, studentCallId, tutorId, entryTime, endTime, 0));

                if (isCreate)
                {
                    s_dalAssignment.Create(new Assignment(0, studentCallId, tutorId, entryTime, endTime, 0));
                }
                else
                {
                    var updateAssignment = assignment with
                    {
                        StudentCallId = studentCallId != null ? studentCallId : assignment.StudentCallId,
                        TutorId = tutorId != null ? tutorId : assignment.TutorId,
                        EntryTime = entryTime ?? assignment.EntryTime,
                        EndTime = endTime ?? assignment.EndTime
                    };
                    s_dalAssignment.Update(updateAssignment);
                }
                break;
        }
    }

    private static void readEntityById(string entity)
    {
        Console.WriteLine("enter id:");
        int id = int.Parse(Console.ReadLine());

        switch (entity)
        {
            case "Tutor":
                var tutor = s_dalTutor!.Read(id);
                Console.WriteLine(tutor);
                break;
            case "StusentCall":
                var studentCall = s_dalStudentCall!.Read(id);
                Console.WriteLine(studentCall);
                break;
            case "Assignment":
                var assignment = s_dalAssignment!.Read(id);
                Console.WriteLine(assignment);
                break;
        }
    }
    private static void readAllEntities(string entity)
    {
        switch (entity)
        {
            case "Tutor":
                var tutors = s_dalTutor!.ReadAll();
                tutors.ForEach(x => Console.WriteLine(x));
                break;
            case "StudentCall":
                var studentCalls = s_dalStudentCall!.ReadAll();
                studentCalls.ForEach(x => Console.WriteLine(x));
                break;
            case "Assignment":
                var assignments = s_dalAssignment!.ReadAll();
                assignments.ForEach(x => Console.WriteLine(x));
                break;
        }
    }
    private static void displayAllDataMenu()
    {
        readAllEntities("Tutor");
        readAllEntities("StudentCall");
        readAllEntities("Assignment");
    }
    private static void displayConfigMenu()
    {
        while (true)
        {
            Console.WriteLine("\n--- Configuration Submenu ---");
            Console.WriteLine("1. Exit Submenu");
            Console.WriteLine("2. Promote system clock by 1 minute");
            Console.WriteLine("3. Promote system clock by 1 hour");
            Console.WriteLine("4. Display current value of the system clock");
            Console.WriteLine("5. Set a new value for a specific configuration variable");
            Console.WriteLine("6. Display the current value of a specific configuration variable");
            Console.WriteLine("7. Reset all configuration values");
            configSubMenu option = (configSubMenu)int.Parse(Console.ReadLine());

            switch (option)
            {
                case configSubMenu.EXIT:
                    return;
                case configSubMenu.PROMOTE_MINUTE:
                    s_dalConfig!.Clock = s_dalConfig.Clock.AddMinutes(1);
                    break;
                case configSubMenu.PROMOTE_HOUR:
                    s_dalConfig!.Clock = s_dalConfig.Clock.AddHours(1);
                    break;
                case configSubMenu.DISPLAY_TIME:
                    Console.WriteLine($"Current time: {s_dalConfig!.Clock}");
                    break;
                case configSubMenu.SET_CONFIG_VARIABLE:
                    Console.WriteLine("Setting the time");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime setTime)) throw new FormatException("Date is invalid!");
                    s_dalConfig!.Clock = (DateTime)setTime;
                    break;
                case configSubMenu.DISPLAY_VALUE:
                    Console.WriteLine(s_dalConfig!.Clock);
                    break;
                case configSubMenu.RESET:
                    Console.WriteLine("Resetting configuration...");
                    s_dalConfig!.Reset();
                    break;
            }
        }
    }
    private static void deleteAllEntities(string entity)
    {

        switch (entity)
        {
            case "Tutor":
                s_dalTutor!.DeleteAll();
                break;
            case "StudentCall":
                s_dalStudentCall!.DeleteAll();
                break;
            case "Assignment":
                s_dalAssignment!.DeleteAll();
                break;
        }

    }
    private static void deleteEntity(string entity)
    {
        Console.WriteLine("enter id:");
        int id = int.Parse(Console.ReadLine());

        switch (entity)
        {
            case "Tutor":
                s_dalTutor!.Delete(id);
                break;
            case "StusentCall":
                s_dalStudentCall!.Delete(id);
                break;
            case "Assignment":
                s_dalAssignment!.Delete(id);
                break;
        }
    }

    private static void Main(string[] args)
    {
        try
        {
            displayMainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}