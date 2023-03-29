using Autodesk.Revit.DB;
using System.Linq;
using System.Text.RegularExpressions;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class SheetsViews
    {
        /// <summary>
        /// Method to check if sheet number is in use in the project as sheet numbers must be unique
        /// </summary>
        /// <param name="sheets"></param>
        /// <param name="numberSheet"></param>
        /// <returns></returns>
        public static bool IsSheetNumberUnique(ViewSheet[] sheets, string numberSheet)
        {
            foreach (ViewSheet sheet in sheets)
            {
                if (sheet.SheetNumber == numberSheet)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Method to check if view name is unique
        /// </summary>
        /// <param name="views"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsViewNameUnique(View[] views, string name)
        {
            foreach (View view in views)
            {
                if (view.Name == name)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Check if a string is unique and return a unique name if not
        /// </summary>
        /// <param name="stringNumber"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string UniqueStringNumberSheet(string stringNumber, ViewSheet[] sheets)
        {
            // Check if stringNumber is unique
            if (IsSheetNumberUnique(sheets, stringNumber) == false)
            {
                // Create new unique string
                string uniqueNumber = stringNumber;
                Regex regexLastDigits = new Regex(@"\d+$");
                Regex regexCopy = new Regex(@" - Copy \d+$");

                while (IsSheetNumberUnique(sheets, uniqueNumber) == false)
                {
                    Match lastDigitsMatch = regexLastDigits.Match(uniqueNumber);
                    Match copyMatch = regexCopy.Match(uniqueNumber);

                    if (copyMatch.Success)
                    {
                        string newNumber = (double.Parse(lastDigitsMatch.Value) + 1).ToString();
                        uniqueNumber = Regex.Replace(uniqueNumber, @"\d+$", newNumber);

                    }
                    else
                    {
                        uniqueNumber = uniqueNumber + " - Copy 1";
                    }
                }

                return uniqueNumber;
            }
            else
            {
                return stringNumber;
            }
        }

        /// <summary>
        /// Method to create a unique name for view
        /// </summary>
        /// <param name="stringNumber"></param>
        /// <param name="views"></param>
        /// <returns></returns>
        public static string UniqueStringViewName(string stringNumber, View[] views)
        {
            // Check if stringNumber is unique
            if (IsViewNameUnique(views, stringNumber) == false)
            {
                // Create new unique string
                string uniqueNumber = stringNumber;
                Regex regexLastDigits = new Regex(@"\d+$");
                Regex regexCopy = new Regex(@" - Copy \d+$");

                while (IsViewNameUnique(views, uniqueNumber) == false)
                {
                    Match lastDigitsMatch = regexLastDigits.Match(uniqueNumber);
                    Match copyMatch = regexCopy.Match(uniqueNumber);

                    if (copyMatch.Success)
                    {
                        string newNumber = (double.Parse(lastDigitsMatch.Value) + 1).ToString();
                        uniqueNumber = Regex.Replace(uniqueNumber, @"\d+$", newNumber);
                    }
                    else
                    {
                        uniqueNumber = uniqueNumber + " - Copy 1";
                    }
                }

                return uniqueNumber;
            }
            else
            {
                return stringNumber;
            }
        }
    }
}
