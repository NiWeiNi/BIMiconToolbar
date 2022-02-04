using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.IO;

namespace BIMiconToolbar.ConvertCase
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class ConvertCase : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Selected path
            string path = @"C:\Users\BIMicon\Desktop\Title Test\LIGHTING";

            string[] files = Helpers.HelpersDirectory.RetrieveFiles(path);

            foreach (string file in files)
            {
                // Extract file name from path
                DirectoryInfo pathFile = new DirectoryInfo(file);
                string nameWithExtension = pathFile.Name;
                string extension = Path.GetExtension(file);
                string name = nameWithExtension.Substring(0, nameWithExtension.Length - extension.Length);

                // Convert string to Title case and store new name
                string newName = "";
                Helpers.Parsing.StringToTitleCase(name, ref newName);

                // New file path
                string parentPath = Directory.GetParent(path).FullName;
                string newPath = path + @"\" + newName + extension;

                // Change file name
                File.Move(file, newPath);
            }

            return Result.Succeeded;
        }
    }
}
