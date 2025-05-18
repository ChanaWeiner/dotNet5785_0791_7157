using System.Collections;

namespace PL
{
    internal class RolesCollection : IEnumerable
    {
        static readonly IEnumerable<BO.Role> s_enums =
    (Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }

    internal class DistanceTypeCollection : IEnumerable
    {
        static readonly IEnumerable<BO.DistanceType> s_enums =
    (Enum.GetValues(typeof(BO.DistanceType)) as IEnumerable<BO.DistanceType>)!;

        public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
    }
}