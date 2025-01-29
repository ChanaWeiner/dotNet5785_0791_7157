using BlApi;
namespace BlImplementation;

using DalApi;
using DalTest;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    private static IDal s_dal = Factory.Get; //stage 4

    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        DateTime newTime = ClockManager.Now;

        switch (timeUnit)
        {
            case BO.TimeUnit.Hour:
                ClockManager.UpdateClock(newTime.AddHours(1));
                break;
            case BO.TimeUnit.Minute:
                ClockManager.UpdateClock(newTime.AddMinutes(1));
                break;
            case BO.TimeUnit.Year:
                ClockManager.UpdateClock(newTime.AddYears(1));
                break;
            case BO.TimeUnit.Month:
                ClockManager.UpdateClock(newTime.AddMonths(1));
                break;
            case BO.TimeUnit.Day:
                ClockManager.UpdateClock(newTime.AddDays(1));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(timeUnit), "Invalid time unit.");
        }
    }


    public TimeSpan GetRiskTimeRange() => s_dal.Config.RiskTimeSpan;


    public DateTime GetSystemClock() => ClockManager.Now;


    public void InitializeDatabase()
    {
        Initialization.Do();
    }

    public void ResetDatabase()
    {
        s_dal.ResetDB();
    }

    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        s_dal.Config.RiskTimeSpan = timeRange;

    }
}
