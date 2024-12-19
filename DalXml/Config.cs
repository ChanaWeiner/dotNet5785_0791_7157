namespace Dal;

internal static class Config
{
    // File name for storing general data configuration values.
    internal const string s_data_config_xml = "data-config.xml";

    // File name for storing student call data.
    internal const string s_studentcalls_xml = "studentcalls.xml";

    // File name for storing tutor data.
    internal const string s_tutors_xml = "tutors.xml";

    // File name for storing assignment data.
    internal const string s_assignments_xml = "assignments.xml";

    /// Gets the next assignment ID and increments it in the configuration file.
    /// This ensures unique IDs for new assignments.
    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    /// Gets the next student call ID and increments it in the configuration file.
    /// This ensures unique IDs for new student calls.
    internal static int NextStudentCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextStudentCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextStudentCallId", value);
    }

    /// Gets or sets the system clock. The clock value is stored in the configuration file.
    /// This allows for centralized management of the current date and time in the system.
    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    
    /// Gets or sets the risk time span for assignments.
    /// This value represents the time span during which assignments are considered at risk.
    internal static TimeSpan RiskTimeSpan
    {
        get => XMLTools.GetConfigTimeSpanVal(s_data_config_xml, "RiskTimeSpan");
        set => XMLTools.SetConfigTimeSpanVal(s_data_config_xml, "RiskTimeSpan", value);
    }
    

    /// Resets all configuration values to their default settings.
    /// - Resets the next assignment ID to 1000.
    /// - Resets the next student call ID to 1000.
    /// - Resets the system clock to the current date and time.
    internal static void Reset()
    {
        NextAssignmentId = 1000;
        NextStudentCallId = 1000;
        Clock = DateTime.Now;
        RiskTimeSpan = TimeSpan.Zero;
    }
}
