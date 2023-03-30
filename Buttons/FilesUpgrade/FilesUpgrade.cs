using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers.Browser;
using System.Collections.Generic;
using System.IO;

namespace BIMicon.BIMiconToolbar.FilesUpgrade
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FilesUpgrade : IExternalCommand
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

                // Define current application
                Application app = commandData.Application.Application;

                // Files failed to upgrade
                List<string> failedFiles = new List<string>();

                // Folders inside the selected folder
                string[] folders = Directory.GetDirectories(selectedPath, "*", SearchOption.AllDirectories);

                // Retrieve files in each folder
                foreach (string f in folders)
                {
                    // Path of files to be upgraded
                    DirectoryInfo pathdir = new DirectoryInfo(f);

                    // Upgrade each file
                    foreach (FileInfo fi in pathdir.GetFiles())
                    {
                        if (fi.IsReadOnly)
                        {
                            failedFiles.Add(fi.Name);
                        }
                        else if (fi.Name.EndsWith(".rfa") && !fi.Name.Contains("000"))
                        {
                            // Define save file options
                            SaveAsOptions saveAsOptions = new SaveAsOptions();
                            saveAsOptions.OverwriteExistingFile = true;

                            // Open document and save files
                            Document doc = app.OpenDocumentFile(fi.FullName);
                            doc.SaveAs(fi.FullName, saveAsOptions);
                            doc.Close(false);
                        }
                    }
                }

                // Output message
                if (failedFiles.Count > 0)
                {
                    string mes = "The following files have not been upgraded: " + string.Join("\n", failedFiles);
                    TaskDialog.Show("Warning", mes);
                }
                else
                {
                    TaskDialog.Show("Success", "All files have been upgraded.");
                }

                return Result.Succeeded;
            }
        }
    }
}
