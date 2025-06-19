using System.Runtime.CompilerServices;
using DalApi;
namespace Dal;

internal class ConfigImplementation : IConfig
{
    /// <summary>
    /// Gets or sets the current clock time.
    /// </summary>
    public DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get => Config.Clock;
        [MethodImpl(MethodImplOptions.Synchronized)]

        set => Config.Clock = value;
    }

    /// <summary>
    /// Gets or sets the current risk time span.
    /// </summary>
    public TimeSpan RiskTimeSpan
    {
        [MethodImpl(MethodImplOptions.Synchronized)]

        get => Config.RiskTimeSpan;
        [MethodImpl(MethodImplOptions.Synchronized)]

        set => Config.RiskTimeSpan = value;
    }

    /// <summary>
    /// Resets the configuration values to their default states.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)]
    public void Reset()
    {
        Config.Reset();
    }
}