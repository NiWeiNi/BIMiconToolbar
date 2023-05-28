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

        public static string GetProjectUnitsRepresentation(Document doc)
        {
            string unitsRep = null;

#if v2022 || v2023 || v2024
            ForgeTypeId fTypeId = doc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
            //internalUnits = UnitUtils.ConvertToInternalUnits(number, fTypeId);
#else

            DisplayUnitType dUT = ProjectLengthUnit(doc);
            unitsRep = dUT.ToString().Replace("DUT_", "").Replace("_", " ").ToLower();
#endif
            return unitsRep;
        }


        /// <summary>
        /// Method to retrieve length unit in project
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>

        public static dynamic ProjectLengthUnit(Document doc)
        {
#if v2022 || v2023 || v2024
            Units units = doc.GetUnits();
            FormatOptions fo = units.GetFormatOptions(SpecTypeId.Length);
            ForgeTypeId fTypeId = fo.GetUnitTypeId();
            return fTypeId;
#else
            Units units = doc.GetUnits();
            FormatOptions fo = units.GetFormatOptions(UnitType.UT_Length);
            DisplayUnitType dUType = fo.DisplayUnits;
            return dUType;
#endif
        }
    }
}
