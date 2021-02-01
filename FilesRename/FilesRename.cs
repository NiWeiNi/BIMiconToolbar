using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMiconToolbar.Helpers.Browser;
using System.IO;

namespace BIMiconToolbar.FilesRename
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FilesRename : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (BrowserWindow browserWindow = new BrowserWindow())
            {
                browserWindow.ShowDialog();

                string fullPath = browserWindow.selectedPath;

                // CHeck that path is not empty and path is a folder
                if (fullPath == null || !Directory.Exists(fullPath))
                {
                    TaskDialog.Show("Warning", "No folder has been selected");
                    return Result.Cancelled;
                }
                // Show next window for user input
                else
                {
                    
                }
            }

            return Result.Succeeded;
        }
    }
}
