using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.ViewOnSheet
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
                TaskDialog.Show("Error", "Current view is a sheet. Please open a view.");
                return Result.Succeeded;
            }

            using (ViewSheetsWindow VOSwindow = new ViewSheetsWindow(commandData))
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(VOSwindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                VOSwindow.ShowDialog();

                // List with all selected sheets
                List<int> intIds = VOSwindow.listIds;

                // Check if view is already on sheets
                bool isViewOnSheet = GeneralHelpers.IsViewOnSheet(doc, activeView);
                bool viewPlaced = false;

                // List to store views placed on sheet
                var viewSheetSuccess = new List<string>();

                // Transaction
                Transaction transactionViews = new Transaction(doc, "Place Views on Sheets");
                transactionViews.Start();

                // Place view on selected sheets
                foreach (var item in intIds)
                {
                    ElementId eId = new ElementId(item);
                    ViewSheet sheet = doc.GetElement(eId) as ViewSheet;

                    // Set view placement point
                    XYZ centerTitleBlock;

                    try
                    {
                        // Retrieve title block
                        FamilyInstance tBlock = new FilteredElementCollector(doc, sheet.Id)
                                                .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                .FirstElement() as FamilyInstance;

                        // Retrieve title block size
                        double sheetHeight = tBlock.get_Parameter(BuiltInParameter.SHEET_HEIGHT).AsDouble();
                        double sheetWidth = tBlock.get_Parameter(BuiltInParameter.SHEET_WIDTH).AsDouble();

                        // Center of title block
                        centerTitleBlock = new XYZ(sheetWidth / 2, sheetHeight / 2, 0);
                    }
                    catch
                    {
                        centerTitleBlock = new XYZ();
                        // TODO: Error log
                    }

                    // If activeView is a schedule
                    if (activeViewType == ViewType.Schedule ||
                        activeViewType == ViewType.ColumnSchedule ||
                        activeViewType == ViewType.PanelSchedule)
                    {
                        ScheduleSheetInstance.Create(doc, sheet.Id, activeView.Id, centerTitleBlock);
                        // Mark sheet as successfully placed
                        viewSheetSuccess.Add(sheet.SheetNumber);
                    }
                    // View is a legend
                    else if (activeViewType == ViewType.Legend)
                    {
                        // Place legend if it is not placed on any sheet
                        if (isViewOnSheet == false)
                        {
                            Viewport.Create(doc, sheet.Id, activeView.Id, centerTitleBlock);
                            // Mark sheet as successfully placed
                            viewSheetSuccess.Add(sheet.SheetNumber);
                        }
                        // Check where is the legend placed on
                        else
                        {
                            ISet<ElementId> viewIds = sheet.GetAllPlacedViews();
                            bool flagLegend = true;
                            // Check if legend is placed on selected sheet
                            foreach (var vId in viewIds)
                            {
                                if (vId == activeView.Id)
                                {
                                    flagLegend = false;
                                    TaskDialog.Show("Warning", $"Legend already placed on {sheet.Name}.");
                                    break;
                                }
                            }
                            // Placed on selected sheet if it is not placed already
                            if (flagLegend)
                            {
                                Viewport.Create(doc, sheet.Id, activeView.Id, centerTitleBlock);
                                // Mark sheet as successfully placed
                                viewSheetSuccess.Add(sheet.SheetNumber);
                            }
                        }
                    }
                    // If activeView is already place on a sheet
                    else if (isViewOnSheet || viewPlaced == true)
                    {
                        // Duplicate view
                        ElementId viewId = activeView.Duplicate(ViewDuplicateOption.WithDetailing);
                        // Place duplicated view on sheet
                        Viewport.Create(doc, sheet.Id, viewId, centerTitleBlock);
                        // Mark sheet as successfully placed
                        viewSheetSuccess.Add(sheet.SheetNumber);
                    }
                    // If activeView is not on sheet
                    else
                    {
                        Viewport.Create(doc, sheet.Id, activeView.Id, centerTitleBlock);
                        viewPlaced = true;
                        // Mark sheet as successfully placed
                        viewSheetSuccess.Add(sheet.SheetNumber);
                    }
                }

                // Commit transaction
                transactionViews.Commit();

                // Display result message to user
                if (viewSheetSuccess.Count > 0)
                {
                    TaskDialog.Show("Success", activeView.Name + " has been placed on: \n" + string.Join("\n", viewSheetSuccess));
                }
            }

            return Result.Succeeded;
        }
    }
}
