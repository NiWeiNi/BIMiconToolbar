using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMiconToolbar.Helpers.Browser;
using System.IO;

namespace BIMiconToolbar.RemoveBackups
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class RemoveBackups : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (BrowserWindow browserWindow = new BrowserWindow())
            {
                // the following two lines let the Revit application be this window's owner, so that it will always be on top of the Revit screen even when the user tries to switch the current screen.
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(browserWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                browserWindow.ShowDialog();

                // Variables
                string fullPath = browserWindow.selectedPath;
                int countBackups = 0;

                // CHeck that path is not empty and path is a folder
                if (fullPath == null || !Directory.Exists(fullPath))
                {
                    TaskDialog.Show("Warning", "No folder has been selected");
                    return Result.Cancelled;
                }
                else
                {
                    // Remove backups
                    Helpers.RemoveBackupHelpers.DeleteBackups(fullPath, ref countBackups);
                    // Check number of files deleted
                    if (countBackups > 0)
                    {
                        TaskDialog.Show("Remove Backup Files", countBackups.ToString() + " backup files have been deleted.");
                    }
                    else
                    {
                        TaskDialog.Show("Warning", "No files have been found");
                    }

                    return Result.Succeeded;
                }
            }
        }
    }
}
