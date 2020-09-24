using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.NumberDoors
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberDoors2020 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            BuiltInCategory builtInCategory = BuiltInCategory.OST_Doors;

            Helpers.Helpers.numberFamilyInstance(doc, builtInCategory);

            TaskDialog.Show("Number Doors", "Doors numbered");

            return Result.Succeeded;
        }
    }
}
