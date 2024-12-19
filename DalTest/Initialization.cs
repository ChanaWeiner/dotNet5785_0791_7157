using DalApi;
using DO;

namespace DalTest;

public static class Initialization
{
    private static IDal? s_dal; //stage 2
    //private static ITutor? s_dalTutor; //stage 1
    //private static IStudentCall? s_dalStudentCall;
    //private static IAssignment? s_dalAssignment; //stage 1
    //private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();

    /// Creates random tutors and adds them to the DAL.
    private static void CreateTutors()
    {
        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira", "Rivka", "David", "Moshe", "Tamar" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof", "Mizrahi", "Peretz", "Azoulay", "Sharabi" };

        const int MIN_ID = 200000000;
        const int MAX_ID = 400000000;

        for (int i = 0; i < 15; i++)
        {
            int id;
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID); // Generate random ID
            }
            while (s_dal!.Tutor.Read(id) != null); // Ensure the tutor doesn't already exist

            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            string password = $"Pass{id % 1000}!";
            string currentAddress = $"Street {s_rand.Next(1, 100)}, City {s_rand.Next(1, 20)}";
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;
            Role role = (i == 0) ? Role.Manager : (Role)s_rand.Next(2);
            bool active = s_rand.Next(0, 2) == 1;
            double distance = s_rand.NextDouble() * 20;
            DistanceType distanceType = (DistanceType)s_rand.Next(3);

            s_dal!.Tutor.Create(new Tutor(id, fullName, cellNumber, email, password, currentAddress, latitude, longitude, role, active, distance, distanceType));
        }
    }

    /// Creates random student calls and adds them to the DAL.
    private static void CreateStudentCalls()
    {
        string[] subjects = { "English", "Math", "Grammer", "Programming", "History" };
        string[] addresses = { "123 Main St, City A", "45 Elm St, City B", "678 Pine Rd, City C", "89 Maple Ave, City D" };
        string[] firstNames = { "Noa", "Itai", "Maya", "Amit", "Eden", "Omer", "Roni", "Tal", "Shai", "Yael" };
        string[] lastNames = { "Levi", "Cohen", "Mizrahi", "Peretz", "Sharabi", "Azoulay", "Hazan", "Katz", "Berger", "Shaked" };

        for (int i = 0; i < 50; i++)
        {
            int numSubject = s_rand.Next(Enum.GetValues(typeof(Subjects)).Length);
            Subjects subject = (Subjects)numSubject;
            string description = $"Request for help in {subjects[numSubject]}";
            string fullAddress = addresses[s_rand.Next(addresses.Length)];
            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;

            DateTime openTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 365));
            DateTime? finalTime = s_rand.Next(0, 2) == 1 ? openTime.AddHours(s_rand.Next(1, 48)) : null;

            s_dal!.StudentCall.Create(new StudentCall(0, subject, description, fullAddress, fullName, cellNumber, email, latitude, longitude, openTime, finalTime));
        }
    }

    /// Creates random assignments for student calls and tutors, and adds them to the DAL.
    private static void CreateAssignments()
    {
        List<StudentCall> studentCalls = s_dal!.StudentCall.ReadAll().ToList();
        List<Tutor> tutors = s_dal!.Tutor.ReadAll().ToList();

        if (studentCalls.Count == 0 || tutors.Count == 0)
            throw new Exception("Cannot initialize assignments: no student calls or tutors available.");

        for (int i = 0; i < 155; i++)
        {
            StudentCall studentCall = studentCalls[s_rand.Next(studentCalls.Count - 15)];
            Tutor tutor = tutors[s_rand.Next(tutors.Count)];

            DateTime entryTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 30));
            DateTime? endTime = s_rand.Next(0, 2) == 1 ? entryTime.AddHours(s_rand.Next(1, 48)) : null;

            EndOfTreatment status = i < 50 ? EndOfTreatment.Treated : (i < 100 ? EndOfTreatment.SelfCancel : (i < 150 ? EndOfTreatment.ManagerCancel : EndOfTreatment.Expired));

            s_dal!.Assignment.Create(new Assignment(0, studentCall.Id, tutor.Id, entryTime, endTime, status));
        }
    }

    /// Initializes the DAL and populates the data with random values.
    public static void Do(IDal dal)
    {
        
        s_dal = dal ?? throw new NullReferenceException("DAL object can not be null!");
        s_dal!.ResetDB();

        //s_dal!.Tutor = dalTutors ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dal.StudentCall = dalStudentCalls ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalAssignment = dalAssignments ?? throw new NullReferenceException("DAL object can not be null!");
        //s_dalConfig = dalConfig;
        Console.WriteLine("Reset Configuration values and List values...");
        //s_dalConfig!.Reset();
        //s_dalStudentCall.DeleteAll();
        //s_dalStudentCall.DeleteAll();
        //s_dalAssignment.DeleteAll();
        Console.WriteLine("Initializing All lists ...");
        CreateTutors();
        CreateStudentCalls();
        CreateAssignments();
    }
}
