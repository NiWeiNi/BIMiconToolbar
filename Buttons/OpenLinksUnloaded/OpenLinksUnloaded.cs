using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMicon.BIMiconToolbar.OpenLinksUnloaded
{
    [TransactionAttribute(TransactionMode.Manual)]
    class OpenLinksUnloaded : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;

            using (OpenLinksUnloadedWPF customWindow = new OpenLinksUnloadedWPF(uiApp))
            {
                // Revit application as window's owner and always in front
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();
            }

            return Result.Succeeded;
        }
    }
}
