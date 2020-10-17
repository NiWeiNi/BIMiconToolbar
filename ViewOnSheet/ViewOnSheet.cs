using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows;

namespace BIMiconToolbar.ViewOnSheet
{
    [TransactionAttribute(TransactionMode.Manual)]
    class ViewOnSheet : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Retrieve active view
            View activeView = doc.ActiveView;
            ViewType activeViewType = activeView.ViewType;
            if (activeViewType == ViewType.DrawingSheet)
            {
                TaskDialog.Show("Error", "Current view is a sheet. Please open a view");
                return Result.Succeeded;
            }
            MessageBox.Show(activeView.ViewType.ToString());

            using (ViewSheetsWindow VOSwindow = new ViewSheetsWindow(commandData))
            {
                // the following two lines let the Revit application be this window's owner, so that it will always be on top of the Revit screen even when the user tries to switch the current screen.
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(VOSwindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                //VOSwindow.ResizeMode = ResizeMode.NoResize;
                VOSwindow.ShowDialog();

                // List with all selected sheets
                List<int> intIds = VOSwindow.listIds;

                // Check if view is already on sheets
                bool isViewOnSheet = Helpers.Helpers.IsViewOnSheet(doc, activeView);

                // Transaction
                Transaction transactionViews = new Transaction(doc, "Place Views on Sheets");
                transactionViews.Start();

                if (intIds.Count > 1) //&& isViewOnSheet == false)
                {
                    // Place view on selected sheets
                    foreach (var item in intIds)
                    {
                        ElementId eId = new ElementId(item);
                        ViewSheet sheet = doc.GetElement(eId) as ViewSheet;

                        if (isViewOnSheet)
                        {
                            ElementId viewId = activeView.Duplicate(ViewDuplicateOption.WithDetailing);

                            Viewport.Create(doc, sheet.Id, viewId, new XYZ());
                        }
                        else
                        {
                            Viewport.Create(doc, sheet.Id, activeView.Id, new XYZ());
                        }
                        

                    }
                }

                // Commit transaction
                transactionViews.Commit();


                string prompt = "";
                switch (activeView.ViewType)
                {
                    case ViewType.CostReport:
                        prompt += "a cost report view.";
                        break;
                    case ViewType.Internal:
                        prompt += "Revit's internal view.";
                        break;
                    case ViewType.Legend:
                        prompt += "a legend view.";
                        break;
                    case ViewType.LoadsReport: // cannot be placed on sheets
                        prompt += "a loads report view.";
                        break;
                    case ViewType.PanelSchedule: // schedules
                        prompt += "a panel schedule view.";
                        break;
                    case ViewType.PresureLossReport: 
                        prompt += "a pressure loss report view.";
                        break;
                    case ViewType.Report:
                        prompt += "a report view.";
                        break;
                    case ViewType.Schedule:
                        prompt += "a schedule view.";
                        break;
                    case ViewType.Undefined:
                        prompt += "an undefined/unspecified view.";
                        break;
                    case ViewType.Walkthrough:
                        prompt += "a walkthrough view.";
                        break;
                    default:
                        // All views that cannot be placed more than once on sheets


                        break;
                }
            }
            
            return Result.Succeeded;
        }
    }
}
