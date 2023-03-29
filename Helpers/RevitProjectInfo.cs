using Autodesk.Revit.DB;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class RevitProjectInfo
    {
        /// <summary>
        /// Method to check if project unit is metric
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static bool IsProjectUnitMetric(Document doc)
        {
            DisplayUnit unit = doc.DisplayUnitSystem;

            if (unit == DisplayUnit.METRIC)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Method to retrieve length unit in project
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static ForgeTypeId ProjectLengthUnit(Document doc)
        {
            Units units = doc.GetUnits();
            FormatOptions fo = units.GetFormatOptions(SpecTypeId.Length);
            ForgeTypeId fTypeId = fo.GetUnitTypeId();
            return fTypeId;
        }
    }
}
