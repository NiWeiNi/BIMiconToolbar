using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.FloorFinish
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FloorFinish : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            return Result.Succeeded;
        }
    }
}
