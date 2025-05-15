using System.Collections;

namespace PL;
internal class SemestersCollection : IEnumerable
{
    static readonly IEnumerable<BO.Role> s_enums =
(Enum.GetValues(typeof(BO.Role)) as IEnumerable<BO.Role>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}
