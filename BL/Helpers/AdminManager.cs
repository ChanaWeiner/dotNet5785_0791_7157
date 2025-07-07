using BlImplementation;
using BO;
using DalApi;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
namespace Helpers;

/// <summary>
/// Internal BL manager for all Application's Clock logic policies
/// </summary>
internal static class AdminManager//stage 4
{
    #region Stage 4
    private static readonly DalApi.IDal s_dal = DalApi.Factory.Get; //stage 4
    /// <summary>
    /// Property for providing/setting current configuration variable value for any BL class that may need it
    /// </summary>
    internal static TimeSpan RiskTimeSpan
    {
        get
        {
            lock (BlMutex)
                return s_dal.Config.RiskTimeSpan;
        }
        set
        {

            lock (BlMutex) // Stage 7
                s_dal.Config.RiskTimeSpan = value;
            ConfigUpdatedObservers?.Invoke(); // stage 5
            StudentCallManager.Observers.NotifyListUpdated();
        }
    }

    /// <summary>
    /// Property for providing current application's clock value for any BL class that may need it
    /// </summary>
    internal static DateTime Now { get => s_dal.Config.Clock; } //stage 4
    internal static void ResetDB()
    {
        lock (BlMutex) // Stage 7
        {
            s_dal.ResetDB();
            UpdateClock(Now); // Stage 5
            RiskTimeSpan = RiskTimeSpan; // Stage 5 – refresh value
        }
    }
    internal static void InitializeDB()
    {
        lock (BlMutex)
        {
            DalTest.Initialization.Do();
            UpdateClock(Now); // Stage 5
            RiskTimeSpan = RiskTimeSpan; // Stage 5 – refresh value
        }
    }
    private static Task? periodicTask = null;
    /// <summary>
    /// Method to perform application's clock from any BL class as may be required
    /// </summary>
    /// <param name="newClock">updated clock value</param>

    public static void UpdateClock(DateTime newClock) // prepared for stage 7 as DRY to eliminate needless repetition
    {
        DateTime oldClock;
        lock (BlMutex)
        {
            oldClock = s_dal.Config.Clock; //stage 4
            s_dal.Config.Clock = newClock; //stage 4

        }

        if (periodicTask is null || periodicTask.IsCompleted) //stage 7
            periodicTask = Task.Run(() => StudentCallManager.PeriodicStudentcallStatusUpdates(oldClock, newClock));

        ClockUpdatedObservers?.Invoke(); //prepared for stage 5
    }

    #endregion Stage 4

    #region Stage 5
    internal static event Action? ConfigUpdatedObservers; //prepared for stage 5 - for config update observers
    internal static event Action? ClockUpdatedObservers; //prepared for stage 5 - for clock update observers
    #endregion Stage 5


    #region Stage 7 base
    /// <summary>
    /// Mutex for mutual exclusion while simulator is running
    /// </summary>
    internal static readonly object BlMutex = new();

    /// <summary>
    /// The thread of the simulator
    /// </summary>
    private static Thread? s_thread;
    /// <summary>
    /// The interval for clock updating (in minutes)
    /// </summary>
    private static int s_interval = 1; //in minutes by second    
    /// <summary>
    /// Stop flag for simulator
    /// </summary>
    private static volatile bool s_stop = false;
    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void ThrowOnSimulatorIsRunning()
    {
        if (s_thread is not null)
            throw new BO.BLTemporaryNotAvailableException("Cannot perform the operation since Simulator is running");
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal static void Start(int interval)
    {

        if (s_thread is null)
        {
            s_interval = interval;
            s_stop = false;
            s_thread = new Thread(clockRunner) { Name = "ClockRunner" };
            s_thread.Start();
        }
    }
    [MethodImpl(MethodImplOptions.Synchronized)]

    internal static void Stop()
    {
        if (s_thread is not null)
        {
            s_stop = true;
            s_thread?.Interrupt();
            s_thread.Name = "ClockRunner stopped";
            s_thread = null;
        }
    }
    private static Task? simulateTask = null;


    private static void clockRunner()
    {
        while (!s_stop)
        {
            UpdateClock(Now.AddMinutes(s_interval));

            if (simulateTask is null || simulateTask.IsCompleted)
                simulateTask = Task.Run(() => TutorManager.TutorSimulator());

            try
            {
                Thread.Sleep(1000); // 1 second
            }
            catch (ThreadInterruptedException) { }
        }
    }
    #endregion Stage 7 base
}