using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BIMiconToolbar.Helpers
{
    class Parsing
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

        /// <summary>
        /// Method to remove forbidden characters for directories and files in a string
        /// </summary>
        /// <param name="stringCheck"></param>
        /// <returns></returns>
        public static string RemoveForbiddenChars(string stringCheck)
        {
            string textDisplay = "";

            // Check for forbidden characters
            var regex = new Regex(@"[/\\:*?<>\|""]");
            bool match = regex.IsMatch(stringCheck);

            // If there is a match, check all possible matches and clean the string from forbidden characters
            if (match)
            {
                foreach (Match m in regex.Matches(stringCheck))
                {
                    string specialChars = m.Value;

                    if (stringCheck != null && stringCheck != "" && specialChars != "")
                    {
                        textDisplay = stringCheck.Replace(specialChars, "");

                        stringCheck = textDisplay;
                    }
                }
            }
            else
            {
                textDisplay = stringCheck;
            }

            return textDisplay;
        }

    }
}
