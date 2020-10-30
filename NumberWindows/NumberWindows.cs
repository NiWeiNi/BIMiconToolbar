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

            int countInstances = 0;

            Helpers.Helpers.numberFamilyInstance(doc, builtInCategory, ref countInstances);

            TaskDialog.Show("Success", countInstances.ToString() + " windows numbered");

            return Result.Succeeded;
        }
    }
}
