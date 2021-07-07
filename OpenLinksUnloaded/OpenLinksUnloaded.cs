using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.OpenLinksUnloaded
{
    [TransactionAttribute(TransactionMode.Manual)]
    class OpenLinksUnloaded : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;

            using (OpenLinksUnloadedWPF customWindow = new OpenLinksUnloadedWPF())
            {
                // Revit application as window's owner and always in front
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();
            }

            if (false)
            {
                // Create ModelPath to project file
                string modelPathString = @"C:\BIMicon\00-Archive\Open Unlinked.rvt";
                FilePath modelPath = new FilePath(modelPathString);

                string localPathString = @"C:\Users\BIMicon\Documents\Open Unlinked - Links Unloaded.rvt";
                FilePath localPath = new FilePath(localPathString);

                bool isWorkshared = BasicFileInfo.Extract(modelPathString).IsWorkshared;

                if (isWorkshared)
                {
                    TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
                    if (transData.IsTransmitted)
                    {
                        transData.IsTransmitted = false;
                        TransmissionData.WriteTransmissionData(modelPath, transData);
                    }

                    WorksharingUtils.CreateNewLocal(modelPath, localPath);

                    Helpers.RevitDirectories.UnloadRevitLinks(localPath);

                    OpenOptions openOptions = new OpenOptions();

                    uiApp.OpenAndActivateDocument(localPath, openOptions, false);
                }
            }

            return Result.Succeeded;
        }
    }
}
