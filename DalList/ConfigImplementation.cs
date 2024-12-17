using DalApi;

namespace Dal;

internal class ConfigImplementation : IConfig
{
    /// Gets or sets the current clock time.
    public DateTime Clock
    {
        get => Config.Clock;
        set => Config.Clock = value;
    }

    /// Resets the configuration values to their default states.
    public void Reset()
    {
        Config.Reset();
    }
}

