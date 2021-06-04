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

        /// <summary>
        /// Method to check if a string is formatted as imperial fraction
        /// </summary>
        /// <param name="inputStringNumber"></param>
        /// <returns></returns>
        public static bool IsImperialFraction(string inputStringNumber)
        {
            // Regex to check string only has digits, ", ', \s, and /
            Regex regexImperial = new Regex(@"^[\d""'\/\s-]*$");
            Match matchIsImperialFraction = regexImperial.Match(inputStringNumber);

            if (matchIsImperialFraction.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to convert imperial fractions to decimal feet
        /// </summary>
        /// <param name="inputStringNumber"></param>
        public static void ImperialFractionToDecimalFeet(string inputStringNumber)
        {
            // Regex to check string is properly formatted
            Regex rx = new Regex(@"^(\d*)'*[\s|-]*(\d)*[\s|""]* (\d *\/\d *)*");
            Match matchFraction = rx.Match(inputStringNumber);
            double feet = 0;

            if (IsImperialFraction(inputStringNumber))
            {
                GroupCollection matchGroups = matchFraction.Groups;

                if (matchGroups[1].Success)
                {
                    string stringGroupOne = matchGroups[1].Value;
                    double groupOne = double.Parse(stringGroupOne);
                    feet += groupOne;
                }
                if (matchGroups[2].Success)
                {
                    string stringGroupTwo = matchGroups[2].Value;
                    double groupTwo = double.Parse(stringGroupTwo);
                    feet += groupTwo / 12;
                }
                if (matchGroups[3].Success)
                {
                    string stringGroupThree = matchGroups[3].Value;
                    double groupThree = double.Parse(stringGroupThree);
                    feet += groupThree;
                }
            }
        }
    }
}
