using Dal;
using DalApi;
using DalTest;
using DO;
using System.Data.SqlTypes;
using System.Diagnostics;

internal class Program
{
    /// Enum for main menu
    enum MainMenu { Exit = 1, DisplayTutors, DisplayStudentCalls, DisplayAssignment, Initialization, DisplayAllData, DisplayConfig, Reset }

    /// Enum for sub-menu
    enum SubMenu { Exit = 1, Create, Read, ReadAll, Update, Delete, DeleteAll }

    /// Enum for configuration sub-menu
    enum ConfigSubMenu { Exit = 1, PromoteMinute, PromoteHour, DisplayTime, SetConfigVariable, DisplayValue, Reset }

    /// Data Access Layer instance
    //static readonly IDal s_dal = new DalXml(); // stage 3
    static readonly IDal s_dal = Factory.Get; //stage 4



    /// Displays the main menu and handles user input
    private static void DisplayMainMenu()
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

                MainMenu option = (MainMenu)int.Parse(Console.ReadLine());

                switch (option)
                {
                    case MainMenu.Exit:
                        return;
                    case MainMenu.DisplayTutors:
                        DisplayEntityMenu("Tutor");
                        break;
                    case MainMenu.DisplayStudentCalls:
                        DisplayEntityMenu("StudentCall");
                        break;
                    case MainMenu.DisplayAssignment:
                        DisplayEntityMenu("Assignment");
                        break;
                    case MainMenu.Initialization:
                        Initialization.Do();
                        break;
                    case MainMenu.DisplayAllData:
                        DisplayAllDataMenu();
                        break;
                    case MainMenu.DisplayConfig:
                        DisplayConfigMenu();
                        break;
                    case MainMenu.Reset:
                        s_dal.Tutor!.DeleteAll();
                        s_dal.Assignment!.DeleteAll();
                        s_dal.Config!.Reset();
                        s_dal.StudentCall!.DeleteAll();
                        break;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    /// Displays the entity-specific menu and handles user actions
    private static void DisplayEntityMenu(string entity)
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

                SubMenu option = (SubMenu)int.Parse(Console.ReadLine());

                switch (option)
                {
                    case SubMenu.Exit:
                        return;
                    case SubMenu.Create:
                        CreateOrUpdateObject(entity, true);
                        break;
                    case SubMenu.Read:
                        ReadEntityById(entity);
                        break;
                    case SubMenu.ReadAll:
                        ReadAllEntities(entity);
                        break;
                    case SubMenu.Update:
                        CreateOrUpdateObject(entity, false);
                        break;
                    case SubMenu.Delete:
                        DeleteEntity(entity);
                        break;
                    case SubMenu.DeleteAll:
                        DeleteAllEntities(entity);
                        break;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }
    }

