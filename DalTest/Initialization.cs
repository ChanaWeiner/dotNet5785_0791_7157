
using DalApi;
using DO;
namespace DalTest;

public static class Initialization
{
    private static IStudentCall? s_dalStudentCall;
    private static ITutor? s_dalTutor; //stage 1
    private static IAssignment? s_dalAssignment; //stage 1
    private static IConfig? s_dalConfig; //stage 1
    private static readonly Random s_rand = new();
    private static void createStudentsCall()
    {
       // string[] studentNames =
       //{ "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof" };

       // foreach (var name in studentNames)
       // {
       //     int id;
       //     do
       //         id = s_rand.Next(MIN_ID, MAX_ID);
       //     while (s_dalStudent!.Read(id) != null);

       //     bool? even = (id % 2) == 0 ? true : false;
       //     string? alias = even ? name + "ALIAS" : null;
       //     DateTime start = new DateTime(1995, 1, 1);
       //     DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

       //     s_dalStudent!.Create(new(id, name, alias, even, bdt));
       // }

    }
    private static void createTutor()
    {
        string[] tutorNames =
      { "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein", "Shira Israelof" };
        string[] cellPhones = { "04623875", "034567248", "025427", "78686786", "76575756", "0786757" };
        for (int i=0;i< tutorNames.Length; i++)
        {
            int id;
            string email = $"{tutorNames[i].Split()[0]}{cellPhones[i]}@gmail.com";
            
            do
                id = s_rand.Next(200000000, 400000000);
            while (s_dalTutor!.Read(id) != null);
            string password = $"{id / 5}{id % 5}";
            //bool? even = (id % 2) == 0 ? true : false;
            //string? alias = even ? name + "ALIAS" : null;
            //DateTime start = new DateTime(1995, 1, 1);
            //DateTime bdt = start.AddDays(s_rand.Next((s_dalConfig.Clock - start).Days));

            //s_dalStudent!.Create(new(id, tutorNames[i], alias, even, bdt));
        }


    }
    private static void createAssignment()
    {

    }

}
