using DalApi;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock time.
    /// This property retrieves or updates the system clock, which is managed centrally.
    /// </summary>
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the current risk time span.
    /// This property retrieves or updates the time span during which assignments are considered at risk.
    /// </summary>
    public TimeSpan RiskTimeSpan
    {
        get => Config.RiskTimeSpan;
        set => Config.RiskTimeSpan = value;
    }

    /// <summary>
    /// Resets the configuration values to their default state.
    /// Calls the Reset method from the Config class to reset all configuration settings.
    /// </summary>
    public void Reset()
    {
        Config.Reset();
    }
}
