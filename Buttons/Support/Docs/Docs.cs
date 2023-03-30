using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;

namespace BIMicon.BIMiconToolbar.Support.Docs
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class Docs : IExternalCommand
    {
        private readonly string _docsWeb = "https://www.bimicon.com/bimicon-plugin/";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            GeneralHelpers.OpenUri(_docsWeb);
            return Result.Succeeded;
        }
    }
}
