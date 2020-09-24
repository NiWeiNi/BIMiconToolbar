using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMiconToolbar.Helpers.Browser;
using System.Windows;

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
                browserWindow.ResizeMode = ResizeMode.NoResize;
                browserWindow.ShowDialog();

                return Result.Succeeded;
            }
        }
    }
}
