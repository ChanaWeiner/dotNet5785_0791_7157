using DalApi;
using DO;
using BCrypt;

namespace DalTest;

public static class Initialization
{
    private static IDal? s_dal = DalApi.Factory.Get;
    // private static ITutor? s_dalTutor; // stage 1
    // private static IStudentCall? s_dalStudentCall;
    // private static IAssignment? s_dalAssignment; // stage 1
    // private static IConfig? s_dalConfig; // stage 1
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

        // יצירת מתנדב מנהל קבוע
        int managerId = 328240791;
        if (s_dal!.Tutor.Read(managerId) == null)
        {
            s_dal.Tutor.Create(new DO.Tutor(
                managerId,
                "Chana Weiner",
                "058-3202711",
                "admin@example.com",
                BCrypt.Net.BCrypt.HashPassword("1"),
                "הרב אברהם דוד רוזנטל, ירושלים\r\n",
                31.789353,
                35.171381,
                DO.Role.Manager,
                true,
                100.0,
                DO.DistanceType.Air));
        }

        // מתנדבים רנדומליים
        string[] firstNames = { "Dani", "Eli", "Yair", "Ariela", "Dina", "Shira", "Rivka", "David", "Moshe", "Tamar" };
        string[] lastNames = { "Levy", "Amar", "Cohen", "Levin", "Klein", "Israelof", "Mizrahi", "Peretz", "Azoulay", "Sharabi" };

        for (int i = 0; i < 15; i++)
        {
            var loc = locations[i % locations.Length];
            int id;
            do
            {
                id = s_rand.Next(20000000, 40000000);
                id = id * 10 + CalculateCheckDigit(id);
            }
            while (s_dal.Tutor.Read(id) != null);

            string fullName = $"{firstNames[s_rand.Next(firstNames.Length)]} {lastNames[s_rand.Next(lastNames.Length)]}";
            string email = $"{fullName.Replace(" ", ".").ToLower()}@example.com";
            string cell = $"05{s_rand.Next(0, 10)}-{s_rand.Next(1000000, 9999999)}";
            string pass = $"Pass{id % 1000}!";

            s_dal.Tutor.Create(new DO.Tutor(
                id, fullName, cell, email, BCrypt.Net.BCrypt.HashPassword(pass),
                $"{loc.Address}, {loc.City}", loc.Lat, loc.Lon,
                DO.Role.BeginnerTutor,
                true,
                30 + s_rand.NextDouble() * 100,
                DO.DistanceType.Air));
        }
    }


    private static void CreateStudentCalls()
    {
        var locations = new[] {
    new { City = "Tel Aviv", Address = "Dizengoff St 100",        Lat = 32.080094, Lon = 34.774478 },
    new { City = "Jerusalem", Address = "Jaffa St 23",            Lat = 31.783391, Lon = 35.216024 },
    new { City = "Haifa", Address = "Herzl St 50",                Lat = 32.812003, Lon = 35.003792 },
    new { City = "Be'er Sheva", Address = "Ringelblum St 15",     Lat = 31.244408, Lon = 34.792498 },
    new { City = "Netanya", Address = "Ussishkin St 10",          Lat = 32.329628, Lon = 34.855190 },
    new { City = "Ashdod", Address = "Ha-Tikshoret St 3",         Lat = 31.807644, Lon = 34.652460 },
    new { City = "Modi'in", Address = "Emek Zevulun 12",          Lat = 31.900967, Lon = 35.004367 },
    new { City = "Rehovot", Address = "Herzl St 2",               Lat = 31.894722, Lon = 34.811333 },
    new { City = "Kfar Saba", Address = "Weizmann St 77",         Lat = 32.175222, Lon = 34.910139 },
    new { City = "Ashkelon", Address = "David Remez St 5",        Lat = 31.666612, Lon = 34.574732 }
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

            DateTime open = DateTime.Now.AddDays(-s_rand.Next(1, 20));
            DateTime? final = s_rand.Next(0, 8) == 0 ? open.AddDays(-1) : // פג תוקף
                              s_rand.Next(0, 3) == 0 ? null :
                              open.AddDays(s_rand.Next(5, 25));

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
    public static void Do()
    {
        s_dal!.Tutor.DeleteAll();
        s_dal!.StudentCall.DeleteAll();
        s_dal!.Assignment.DeleteAll();
        Console.WriteLine("Reset Configuration values and List values...");
        Console.WriteLine("Initializing All lists ...");
        CreateTutors();
        CreateStudentCalls();
        //CreateAssignments();
    }
}
