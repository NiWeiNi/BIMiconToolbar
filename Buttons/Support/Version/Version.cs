using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMicon.BIMiconToolbar.Support.Version
{
    [TransactionAttribute(TransactionMode.ReadOnly)]
    class Version : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            System.Diagnostics.FileVersionInfo fvi = System.Diagnostics.FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;

            TaskDialog.Show("Version", "Current version is: " + version);

            return Result.Succeeded;
        }
    }
}
