partial class Program
{
    private static void Main(string[] args)
    {
        Welcome0791();
        Welcome7157();
        Console.ReadKey();

    }

    private static void Welcome0791()
    {
        Console.WriteLine("Enter your name:");
        string name = Console.ReadLine();
        Console.WriteLine(name + ", welcome to my console application");
    }
    static partial void Welcome7157();
   
}