using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;

namespace BIMicon.BIMiconToolbar.NumberDoors
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberDoors2020 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Check document is not a family document
            if (RevitDocument.IsDocumentNotProjectDoc(doc))
            {
                return Result.Failed;
            }
            else
            {
                BuiltInCategory builtInCategory = BuiltInCategory.OST_Doors;

                // Call WPF for user input
                using (NumberDoorsWPF customWindow = new NumberDoorsWPF(commandData))
                {
                    // Revit application as window's owner
                    System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                    helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                    customWindow.ShowDialog();

                    // Retrieve user input
                    Phase phase = customWindow.SelectedComboItemPhase.Tag as Phase;
                    Parameter parameter = null;
                    if (customWindow.SelectedComboItemParameters != null)
                    {
                       parameter = customWindow.SelectedComboItemParameters.Tag as Parameter;
                    }
                    else
                    {
                        TaskDialog.Show("Error", "Please create doors in the project first");
                        return Result.Failed;
                    }
                    bool numeric = customWindow.optNumeric;
                    string separator = customWindow.Separator;

                    // Count doors renumbered
                    int countInstances = 0;
                    if (separator == null)
                    {
                        return Result.Cancelled;
                    }
                    else
                    {
                        GeneralHelpers.numberFamilyInstance(doc, phase, numeric, separator, builtInCategory, ref countInstances, parameter);
                    }

                    // Display result to user if any door was numbered
                    if (countInstances > 0)
                    {
                        TaskDialog.Show("Success", countInstances.ToString() + " doors numbered");
                        return Result.Succeeded;
                    }
                    else
                    {
                        TaskDialog.Show("Warning", "No doors numbered");
                        return Result.Failed;
                    }
                }
            }
        }
    }
}
