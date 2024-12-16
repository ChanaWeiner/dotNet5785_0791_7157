

namespace Dal;

internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
    internal const string s_studentcalls_xml = "studentcalls.xml";
    internal const string s_tutors_xml = "tutors.xml";
    internal const string s_assignments_xml = "assignments.xml";

    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    internal static int NextStudentCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextStudentCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextStudentCallId", value);
    }


    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }

    internal static void Reset()
    {
        NextAssignmentId = 1000;
        NextStudentCallId = 1000;
        Clock = DateTime.Now;
    }

}
