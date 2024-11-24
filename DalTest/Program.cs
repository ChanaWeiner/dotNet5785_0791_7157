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
        mainMenu option = (mainMenu)int.Parse(Console.ReadLine());
        switch (option)
        {
            case mainMenu.EXIT: 
                return;
            case mainMenu.DISPLAY_TUTORS:
                displayTutorsMenu(); 
                break;
            case mainMenu.DISPLAY_STUDENT_CALLS:
                displayStudentCallsMenu(); 
                break;
            case mainMenu.DISPLAY_ASSIGNMENT:
                displayAssignmentMenu(); 
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
    private static void displayTutorsMenu() {
    
    
    }
    private static void displayAssignmentMenu() { }
    private static void displayStudentCallsMenu() { }
    private static void displayConfigMenu() { }
    private static void displayAllDataMenu() { }

    private static void reset()
    {
        Console.WriteLine("בחר את המתודה שברצונך לבצע:");
        Console.WriteLine("1. יציאה מתת-תפריט");
        Console.WriteLine("2. הוספת אובייקט חדש (Create)");
        Console.WriteLine("3. תצוגת אובייקט עפ מזהה(Read)");
    
        Console.WriteLine("4. תצוגת רשימת כל האובייקטים (ReadAll)");
        Console.WriteLine("5. עדכון אובייקט (Update)");
        Console.WriteLine("6. מחיקת אובייקט (Delete)");
        Console.WriteLine("7. מחיקת כל האובייקטים (DeleteAll)");
        subMenue option = (subMenue)int.Parse(Console.ReadLine());
        switch (option) {
            case subMenue.EXIT: // יציאה מתת-תפריט
                return;

            case subMenue.CREATE: // הוספת אובייקט חדש (CREATE)
                Console.WriteLine("הוסף אובייקט חדש:");
                // הוסף אובייקט חדש מטיפוס Tutor לדוגמה
                s_dalTutor.Create(new Tutor { Id = 1, Name = "Tutor Name" });
                Console.WriteLine("אובייקט נוצר בהצלחה.");
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
                s_dalTutor.Update(new Tutor { Id = idToUpdate, Name = "Updated Tutor" });
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

    private static void Main(string[] args)
    {
        try
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
    }
}