using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class Parsing
    {
        public static bool Contains(string source, string toCheck, StringComparison comp)
        {
            return source != null && toCheck != null && source.IndexOf(toCheck, comp) >= 0;
        }

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
            // Regex to check string is properly formatted as imperial fraction
            Regex rx = new Regex(@"^\d*/*\d*'*\s*\d*""*\s*\d*/*\d*""*");
            Match matchFraction = rx.Match(inputStringNumber);

            if (matchFraction.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to convert imperial fractions to decimal feet
        /// </summary>
        /// <param name="inputStringNumber"></param>
        public static double ImperialFractionToDecimalFeet(string inputStringNumber)
        {
            double feet = 0;

            Regex regexFeet = new Regex(@"\d+'");
            Match matchFeet = regexFeet.Match(inputStringNumber);
            Regex regexFeetFraction = new Regex(@"\d+/\d+'");
            Match matchFeetFraction = regexFeetFraction.Match(inputStringNumber);
            Regex regexInches = new Regex(@"\d+""");
            Match matchInches = regexInches.Match(inputStringNumber);
            Regex regexInchesFraction = new Regex(@"\d+/\d+""");
            Match matchInchesFraction = regexInchesFraction.Match(inputStringNumber);
            Regex regexAfterFracInches = new Regex(@"\d+\s\d+/\d+""");
            Match matchAfterFracInches = regexAfterFracInches.Match(inputStringNumber);
            Regex regexBeforeFracInches = new Regex(@"\d+""\s\d+/\d+");
            Match matchBeforeFracInches = regexBeforeFracInches.Match(inputStringNumber);

            string reducedString = inputStringNumber;

            if (IsImperialFraction(inputStringNumber))
            {
                // Match fraction feet first to avoid single number match
                if (matchFeetFraction.Success)
                {
                    string[] division = matchFeetFraction.Value.Replace("'", "").Split('/');
                    double nb = double.Parse(division[0]) / double.Parse(division[1]);
                    feet += nb;
                    reducedString = inputStringNumber.Replace(matchFeetFraction.Value, "");
                }
                else if (matchFeet.Success)
                {
                    double nb = double.Parse(matchFeet.Value.Replace("'", ""));
                    feet += nb;
                    reducedString = inputStringNumber.Replace(matchFeet.Value, "");
                }
                // Match inches
                if (matchAfterFracInches.Success)
                {
                    // Extract entire part of fraction
                    Regex entire = new Regex(@"\d+\s");
                    Match matchEntire = entire.Match(reducedString);
                    feet += double.Parse(matchEntire.Value) / 12;

                    // Extract fraction
                    Regex fraction = new Regex(@"\d+/\d+""");
                    Match matchFractional = fraction.Match(reducedString);
                    string[] fractionInches = matchFractional.Value.Replace("\"", "").Split('/');

                    feet += (double.Parse(fractionInches[0]) / double.Parse(fractionInches[1])) / 12;
                }
                else if (matchBeforeFracInches.Success)
                {
                    // Extract entire part of fraction
                    Regex entire = new Regex(@"\d+\s");
                    Match matchEntire = entire.Match(reducedString);
                    feet += double.Parse(matchEntire.Value) / 12;

                    // Extract fraction
                    Regex fraction = new Regex(@"\d+/\d+""");
                    Match matchFractional = fraction.Match(reducedString);
                    string[] fractionInches = matchFractional.Value.Replace("\"", "").Split('/');

                    feet += (double.Parse(fractionInches[0]) / double.Parse(fractionInches[1])) / 12;
                }
                else if (matchInchesFraction.Success)
                {
                    string[] division = matchInchesFraction.Value.Replace("\"", "").Split('/');
                    double nb = double.Parse(division[0]) / double.Parse(division[1]) / 12;
                    feet += nb;
                }
                else if (matchInches.Success)
                {
                    double nb = double.Parse(matchInches.Value.Replace("\"", ""));
                    feet += nb / 12;
                }
            }

            return feet;
        }

        /// <summary>
        /// Method to check if string has any alpha characters
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static bool IsAlpha(string inputString)
        {
            Regex rx = new Regex(@"^(\d*)'*[\s|-]*(\d)*[\s|""]* (\d *\/\d *)*");
            Match match = rx.Match(inputString);

            if (match.Success)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function to check if all characters of string is upper
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAllUpper(string input)
        {
            foreach (char c in input)
            {
                if (char.IsLetter(c) && char.IsUpper(c) == false)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Function to change string to Title case
        /// </summary>
        /// <param name="input"></param>
        public static void StringToTitleCase(string input, ref string output)
        {
            // Loop through all characters of string
            for (int i = 0; i < input.Length; i++)
            {
                // Convert char to string for ease of manipulation
                string s = input[i].ToString();

                // Convert to uppercase if the first character is a letter
                if (i == 0 && char.IsLetter(input[i]))
                {
                    output += input[i].ToString().ToUpper();
                    continue;
                }

                // Start a new call to capitalize next delimited string
                if (s == "-" || s == " " || s == "_")
                {
                    output += input[i];
                    StringToTitleCase(input.Substring(i + 1), ref output);
                    break;
                }
                // Convert rest of letter into lower case
                else if (char.IsLetter(input[i]))
                {
                    output += input[i].ToString().ToLower();
                    continue;
                }
                // Add rest characters as they are
                else
                {
                    output += input[i];
                    continue;
                }
            }
        }
    }
}
