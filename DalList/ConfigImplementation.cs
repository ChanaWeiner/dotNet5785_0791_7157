using DalApi;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock time.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the current risk time span.
    /// </summary>
    public TimeSpan RiskTimeSpan
    {
        get => Config.RiskTimeSpan;
        set => Config.RiskTimeSpan = value;
    }

    /// <summary>
    /// Resets the configuration values to their default states.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}