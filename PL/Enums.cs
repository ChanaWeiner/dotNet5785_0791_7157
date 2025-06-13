using System.Collections;

namespace PL
{
    public class RolesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Role> s_enums =
    (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class StatusCallsCollection : IEnumerable
    {
        static readonly IEnumerable<BO.CallStatus> s_enums =
    (Enum.GetValues(typeof(BO.CallStatus)) as IEnumerable<BO.CallStatus>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class SubjectsCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Subjects> s_enums =
    (Enum.GetValues(typeof(BO.Subjects)) as IEnumerable<BO.Subjects>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class DistanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class EndOfTreatmentCollection : IEnumerable
    {
        static readonly IEnumerable<BO.EndOfTreatment> s_enums =
    (Enum.GetValues(typeof(BO.EndOfTreatment)) as IEnumerable<BO.EndOfTreatment>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    public class TutorFieldsCollection : IEnumerable
    {
        static readonly IEnumerable<BO.TutorField> s_enums =
    (Enum.GetValues(typeof(BO.TutorField)) as IEnumerable<BO.TutorField>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}