using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.NumberWindows
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberWindows : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            BuiltInCategory builtInCategory = BuiltInCategory.OST_Windows;

            Helpers.Helpers.numberFamilyInstance(doc, builtInCategory);

            TaskDialog.Show("Number Windows", "Windows numbered");

            return Result.Succeeded;
        }
    }
}
