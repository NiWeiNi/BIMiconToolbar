using Autodesk.Revit.DB;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class UnitsConverter
    {
        /// <summary>
        /// Method to convert length units to Internal Units
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public static double ConvertProjectLengthToInternal(Document doc, double number)
        {
            double internalUnits = 0;
#if v2022 || v2023 || v2024
            ForgeTypeId fTypeId = doc.GetUnits().GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
            internalUnits = UnitUtils.ConvertToInternalUnits(number, fTypeId);
#else
            Units units = doc.GetUnits();
            internalUnits = UnitUtils.ConvertToInternalUnits(number, units.GetFormatOptions(UnitType.UT_Length).DisplayUnits);
#endif
            return internalUnits;
        }
    }
}
