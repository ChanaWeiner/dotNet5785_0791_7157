using BlApi;
using DalApi;
using DalTest;
using Helpers;


namespace BlImplementation;

/// <summary>
/// Implementation of administrative actions in the system.
/// </summary>
internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;


    #region Stage 5
    public void AddClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers += clockObserver;
    public void RemoveClockObserver(Action clockObserver) =>
    AdminManager.ClockUpdatedObservers -= clockObserver;
    public void AddConfigObserver(Action configObserver) =>
   AdminManager.ConfigUpdatedObservers += configObserver;
    public void RemoveConfigObserver(Action configObserver) =>
    AdminManager.ConfigUpdatedObservers -= configObserver;
    #endregion Stage 5


    /// <summary>
    /// Advances the system clock by a specified time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit by which the clock should be advanced.</param>
    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        DateTime newTime = AdminManager.Now;

        switch (timeUnit)
        {
            case BO.TimeUnit.Hour:
                AdminManager.UpdateClock(newTime.AddHours(1));
                break;
            case BO.TimeUnit.Minute:
                AdminManager.UpdateClock(newTime.AddMinutes(1));
                break;
            case BO.TimeUnit.Year:
                AdminManager.UpdateClock(newTime.AddYears(1));
                break;
            case BO.TimeUnit.Month:
                AdminManager.UpdateClock(newTime.AddMonths(1));
                break;
            case BO.TimeUnit.Day:
                AdminManager.UpdateClock(newTime.AddDays(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit.");
        }
    }

    /// <summary>
    /// Gets the configured risk time range for the system.
    /// </summary>
    /// <returns>A <see cref="TimeSpan"/> representing the risk time range.</returns>
    public TimeSpan GetRiskTimeRange() => AdminManager.RiskTimeSpan;

    /// <summary>
    /// Gets the current system clock time.
    /// </summary>
    /// <returns>The current system date and time.</returns>
    public DateTime GetSystemClock() => AdminManager.Now;

    /// <summary>
    /// Initializes the database.
    /// </summary>
    public void InitializeDatabase()
    {
        AdminManager.InitializeDatabase();
    }

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    public void ResetDatabase()
    {
        _dal.ResetDB();

    }

    /// <summary>
    /// Sets the risk time range for the system.
    /// </summary>
    /// <param name="timeRange">A <see cref="TimeSpan"/> representing the new risk time range.</param>
    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        AdminManager.RiskTimeSpan = timeRange;
    }


}

