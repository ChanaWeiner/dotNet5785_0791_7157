using BlApi;
namespace BlImplementation;
using Helpers;

internal class AdminImplementation : IAdmin
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;
    public void AdvanceClock(BO.TimeUnit timeUnit)
    {
        throw new NotImplementedException();
    }

    public TimeSpan GetRiskTimeRange()
    {
        throw new NotImplementedException();
    }

    public DateTime GetSystemClock()
    {
        throw new NotImplementedException();
    }

    public void InitializeDatabase()
    {
        throw new NotImplementedException();
    }

    public void ResetDatabase()
    {
        throw new NotImplementedException();
    }

    public void SetRiskTimeRange(TimeSpan timeRange)
    {
        throw new NotImplementedException();
    }
}
