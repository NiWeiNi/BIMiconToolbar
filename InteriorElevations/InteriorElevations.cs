using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.InteriorElevations
{
    [TransactionAttribute(TransactionMode.Manual)]
    class InteriorElevations : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            TaskDialog.Show("Warning", "Sorry, the tool is WIP");

            return Result.Succeeded;
        }
    }
}
