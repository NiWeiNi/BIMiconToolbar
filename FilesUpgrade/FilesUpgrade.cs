using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace BIMiconToolbar.FilesUpgrade
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FilesUpgrade : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Define current application
            Application app = commandData.Application.Application;

            // Path of files to be upgraded
            string path = @"C:\Users\BIMicon\Desktop\upgradeTest";
            DirectoryInfo pathdir = new DirectoryInfo(path);

            // Upgrade each file
            foreach (FileInfo fi in pathdir.GetFiles())
            {
                // Define save file options
                SaveAsOptions saveAsOptions = new SaveAsOptions();
                saveAsOptions.OverwriteExistingFile = true;

                // Open document and save files
                Document doc = app.OpenDocumentFile(fi.FullName);
                doc.SaveAs(fi.FullName, saveAsOptions);
                doc.Close(false);
            }

            return Result.Succeeded;
        }
    }
}
