namespace BlApi;

public interface IAdmin
{

    #region Stage 5
    void AddConfigObserver(Action configObserver);
    void RemoveConfigObserver(Action configObserver);
    void AddClockObserver(Action clockObserver);
    void RemoveClockObserver(Action clockObserver);
    #endregion Stage 5

    /// <summary>
    /// Gets the current system clock time.
    /// </summary>
    /// <returns>The current system date and time.</returns>
    public DateTime GetSystemClock();

    /// <summary>
    /// Advances the system clock by a specified time unit.
    /// </summary>
    /// <param name="timeUnit">The time unit by which the clock should be advanced.</param>
    public void AdvanceClock(BO.TimeUnit timeUnit);

    /// <summary>
    /// Gets the configured risk time range for the system.
    /// </summary>
    /// <returns>A <see cref="TimeSpan"/> representing the risk time range.</returns>
    public TimeSpan GetRiskTimeRange();

    /// <summary>
    /// Sets the risk time range for the system.
    /// </summary>
    /// <param name="timeRange">A <see cref="TimeSpan"/> representing the new risk time range.</param>
    public void SetRiskTimeRange(TimeSpan timeRange);

    /// <summary>
    /// Resets the database to its initial state.
    /// </summary>
    public void ResetDatabase();

    /// <summary>
    /// Initializes the database.
    /// </summary>
    public void InitializeDatabase();

 

}
