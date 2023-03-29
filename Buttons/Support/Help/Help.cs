using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.Support.Help
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class Help : IExternalCommand
    {
        private readonly string _helpWeb = "https://www.bimicon.com/contact/";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Helpers.Helpers.OpenUri(_helpWeb);
            return Result.Succeeded;
        }
    }
}
