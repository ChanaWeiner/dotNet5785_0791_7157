using BlImplementation;
using BO;
using DalApi;
namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager //stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    #endregion Stage 4

    #region initialize
    private static readonly Random s_rand = new();

    /// <summary>
    /// Calculates the check digit for a given ID.
    /// </summary>
    private static int CalculateCheckDigit(int id)
    {
        string idStr = id.ToString().PadLeft(8, '0');
        int sum = 0;

        for (int i = 0; i < 8; i++)
        {
            int num = (idStr[i] - '0') * (i % 2 == 0 ? 1 : 2);
            sum += (num > 9) ? num - 9 : num;
        }

        int checkDigit = (10 - (sum % 10)) % 10;
        return checkDigit;
    }

    /// <summary>
    /// Creates random tutors and adds them to the DAL.
    /// </summary>
    private static void CreateTutors()
    {
        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira", "Rivka", "David", "Moshe", "Tamar" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof", "Mizrahi", "Peretz", "Azoulay", "Sharabi" };

        const int MIN_ID = 20000000;
        const int MAX_ID = 40000000;

        for (int i = 0; i < 15; i++)
        {
            int id;
            do
            {
                id = s_rand.Next(MIN_ID, MAX_ID); // Generate random ID
                id = id * 10 + CalculateCheckDigit(id);
            }
            while (s_dal!.Tutor.Read(id) != null); // Ensure the tutor doesn't already exist

            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            string password = $"Pass{id % 1000}!";
            string currentAddress = $"Street {s_rand.Next(1, 100)}, City {s_rand.Next(1, 20)}";
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;
            DO.Role role = (i == 0) ? DO.Role.Manager : (DO.Role)s_rand.Next(2);
            bool active = s_rand.Next(0, 2) == 1;
            double distance = s_rand.NextDouble() * 20;
            DO.DistanceType distanceType = (DO.DistanceType)s_rand.Next(3);

            s_dal!.Tutor.Create(new DO.Tutor(id, fullName, cellNumber, email, BCrypt.Net.BCrypt.HashPassword(password), currentAddress, latitude, longitude, role, active, distance, distanceType));
        }
    }

    /// <summary>
    /// Creates random student calls and adds them to the DAL.
    /// </summary>
    private static void CreateStudentCalls()
    {
        string[] subjects = { "English", "Math", "Grammer", "Programming", "History" };
        string[] addresses = { "123 Main St, City A", "45 Elm St, City B", "678 Pine Rd, City C", "89 Maple Ave, City D" };
        string[] firstNames = { "Noa", "Itai", "Maya", "Amit", "Eden", "Omer", "Roni", "Tal", "Shai", "Yael" };
        string[] lastNames = { "Levi", "Cohen", "Mizrahi", "Peretz", "Sharabi", "Azoulay", "Hazan", "Katz", "Berger", "Shaked" };

        for (int i = 0; i < 50; i++)
        {
            int numSubject = s_rand.Next(Enum.GetValues(typeof(Subjects)).Length);
            DO.Subjects subject = (DO.Subjects)numSubject;
            string description = $"Request for help in {subjects[numSubject]}";
            string fullAddress = addresses[s_rand.Next(addresses.Length)];
            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string cellNumber = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            double latitude = s_rand.NextDouble() * 180 - 90;
            double longitude = s_rand.NextDouble() * 360 - 180;

            DateTime openTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 365));
            DateTime? finalTime = s_rand.Next(0, 2) == 1 ? openTime.AddHours(s_rand.Next(1, 48)) : null;

            s_dal!.StudentCall.Create(new DO.StudentCall(0, subject, description, fullAddress, fullName, cellNumber, email, latitude, longitude, openTime, finalTime));
        }
    }

    /// <summary>
    /// Creates random assignments for student calls and tutors, and adds them to the DAL.
    /// </summary>
    private static void CreateAssignments()
    {
        List<DO.StudentCall> studentCalls = s_dal!.StudentCall.ReadAll().ToList();
        List<DO.Tutor> tutors = s_dal!.Tutor.ReadAll().ToList();

        if (studentCalls.Count == 0 || tutors.Count == 0)
            throw new Exception("Cannot initialize assignments: no student calls or tutors available.");

        for (int i = 0; i < 155; i++)
        {
            DO.StudentCall studentCall = studentCalls[s_rand.Next(studentCalls.Count - 15)];
            DO.Tutor tutor = tutors[s_rand.Next(tutors.Count)];

            DateTime entryTime = s_dal!.Config.Clock.AddDays(-s_rand.Next(1, 30));
            DateTime? endTime = s_rand.Next(0, 2) == 1 ? entryTime.AddHours(s_rand.Next(1, 48)) : null;

            DO.EndOfTreatment status = i < 50 ? DO.EndOfTreatment.Treated : (i < 100 ? DO.EndOfTreatment.SelfCancel : (i < 150 ? DO.EndOfTreatment.ManagerCancel : DO.EndOfTreatment.Expired));

            s_dal!.Assignment.Create(new DO.Assignment(0, studentCall.Id, tutor.Id, entryTime, endTime, status));
        }
    }

    /// <summary>
    /// Initializes the DAL and populates the data with random values.
    /// </summary>
    public static void InitializeDatabase()
    {
        s_dal!.ResetDB();
        Console.WriteLine("Reset Configuration values and List values...");
        Console.WriteLine("Initializing All lists ...");
        CreateTutors();
        CreateStudentCalls();
        CreateAssignments();
    }
    #endregion



    #region Stage 5
    internal static event Action? ConfigUpdatedObservers; //prepared for stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers
    #endregion Stage 5

    #region Stage 4
    /// <summary>
    /// Property for providing/setting current configuration variable value for any BL class that may need it
    /// </summary>
    internal static TimeSpan RiskTimeSpan
    {
        get => s_dal.Config.RiskTimeSpan;
        set
        {
            s_dal.Config.RiskTimeSpan = value;
            ConfigUpdatedObservers?.Invoke(); // stage 5
        }
    }

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4

    /// <summary>
    /// Method to perform application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>
    internal static void UpdateClock(DateTime newClock) //stage 4-7
    {
        // new Thread(() => { // stage 7 - not sure - still under investigation - see stage 7 instructions after it will be released        
        updateClock(newClock);//stage 4-6
        // }).Start(); // stage 7 as above
    }

    private static void updateClock(DateTime newClock) // prepared for stage 7 as DRY to eliminate needless repetition
    {
        var oldClock = s_dal.Config.Clock; //stage 4
        s_dal.Config.Clock = newClock; //stage 4

        //TO_DO:
        //Add calls here to any logic method that should be called periodically,
        //after each clock update
        //for example, Periodic students' updates:
        //Go through all students to update properties that are affected by the clock update
        //(students becomes not active after 5 years etc.)

        StudentCallManager.UpdateStatusCalls(); //stage 4
        //etc ...

        //Calling all the observers of clock update
        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }
    #endregion Stage 4

    #region Stage 7 base
    internal static readonly object blMutex = new();
    private static Thread? s_thread;
    private static int s_interval { get; set; } = 1; //in minutes by second    
    private static volatile bool s_stop = false;
    private static readonly object mutex = new();

    internal static void Start(int interval)
    {
        lock (mutex)
            if (s_thread == null)
            {
                s_interval = interval;
                s_stop = false;
                s_thread = new Thread(clockRunner);
                s_thread.Start();
            }
    }

    internal static void Stop()
    {
        lock (mutex)
            if (s_thread != null)
            {
                s_stop = true;
                s_thread?.Interrupt();
                s_thread = null;
            }
    }

    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            #region Stage 7
            //TO_DO:
            //Add calls here to any logic simulation that was required in stage 7
            //for example: course registration simulation
            //StudentManager.SimulateCourseRegistrationAndGrade(); //stage 7

            //etc...
            #endregion Stage 7

            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }
    #endregion Stage 7 base
}