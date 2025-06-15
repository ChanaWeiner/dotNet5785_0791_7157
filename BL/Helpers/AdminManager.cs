using BlImplementation;
using BO;
using DalApi;
namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager//stage 4
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

    private static void CreateTutors()
    {
        var locations = new[] {
        new { City = "Tel Aviv", Address = "Dizengoff 100", Lat = 32.0853, Lon = 34.7818 },
        new { City = "Ramat Gan", Address = "Jabotinsky 33", Lat = 32.0823, Lon = 34.8106 },
        new { City = "Petah Tikva", Address = "Bar Kochva 21", Lat = 32.0871, Lon = 34.8878 },
        new { City = "Holon", Address = "HaHistadrut 15", Lat = 32.0104, Lon = 34.7740 },
        new { City = "Rishon LeZion", Address = "Herzl 5", Lat = 31.9700, Lon = 34.7900 },
        new { City = "Bat Yam", Address = "Balfour 8", Lat = 32.0163, Lon = 34.7480 },
        new { City = "Herzliya", Address = "Sokolov 50", Lat = 32.1656, Lon = 34.8490 },
    };

        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira", "Rivka", "David", "Moshe", "Tamar" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof", "Mizrahi", "Peretz", "Azoulay", "Sharabi" };

        for (int i = 0; i < 16; i++)
        {
            var loc = locations[i % locations.Length];
            int id;
            do
            {
                id = s_rand.Next(20000000, 40000000);
                id = id * 10 + CalculateCheckDigit(id);
            }
            while (s_dal!.Tutor.Read(id) != null);

            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            string cell = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string pass = $"Pass{id % 1000}!";

            s_dal.Tutor.Create(new DO.Tutor(
                id, fullName, cell, email, BCrypt.Net.BCrypt.HashPassword(pass),
                $"{loc.Address}, {loc.City}", loc.Lat, loc.Lon,
                i == 0 ? DO.Role.Manager : DO.Role.BeginnerTutor,
                true,
                30 + s_rand.NextDouble() * 100,
                DO.DistanceType.Air));
        }
    }

    private static void CreateStudentCalls()
    {
        var locations = new[] {
        new { City = "Tel Aviv", Address = "Rothschild Blvd 1", Lat = 32.0656, Lon = 34.7762 },
        new { City = "Ramat Gan", Address = "Bialik St 10", Lat = 32.0809, Lon = 34.8147 },
        new { City = "Petah Tikva", Address = "Arlozorov 3", Lat = 32.0889, Lon = 34.8864 },
        new { City = "Herzliya", Address = "Ben Gurion St 25", Lat = 32.1635, Lon = 34.8442 },
        new { City = "Bat Yam", Address = "Ben Gurion 67", Lat = 32.0195, Lon = 34.7459 },
        new { City = "Holon", Address = "Sderot Yerushalayim 200", Lat = 32.0135, Lon = 34.7703 },
        new { City = "Rishon LeZion", Address = "Rothschild St 7", Lat = 31.9638, Lon = 34.8044 }
    };

        string[] subjects = { "English", "Math", "Grammar", "Programming", "History" };
        string[] firstNames = { "Noa", "Itai", "Maya", "Amit", "Eden", "Omer", "Roni", "Tal", "Shai", "Yael" };
        string[] lastNames = { "Levi", "Cohen", "Mizrahi", "Peretz", "Sharabi", "Azoulay", "Hazan", "Katz", "Berger", "Shaked" };

        for (int i = 0; i < 50; i++)
        {
            int subjectIndex = s_rand.Next(subjects.Length);
            var loc = locations[s_rand.Next(locations.Length)];
            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string phone = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";

            DateTime open = Now.AddDays(-s_rand.Next(1, 20));
            DateTime? final = s_rand.Next(0, 5) == 0 ? open.AddDays(-1) : // פג תוקף
                              s_rand.Next(0, 3) == 0 ? null :
                              open.AddHours(s_rand.Next(5, 48));

            s_dal!.StudentCall.Create(new DO.StudentCall(
                0, (DO.Subjects)subjectIndex, $"Help in {subjects[subjectIndex]}",
                $"{loc.Address}, {loc.City}", fullName, phone, email, loc.Lat, loc.Lon, open, final));
        }
    }

    private static void CreateAssignments()
    {
        var studentCalls = s_dal!.StudentCall.ReadAll().ToList();
        var tutors = s_dal.Tutor.ReadAll().Where(t => t.Role == DO.Role.BeginnerTutor).ToList();
        var untreatedCalls = studentCalls.Where(c => !s_dal.Assignment.ReadAll().Any(a => a.StudentCallId == c.Id)).ToList();

        // ראשית, 5 קריאות בטיפול עכשיו (ולא הסתיימו)  
        for (int i = 0; i < 5; i++)
        {
            var call = untreatedCalls[i];
            var tutor = tutors[i];

            DateTime entry = call.OpenTime.AddHours(1);
            s_dal.Assignment.Create(new DO.Assignment(
                0, call.Id, tutor.Id, entry, null, DO.EndOfTreatment.None));
        }

        // שאר הקצאות רנדומליות  
        int count = 50;
        for (int i = 0; i < count; i++)
        {
            var call = studentCalls[s_rand.Next(studentCalls.Count)];
            var tutor = tutors[s_rand.Next(tutors.Count)];

            DateTime entry = call.OpenTime.AddHours(1 + s_rand.Next(1, 5));
            DateTime? end = s_rand.Next(0, 2) == 1 ? entry.AddHours(s_rand.Next(1, 10)) : null;
            DO.EndOfTreatment status = end == null
                ? DO.EndOfTreatment.None
                : (DO.EndOfTreatment)s_rand.Next(1, 4);

            s_dal.Assignment.Create(new DO.Assignment(0, call.Id, tutor.Id, entry, end, status));
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