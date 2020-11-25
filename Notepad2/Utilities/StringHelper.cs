using System.Text.RegularExpressions;

namespace SharpPad.Utilities
{
    public static class StringHelper
    {
        /// <summary>
        /// Replaces multiple spaces with a single space.
        /// where dots are spaces, '.....' or '.........' becomes '.'
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string CollapseSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }

        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrEmpty(value);
        }
    }
}