    /// Creates or updates an object based on the specified entity type
    private static void CreateOrUpdateObject(string entity, bool isCreate)
    {
        if (isCreate)
            Console.WriteLine($"Create an {entity}");
        else
            Console.WriteLine($"Update an {entity} enter null in fields you don't want to change");

        int id;
        string fullName, cellNumber, email, password, currentAddress, description, fullAddress;
        double latitude, longitude;
        DateTime openTime;
        DateTime? finalTime;

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
                    s_dal.Tutor!.Create(new Tutor(id, fullName, cellNumber, email, password, currentAddress, latitude, longitude, role, isActive, distance, distanceType));
                else
                {
                    var tutor = s_dal.Tutor.Read(id);
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
                    s_dal.Tutor!.Update(updateEntity);
                }
                break;
            case "StudentCall":
                Console.WriteLine("Enter the following details:");
                Console.Write("Id: ");
                id = int.Parse(Console.ReadLine());
                Console.Write("Subject: ");
                Subjects subject = (Subjects)int.Parse(Console.ReadLine());
                Console.Write("Description: ");
                description = Console.ReadLine();
                Console.Write("Full Address: ");
                fullAddress = Console.ReadLine();
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
                Console.Write("Open Time (format: yyyy-MM-dd HH:mm:ss): ");
                openTime = DateTime.Parse(Console.ReadLine());
                Console.Write("Final Time (format: yyyy-MM-dd HH:mm:ss) (optional, enter null if no final time): ");
                string finalTimeInput = Console.ReadLine();
                finalTime = string.IsNullOrEmpty(finalTimeInput) ? (DateTime?)null : DateTime.Parse(finalTimeInput);

                if (isCreate)
                {
                    s_dal.StudentCall!.Create(new StudentCall(id, subject, description, fullAddress, fullName, cellNumber, email, latitude, longitude, openTime, finalTime));
                }
                else
                {
                    var studentCall = s_dal.StudentCall.Read(id);
                    var updatedStudentCall = studentCall with
                    {
                        Subject = subject != 0 ? subject : studentCall.Subject,
                        Description = description ?? studentCall.Description,
                        FullAddress = fullAddress ?? studentCall.FullAddress,
                        FullName = fullName ?? studentCall.FullName,
                        CellNumber = cellNumber ?? studentCall.CellNumber,
                        Email = email ?? studentCall.Email,
                        Latitude = latitude != 0 ? latitude : studentCall.Latitude,
                        Longitude = longitude != 0 ? longitude : studentCall.Longitude,
                        OpenTime = openTime != DateTime.MinValue ? openTime : studentCall.OpenTime,
                        FinalTime = finalTime ?? studentCall.FinalTime
                    };
                    s_dal.StudentCall.Update(updatedStudentCall);
                }
                break;

            case "Assignment":
                Console.WriteLine("Enter the following details:");
                Console.Write("Id: ");
                id = int.Parse(Console.ReadLine());
                Console.Write("StudentCallId: ");
                int studentCallId = int.Parse(Console.ReadLine());
                Console.Write("TutorId: ");
                int tutorId = int.Parse(Console.ReadLine());
                Console.Write("Entry Time (format: yyyy-MM-dd HH:mm:ss) (optional, enter null if no entry time): ");
                string entryTimeInput = Console.ReadLine();
                DateTime? entryTime = string.IsNullOrEmpty(entryTimeInput) ? (DateTime?)null : DateTime.Parse(entryTimeInput);
                Console.Write("End Time (format: yyyy-MM-dd HH:mm:ss) (optional, enter null if no end time): ");
                string endTimeInput = Console.ReadLine();
                DateTime? endTime = string.IsNullOrEmpty(endTimeInput) ? (DateTime?)null : DateTime.Parse(endTimeInput);
                Console.Write("End Of Treatment (0 for None, 1 for Finished, 2 for Cancelled): ");
                EndOfTreatment endOfTreatment = (EndOfTreatment)int.Parse(Console.ReadLine());

                if (isCreate)
                {
                    s_dal.Assignment!.Create(new Assignment(id, studentCallId, tutorId, entryTime, endTime, endOfTreatment));
                }
                else
                {
                    var assignment = s_dal.Assignment.Read(id);
                    var updatedAssignment = assignment with
                    {
                        StudentCallId = studentCallId != 0 ? studentCallId : assignment.StudentCallId,
                        TutorId = tutorId != 0 ? tutorId : assignment.TutorId,
                        EntryTime = entryTime ?? assignment.EntryTime,
                        EndTime = endTime ?? assignment.EndTime,
                        EndOfTreatment = endOfTreatment != 0 ? endOfTreatment : assignment.EndOfTreatment
                    };
                    s_dal.Assignment.Update(updatedAssignment);
                }
                break;
        }
    }

    /// Reads an object by ID
    private static void ReadEntityById(string entity)
    {
        Console.WriteLine("Enter id:");
        int id = int.Parse(Console.ReadLine());

        switch (entity)
        {
            case "Tutor":
                var tutor = s_dal.Tutor!.Read(id);
                Console.WriteLine(tutor);
                break;
            case "StudentCall":
                var studentCall = s_dal.StudentCall!.Read(id);
                Console.WriteLine(studentCall);
                break;
            case "Assignment":
                var assignment = s_dal.Assignment!.Read(id);
                Console.WriteLine(assignment);
                break;
        }
    }

    /// Reads and prints all entities of a specified type from the database
    private static void ReadAllEntities(string entity)
    {
        switch (entity)
        {
            case "Tutor":
                var tutors = s_dal.Tutor!.ReadAll().ToList();
                tutors.ForEach(x => Console.WriteLine(x));
                break;
            case "StudentCall":
                var studentCalls = s_dal.StudentCall!.ReadAll().ToList();
                studentCalls.ForEach(x => Console.WriteLine(x));
                break;
            case "Assignment":
                var assignments = s_dal.Assignment!.ReadAll().ToList();
                assignments.ForEach(x => Console.WriteLine(x));
                break;
        }
    }

    /// Displays all data by reading and printing all entities from the database
    private static void DisplayAllDataMenu()
    {
        ReadAllEntities("Tutor");
        ReadAllEntities("StudentCall");
        ReadAllEntities("Assignment");
    }

    /// Handles configuration options such as modifying the system clock and displaying/resetting configuration values
    private static void DisplayConfigMenu()
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
            Console.WriteLine("8. Display the current value of RiskRange");
            Console.WriteLine("9. Set a new value for RiskRange");
            ConfigSubMenu option = (ConfigSubMenu)int.Parse(Console.ReadLine());

            switch (option)
            {
                case ConfigSubMenu.Exit:
                    return;
                case ConfigSubMenu.PromoteMinute:
                    s_dal.Config!.Clock = s_dal.Config.Clock.AddMinutes(1);
                    break;
                case ConfigSubMenu.PromoteHour:
                    s_dal.Config!.Clock = s_dal.Config.Clock.AddHours(1);
                    break;
                case ConfigSubMenu.DisplayTime:
                    Console.WriteLine($"Current time: {s_dal.Config!.Clock}");
                    break;
                case ConfigSubMenu.SetConfigVariable:
                    Console.WriteLine("Setting the time");
                    if (!DateTime.TryParse(Console.ReadLine(), out DateTime setTime)) throw new DalDateFormatWorngException("Date is invalid!");
                    s_dal.Config!.Clock = setTime;
                    break;
                case ConfigSubMenu.DisplayValue:
                    Console.WriteLine($"Current Clock Value: {s_dal.Config!.Clock}");
                    break;
                case ConfigSubMenu.Reset:
                    Console.WriteLine("Resetting configuration...");
                    s_dal.Config!.Reset();
                    break;
                case (ConfigSubMenu)8:
                    Console.WriteLine($"Current RiskRange: {s_dal.Config!.RiskTimeSpan}");
                    break;
                case (ConfigSubMenu)9:
                    Console.WriteLine("Enter a new value for RiskRange (format: hh:mm:ss):");
                    if (!TimeSpan.TryParse(Console.ReadLine(), out TimeSpan newRiskRange))
                        throw new DalDateFormatWorngException("Invalid time format!");
                    s_dal.Config.RiskTimeSpan = newRiskRange;
                    break;
            }
        }
    }

    /// Deletes all entities of a specified type from the database
    private static void DeleteAllEntities(string entity)
    {
        switch (entity)
        {
            case "Tutor":
                s_dal.Tutor!.DeleteAll();
                break;
            case "StudentCall":
                s_dal.StudentCall!.DeleteAll();
                break;
            case "Assignment":
                s_dal.Assignment!.DeleteAll();
                break;
        }
    }

    /// Deletes a specific entity of a specified type based on its ID
    private static void DeleteEntity(string entity)
    {
        Console.WriteLine("Enter id:");
        int id = int.Parse(Console.ReadLine());

        switch (entity)
        {
            case "Tutor":
                s_dal.Tutor!.Delete(id);
                break;
            case "StudentCall":
                s_dal.StudentCall!.Delete(id);
                break;
            case "Assignment":
                s_dal.Assignment!.Delete(id);
                break;
        }
    }

    /// Entry point of the program; displays the main menu and handles exceptions
    private static void Main(string[] args)
    {
        try
        {
            DisplayMainMenu();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}
