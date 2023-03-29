using Autodesk.Revit.DB;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class UnitsConverter
    {
        /// <summary>
        /// Method to convert length units to Internal Units
        /// </summary>
        /// <param name="number"></param>
        /// <param name="dUT"></param>
        /// <returns></returns>
        public static double LengthUnitToInternal(double number, ForgeTypeId fTypeId)
        {
            return UnitUtils.ConvertToInternalUnits(number, fTypeId);
        }
    }
}
