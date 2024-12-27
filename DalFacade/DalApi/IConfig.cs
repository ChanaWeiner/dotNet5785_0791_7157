namespace DalApi;

public interface IConfig
{
    DateTime Clock { get; set; }
    TimeSpan RiskTimeSpan { get; set; }

    void Reset();
}
