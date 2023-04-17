using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMicon.BIMiconToolbar.FloorFinish
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FloorFinish : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            FloorFinishWPF customWindow = new FloorFinishWPF(commandData);
            customWindow.ShowDialog();

            return Result.Succeeded;
        }
    }
}
