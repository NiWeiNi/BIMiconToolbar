using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.ExportSchedules
{
    [TransactionAttribute(TransactionMode.Manual)]
    class ExportSchedules : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Warning", "Sorry, the tool is WIP");

            return Result.Succeeded;
        }
    }
}
