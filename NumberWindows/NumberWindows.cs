using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMiconToolbar.NumberWindows
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberWindows : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            BuiltInCategory builtInCategory = BuiltInCategory.OST_Windows;

            // Call WPF for user input
            using (NumberWindowsWPF customWindow = new NumberWindowsWPF(commandData))
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();

                // Retrieve user input
                Phase phase = customWindow.SelectedComboItemPhase.Tag as Phase;
                bool numeric = customWindow.optNumeric;
                string separator = customWindow.Separator;

                // Count windows renumbered
                int countInstances = 0;
                if (separator == null)
                {
                    return Result.Cancelled;
                }
                else
                {
                    Helpers.Helpers.numberFamilyInstance(doc, phase, numeric, separator, builtInCategory, ref countInstances);
                }

                // Display result to user if any window was numbered
                if (countInstances > 0)
                {
                    TaskDialog.Show("Success", countInstances.ToString() + " windows numbered");
                    return Result.Succeeded;
                }
                else
                {
                    TaskDialog.Show("Warning", "No windows numbered");
                    return Result.Failed;
                }
            }
        }
    }
}
