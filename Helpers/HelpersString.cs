using System.Collections.Generic;

namespace BIMiconToolbar.Helpers
{
    class HelpersString
    {
        /// <summary>
        /// Function to return array of replaced strings
        /// </summary>
        /// <param name="oldStrings"></param>
        /// <param name="oldChar"></param>
        /// <param name="newChar"></param>
        /// <returns></returns>
        public static string[] ReplaceString(string[] oldStrings, string oldChar, string newChar)
        {
            var newStrings = new List<string>();

            // Check input array is not null
            if (oldStrings == null)
            {
                return new string[0];
            }
            // Loop through each string and replace selected strings
            else
            {
                foreach (string oString in oldStrings)
                {
                    var newString = oString.Replace(oldChar, newChar);
                    newStrings.Add(newString);
                }
            }

            return newStrings.ToArray();
        }

    }
}
