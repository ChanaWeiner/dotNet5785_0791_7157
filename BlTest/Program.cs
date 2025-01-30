using System;
using System.Linq;
using BlApi;
using BO;

class Program
{
    // Initialize the BL interface through the Factory class
    static readonly IBl s_bl = BlApi.Factory.Get();

    static void Main(string[] args)
    {
        // Main loop displaying the main menu to the user
        bool exit = false;
        while (!exit)
        {
            
            Console.WriteLine("Select the service entity:");
            Console.WriteLine("1. Tutor Management");
            Console.WriteLine("2. Call Management");
            Console.WriteLine("3. System Management");
            Console.WriteLine("4. Exit");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    TutorMenu(); break; // Go to Tutor Management Menu
                case "2":
                    StudentCallMenu(); break; // Go to Call Management Menu
                case "3":
                    AdminMenu(); break; // Go to System Management Menu
                case "4":
                    exit = true; break; // Exit the application
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Tutor Management Menu
    private static void TutorMenu()
    {
        bool exit = false;
        while (!exit)
        {
            
            Console.WriteLine("Tutor Management Menu:");
            Console.WriteLine("1. Read Tutor");
            Console.WriteLine("2. Update Tutor");
            Console.WriteLine("3. Delete Tutor");
            Console.WriteLine("4. Exit");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ReadTutor(); break;
                case "2":
                    UpdateTutor(); break;
                case "3":
                    DeleteTutor(); break;
                case "4":
                    exit = true; break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Read Tutor details
    private static void ReadTutor()
    {
        Console.Write("Enter Tutor ID: ");
        if (int.TryParse(Console.ReadLine(), out int tutorId))
        {
            try
            {
                BO.Tutor tutor = s_bl.Tutor.Read(tutorId);
                Console.WriteLine(tutor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Tutor ID.");
        }
    }

    // Update Tutor details
    private static void UpdateTutor()
    {
        Console.Write("Enter Tutor ID for update: ");
        if (int.TryParse(Console.ReadLine(), out int tutorId))
        {
            Console.Write("Enter new name: ");
            string? name = Console.ReadLine();

            Console.Write("Enter new password: ");
            string? password = Console.ReadLine();

            var tutor = new BO.Tutor
            {
                Id = tutorId,
                FullName = name,
                Password = password
            };

            try
            {
                s_bl.Tutor.Update(tutorId, tutor);
                Console.WriteLine("Tutor updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Tutor ID.");
        }
    }

    // Delete Tutor details
    private static void DeleteTutor()
    {
        Console.Write("Enter Tutor ID for deletion: ");
        if (int.TryParse(Console.ReadLine(), out int tutorId))
        {
            try
            {
                s_bl.Tutor.Delete(tutorId);
                Console.WriteLine("Tutor deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Tutor ID.");
        }
    }

    // Call Management Menu
    private static void StudentCallMenu()
    {
        bool exit = false;
        while (!exit)
        {
            
            Console.WriteLine("Call Management Menu:");
            Console.WriteLine("1. Read Call");
            Console.WriteLine("2. Update Call");
            Console.WriteLine("3. Delete Call");
            Console.WriteLine("4. Exit");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ReadStudentCall(); break;
                case "2":
                    UpdateStudentCall(); break;
                case "3":
                    DeleteStudentCall(); break;
                case "4":
                    exit = true; break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Read Call details
    private static void ReadStudentCall()
    {
        Console.Write("Enter Call ID: ");
        if (int.TryParse(Console.ReadLine(), out int callId))
        {
            try
            {
                var call = s_bl.StudentCall.Read(callId);
                Console.WriteLine(call);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Call ID.");
        }
    }

    // Update Call details
    private static void UpdateStudentCall()
    {
        Console.Write("Enter Call ID for update: ");
        if (int.TryParse(Console.ReadLine(), out int callId))
        {
            Console.Write("Enter new address: ");
            string newAddress = Console.ReadLine();

            var call = new BO.StudentCall
            {
                Id = callId,
                FullAddress = newAddress
            };

            try
            {
                s_bl.StudentCall.Update(call);
                Console.WriteLine("Call updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Call ID.");
        }
    }

    // Delete Call details
    private static void DeleteStudentCall()
    {
        Console.Write("Enter Call ID for deletion: ");
        if (int.TryParse(Console.ReadLine(), out int callId))
        {
            try
            {
                s_bl.StudentCall.Delete(callId);
                Console.WriteLine("Call deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid Call ID.");
        }
    }

    // System Management Menu
    private static void AdminMenu()
    {
        bool exit = false;
        while (!exit)
        {
            
            Console.WriteLine("System Management Menu:");
            Console.WriteLine("1. Get System Clock");
            Console.WriteLine("2. Advance Clock");
            Console.WriteLine("3. Reset Database");
            Console.WriteLine("4. Exit");
            Console.Write("Enter choice: ");

            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    GetSystemClock(); break;
                case "2":
                    AdvanceSystemClock(); break;
                case "3":
                    ResetDatabase(); break;
                case "4":
                    exit = true; break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    // Get the current system clock
    private static void GetSystemClock()
    {
        var clock = s_bl.Admin.GetSystemClock();
        Console.WriteLine($"System Clock: {clock}");
    }

    // Advance the system clock
    private static void AdvanceSystemClock()
    {
        Console.WriteLine("Select time unit (HOUR, MINUTE, DAY, etc.):");
        var input = Console.ReadLine();

        if (Enum.TryParse(input, out BO.TimeUnit timeUnit))
        {
            try
            {
                s_bl.Admin.AdvanceClock(timeUnit);
                Console.WriteLine("Clock advanced successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine("Invalid time unit.");
        }
    }

    // Reset the database
    private static void ResetDatabase()
    {
        try
        {
            s_bl.Admin.ResetDatabase();
            Console.WriteLine("Database reset successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
