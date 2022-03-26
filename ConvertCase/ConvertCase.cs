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
            string path = @"D:\220112 - aba\4.2.1 REVIT LIBRARY";

            string[] directories = Directory.GetDirectories(path);

            foreach (string dir in directories)
            {
                
                string[] folders = Directory.GetDirectories(dir);

                Rename(dir);

                foreach (string f in folders)
                {
                    Rename(f);
                }
            }


            return Result.Succeeded;
        }

        public static void Rename(string dir)
        {
            string[] files = Helpers.HelpersDirectory.RetrieveFiles(dir);

            foreach (string file in files)
            {
                if (file.Contains(".rfa"))
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
                    newName = newName.Replace("-", " - ").Replace("_", " - ");
                    string parentPath = Directory.GetParent(dir).FullName;
                    string newPath = dir + @"\" + newName + extension;

                    try
                    {
                        // Change file name
                        File.Move(file, newPath);
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
