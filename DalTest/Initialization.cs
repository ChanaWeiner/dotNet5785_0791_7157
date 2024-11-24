using DalApi;
using DO;
namespace DalTest;

public static class Initialization
{
    private static ITutor? s_dalTutor; //stage 1
    private static IStudentCall? s_dalStudentCall;
    private static IAssignment? s_dalAssignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();


    private static void createTutors()
    {
        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira", "Rivka", "David", "Moshe", "Tamar" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof", "Mizrahi", "Peretz", "Azoulay", "Sharabi" };

        // הגדרה של טווח רנדומלי לת.ז, לדוגמא
        const int MIN_ID = 200000000;
        const int MAX_ID = 400000000;

        for (int i = 0; i < 10; i++) // ניצור 10 מדריכים
        {
            int id;
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID); // ת.ז רנדומלי
            }
            while (s_dalTutor!.Read(id) != null); // ווידוא שהמדריך לא קיים כבר ברשימה

            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            string password = $"Pass{id % 1000}!";
            string currentAddress = $"Street {s_rand.Next(1, 100)}, City {s_rand.Next(1, 20)}";
            double latitude = s_rand.NextDouble() * 180 - 90; // טווח בין -90 ל-90
            double longitude = s_rand.NextDouble() * 360 - 180; // טווח בין -180 ל-180
            Role role = (i == 0) ? Role.MANEGAR : (Role)s_rand.Next(2); // רול רנדומלי
            bool active = s_rand.Next(0, 2) == 1; // רנדומלי
            double distance = s_rand.NextDouble() * 20; // מרחק רנדומלי עד 20 ק"מ
            DistanceType distanceType = (DistanceType)s_rand.Next(3); // DistanceType רנדומלי

            // יצירת האובייקט והוספתו דרך הממשק
            s_dalTutor!.Create(new Tutor(id, fullName, cellNumber, email, password, currentAddress, latitude, longitude, role, active, distance, distanceType));
        }
    }

    private static void createStudentCalls()
    {
        // מערך של שמות נושאים רלוונטיים
        string[] subjects = { "English", "Math", "Grammer", "Programming", "History" };

        // מערך של כתובות
        string[] addresses = { "123 Main St, City A", "45 Elm St, City B", "678 Pine Rd, City C", "89 Maple Ave, City D" };

        // מערך של שמות פרטיים ושמות משפחה ליצירת שמות רנדומליים
        string[] firstNames = { "Noa", "Itai", "Maya", "Amit", "Eden", "Omer", "Roni", "Tal", "Shai", "Yael" };
        string[] lastNames = { "Levi", "Cohen", "Mizrahi", "Peretz", "Sharabi", "Azoulay", "Hazan", "Katz", "Berger", "Shaked" };

        for (int i = 0; i < 15; i++) // ניצור 15 קריאות שירות
        {
            // הפקת נתונים רנדומליים
            int numSubject = s_rand.Next(Enum.GetValues(typeof(Subjects)).Length);
            Subjects subject = (Subjects)numSubject;
            string description = $"Request for help in {subjects[numSubject]}";
            string fullAddress = addresses[s_rand.Next(addresses.Length)];
            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            double latitude = s_rand.NextDouble() * 180 - 90; // טווח בין -90 ל-90
            double longitude = s_rand.NextDouble() * 360 - 180; // טווח בין -180 ל-180

            // יצירת זמן פתיחה וסגירה רנדומליים
            DateTime openTime = s_dalConfig!.Clock.AddDays(-s_rand.Next(1, 365)); // רנדומלי עד שנה אחורה
            DateTime? finalTime = s_rand.Next(0, 2) == 1 ? openTime.AddHours(s_rand.Next(1, 48)) : null; // רנדומלי אם הקריאה נסגרה

            // יצירת האובייקט והוספתו דרך הממשק
            s_dalStudentCall!.Create(new StudentCall(0, subject, description, fullAddress, fullName, cellNumber, email, latitude, longitude, openTime, finalTime));
        }
    }


    private static void createAssignments()
    {
        // קבלת כל הקריאות והחונכים הקיימים
        List<StudentCall> studentCalls = s_dalStudentCall!.ReadAll();
        List<Tutor> tutors = s_dalTutor!.ReadAll();

        if (studentCalls.Count == 0 || tutors.Count == 0)
            throw new Exception("Cannot initialize assignments: no student calls or tutors available.");

        // ניצור 10 משימות
        for (int i = 0; i < Math.Min(studentCalls.Count, tutors.Count); i++)
        {
            // בחירת קריאה רנדומלית ומדריך רנדומלי
            StudentCall studentCall = studentCalls[s_rand.Next(studentCalls.Count)];
            Tutor tutor = tutors[s_rand.Next(tutors.Count)];

            // בדיקה אם כבר קיימת משימה עם קריאה זו ומדריך זה
            DateTime entryTime = s_dalConfig!.Clock.AddDays(-s_rand.Next(1, 30)); // זמן כניסה רנדומלי עד חודש אחורה
            DateTime? endTime = s_rand.Next(0, 2) == 1 ? entryTime.AddHours(s_rand.Next(1, 48)) : null; // זמן סיום רנדומלי

            // סטטוס טיפול: אם זמן סיום קיים, נבחר "טופל", אחרת "בתהליך"
            EndOfTreatment status = endTime.HasValue ? EndOfTreatment.TREATED : EndOfTreatment.EXPIRED;

            // יצירת האובייקט והוספתו דרך הממשק
            s_dalAssignment!.Create(new Assignment(0, studentCall.Id, tutor.Id, entryTime, endTime, status));
        }
    }

    public static void Do( ITutor? dalTutors, IStudentCall? dalStudentCalls, IAssignment? dalAssignments, IConfig? dalConfig)
    {
        s_dalTutor = dalTutors ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalStudentCall = dalStudentCalls ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignments ?? throw new NullReferenceException("DAL object can not be null!");
        Console.WriteLine("Reset Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalStudentCall.DeleteAll();
        s_dalStudentCall.DeleteAll();
        s_dalAssignment.DeleteAll();
        Console.WriteLine("Initializing All lists ...");
        createTutors();
        createStudentCalls();
        createAssignments();

    }


}
