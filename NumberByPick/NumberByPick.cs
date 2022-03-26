using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.NumberByPick
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberByPick : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // Call WPF for user input
            using (NumberByPickWPF customWindow = new NumberByPickWPF())
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                customWindow.ShowDialog();

                return Result.Succeeded;
            }
        }
    }
}
