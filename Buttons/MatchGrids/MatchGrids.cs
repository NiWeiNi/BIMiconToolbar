using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;

namespace BIMicon.BIMiconToolbar.MatchGrids
{
    [TransactionAttribute(TransactionMode.Manual)]
    class MatchGrids : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Check document is not a family document
            if (RevitDocument.IsDocumentNotProjectDoc(doc)) 
                return Result.Failed;
            // Continue as document is a project
            else if (true)
            {
                // Prompt window to collect user input
                MatchGridsView customWindow = new MatchGridsView(commandData);
                customWindow.ShowDialog();

                return Result.Succeeded;
            }
        }
    }
}
