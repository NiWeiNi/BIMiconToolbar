using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers.Browser;
using System.IO;

namespace BIMicon.BIMiconToolbar.FilesRename
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FilesRename : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            using (BrowserWindow browserWindow = new BrowserWindow())
            {
                browserWindow.ShowDialog();

                string selectedPath = browserWindow.selectedPath;

                // Check that path is not empty and path is a folder
                if (selectedPath == null || !Directory.Exists(selectedPath))
                {
                    TaskDialog.Show("Warning", "No folder has been selected");
                    return Result.Cancelled;
                }
                // Show next window for user input
                else
                {
                    // Call WPF for user input
                    using (FilesRenameWPF customWindow = new FilesRenameWPF(selectedPath))
                    {
                        // Revit application as window's owner
                        System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow)
                        {
                            Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle
                        };

                        customWindow.ShowDialog();

                        if (customWindow.Canceled)
                        {
                            return Result.Cancelled;
                        }

                        return Result.Succeeded;
                    }
                }
            }
        }
    }
}
