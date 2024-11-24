using Dal;
using DalApi;
using DalTest;
using DO;

internal class Program
{
    enum mainMenu { EXIT, DISPLAY_TUTORS, DISPLAY_STUDENT_CALLS, DISPLAY_ASSIGNMENT, INITIALIZATION, DISPLAY_ALL_DATA, DISPLAY_CONFIG, RESET };
    enum subMenue { EXIT, CREATE, READ, READ_ALL, UPDATE, DELETE, DELETE_ALL };
    private static ITutor? s_dalTutor = new TutorImplementation();
    private static IStudentCall? s_dalStudentCall = new StudentCallImplementation();
    private static IAssignment? s_dalAssignment = new AssignmentImplementation();
    private static IConfig? s_dalConfig = new ConfigImplementation();
    private static void displayMainMenu()
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
                reset();
                break;

        }

    }

    private static void displayEntityMenu(string entity)
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
                createObject(entity);
                break;

            case subMenue.READ: // תצוגת אובייקט ע"פ מזהה (READ)
                Console.WriteLine("הזן מזהה לאובייקט:");
                int id = int.Parse(Console.ReadLine());
                var tutor = s_dalTutor.Read(id);
                Console.WriteLine(tutor);
                break;

            case subMenue.READ_ALL: // תצוגת כל האובייקטים (READ_ALL)
                var tutors = s_dalTutor.ReadAll();
                foreach (var tutorItem in tutors)
                {
                    Console.WriteLine(tutorItem);
                }
                break;

            case subMenue.UPDATE: // עדכון אובייקט (UPDATE)
                Console.WriteLine("הזן מזהה לעדכון:");
                int idToUpdate = int.Parse(Console.ReadLine());
                s_dalTutor.Update(new Tutor { Id = idToUpdate });
                Console.WriteLine("האובייקט עודכן בהצלחה.");
                break;

            case subMenue.DELETE: // מחיקת אובייקט (DELETE)
                Console.WriteLine("הזן מזהה למחיקת אובייקט:");
                int idToDelete = int.Parse(Console.ReadLine());
                s_dalTutor.Delete(idToDelete);
                Console.WriteLine("האובייקט נמחק בהצלחה.");
                break;

            case subMenue.DELETE_ALL: // מחיקת כל האובייקטים (DELETE_ALL)
                s_dalTutor.DeleteAll();
                Console.WriteLine("כל האובייקטים נמחקו בהצלחה.");
                break;

            default:
                Console.WriteLine("בחר אפשרות חוקית.");
                break;
        }

        

    }

    private static void createObject(string entity)
    {
        Console.WriteLine($"Create an {entity}");

        switch (entity)
        {
            case "Tutor":
                Console.WriteLine("Enter please: Id,FullName,CellNumber,Email,Email,Password,CurrentAddress, Latitude,Longitude,role,Active,Distance,DistanceType");
                break;
            case "StusentCall":

                break;
            case "Assignment":

                break;
        }
    }

    private static void displayConfigMenu() { }
    private static void displayAllDataMenu() { }

    private static void reset()
    {

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