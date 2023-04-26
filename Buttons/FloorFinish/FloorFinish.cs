using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Buttons.FloorFinish;

namespace BIMicon.BIMiconToolbar.FloorFinish
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FloorFinish : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            WPFTest customWindow = new WPFTest();
            customWindow.ShowDialog();

            return Result.Succeeded;
        }
    }
}
