namespace BlApi;

public interface IAdmin
{
    public DateTime GetSystemClock();
    public void AdvanceClock(BO.TimeUnit timeUnit);
    public TimeSpan GetRiskTimeRange();
    public void SetRiskTimeRange(TimeSpan timeRange);
    public void ResetDatabase();
    public void InitializeDatabase();
}
