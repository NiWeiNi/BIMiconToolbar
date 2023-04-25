using System.Collections;

namespace BIMicon.BIMiconToolbar.WPF.Models
{
    internal static class ListExtensions
    {
        internal static object Last(this IList list)
        {
            if (list.Count < 1)
            {
                return null;
            }
            return list[list.Count - 1];
        }

        internal static object First(this IList list)
        {
            if (list.Count < 1)
            {
                return null;
            }
            return list[0];
        }
    }
}
