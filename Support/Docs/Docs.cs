using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.Support.Docs
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class Docs : IExternalCommand
    {
        private readonly string _docsWeb = "https://www.bimicon.com/bimicon-plugin/";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Helpers.Helpers.OpenUri(_docsWeb);
            return Result.Succeeded;
        }
    }
}
