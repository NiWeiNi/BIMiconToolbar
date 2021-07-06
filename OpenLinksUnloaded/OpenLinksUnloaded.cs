using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.OpenLinksUnloaded
{
    [TransactionAttribute(TransactionMode.Manual)]
    class OpenLinksUnloaded : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            throw new System.NotImplementedException();
        }
    }
}
