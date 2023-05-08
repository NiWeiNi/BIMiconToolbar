using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace BIMicon.BIMiconToolbar.DuplicateSheets
{
    [TransactionAttribute(TransactionMode.Manual)]
    class DuplicateSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Call WPF for user input
            using (DuplicateSheetsWPF customWindow = new DuplicateSheetsWPF(commandData))
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();

                // Retrieve all user input
                List<int> sheetIds = customWindow.sheetIds;
                var titleBlockComboBox = customWindow.SelectedTitleblock;
                var copyViews = customWindow.copyViews;
                var optDuplicate = customWindow.optDuplicate;
                var optDuplicateDetailing = customWindow.optDuplicateDetailing;
                var optDuplicateDependant = customWindow.optDuplicateDependant;
                var viewPrefix = customWindow.viewPrefix;
                var viewSuffix = customWindow.viewSuffix;
                var sheetPrefix = customWindow.sheetPrefix;
                var sheetSuffix = customWindow.sheetSuffix;

                // Establish duplicate options at top to avoid reassignment inside loop
                var viewDuplicateOption = ViewDuplicateOption.Duplicate;
                if (optDuplicateDetailing == true)
                {
                    viewDuplicateOption = ViewDuplicateOption.WithDetailing;
                }
                else if (optDuplicateDependant == true)
                {
                    viewDuplicateOption = ViewDuplicateOption.AsDependent;
                }

                // Retrieve all sheets and views in project
                var sheets = new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).ToElements().Cast<ViewSheet>().ToArray();
                var views = new FilteredElementCollector(doc).OfClass(typeof(View)).ToElements().Cast<View>().ToArray();

                // Group transacation
                TransactionGroup tg = new TransactionGroup(doc, "Duplicate sheets");
                tg.Start();

                // List to store sheets duplicated
                var viewSheetSuccess = new List<string>();

                // Duplicate all selected sheets
                foreach (var sId in sheetIds)
                {
                    // Retrieve sheet and sheet Id
                    ElementId sheetId = new ElementId(sId);
                    ViewSheet vSheet = doc.GetElement(sheetId) as ViewSheet;

                    // Retrieve title block according to user input
                    FamilyInstance titleblock = null;
                    if (titleBlockComboBox.Name != "Current Title Block")
                    {
                        titleblock = doc.GetElement(new ElementId(titleBlockComboBox.Id)) as FamilyInstance;
                    }
                    else
                    {
                        // Retrieve titleblock from current sheet
                        titleblock = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                                        .OfCategory(BuiltInCategory.OST_TitleBlocks).Cast<FamilyInstance>()
                                        .First(q => q.OwnerViewId == vSheet.Id);
                    }
                    // Guard against no loaded titleblocks in project or in sheet
                    if (titleblock == null)
                    {
                        titleblock = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                                    .OfCategory(BuiltInCategory.OST_TitleBlocks).Cast<FamilyInstance>()
                                    .First();
                    }

                    // Retrieve elements on sheet
                    var elementsInViewId = new FilteredElementCollector(doc, sheetId).ToElementIds();
                    // Retrieve viewports in view
                    FilteredElementCollector viewPorts = new FilteredElementCollector(doc, sheetId).OfClass(typeof(Viewport));
                    // Retrieve schedules in view
                    FilteredElementCollector schedules = new FilteredElementCollector(doc).OwnedByView(sheetId)
                                                        .OfClass(typeof(ScheduleSheetInstance));
                    // Retrieve viewSchedules
                    FilteredElementCollector viewSchedules = new FilteredElementCollector(doc).OfClass(typeof(ViewSchedule));

                    // Store copied elements and annotation elements
                    var copiedElementIds = new List<ElementId>();
                    var annotationElementsId = new List<ElementId>();

                    using (Transaction t = new Transaction(doc, "Duplicate Sheet"))
                    {
                        // Start transaction to duplicate sheet
                        t.Start();

                        // Duplicate sheet
                        ViewSheet newsheet = ViewSheet.Create(doc, titleblock.GetTypeId());

                        // Check if sheet number is in use
                        string oldSheetNumber = sheetPrefix + vSheet.SheetNumber + sheetSuffix;
                        string sheetNumber = SheetsViews.UniqueStringNumberSheet(oldSheetNumber, sheets);
 
                        newsheet.SheetNumber = sheetNumber;
                        newsheet.Name = vSheet.Name;

                        // Get origin of the titleblock
                        XYZ originTitle = titleblock.GetTransform().Origin;
                        // Check titleblock position
                        Element copyTitleBlock = new FilteredElementCollector(doc).OwnedByView(newsheet.Id).OfCategory(BuiltInCategory.OST_TitleBlocks).FirstElement();
                        LocationPoint titleLoc = copyTitleBlock.Location as LocationPoint;
                        XYZ titleLocPoint = titleLoc.Point;
                        // Check if title block is in the same position as original
                        if (titleLocPoint.DistanceTo(originTitle) != 0)
                        {
                            // Move it in case it is not
                            titleLoc.Move(originTitle);
                        }

                        // Check if user selected copy views
                        if (copyViews)
                        {
                            // Retrieve all views placed on sheet except schedules
                            foreach (ElementId eId in vSheet.GetAllPlacedViews())
                            {
                                View origView = doc.GetElement(eId) as View;
                                View newView = null;

                                // Legends
                                if (origView.ViewType == ViewType.Legend)
                                {
                                    newView = origView;
                                }
                                // Rest of view types
                                else
                                {
                                    ElementId newViewId = null;

                                    if (origView.CanViewBeDuplicated(viewDuplicateOption))
                                    {
                                        newViewId = origView.Duplicate(viewDuplicateOption);
                                    }
                                    else
                                    {
                                        newViewId = origView.Duplicate(ViewDuplicateOption.Duplicate);
                                    }

                                    newView = doc.GetElement(newViewId) as View;

                                    // Check if view name is in use
                                    string oldViewName = viewPrefix + origView.Name + viewSuffix;
                                    string viewName = SheetsViews.UniqueStringViewName(oldViewName, views);
                                    newView.Name = viewName;
                                }

                                // Loop through viewports
                                foreach (Viewport vp in viewPorts)
                                {
                                    if (vp.SheetId == vSheet.Id && vp.ViewId == origView.Id)
                                    {
                                        // Retrieve centerpoint of original viewport
                                        XYZ center = vp.GetBoxCenter();
                                        // Create viewport in the original spot
                                        Viewport newVp = Viewport.Create(doc, newsheet.Id, newView.Id, center);
                                    }
                                    // Add element in copied list
                                    copiedElementIds.Add(vp.Id);
                                }
                                // Add element in copied list
                                copiedElementIds.Add(eId);
                            }

                            // Retrieve and copy schedules
                            foreach (ScheduleSheetInstance sch in schedules)
                            {
                                // Check schedule is not a revision inside titleblock
                                if (!sch.IsTitleblockRevisionSchedule)
                                {
                                    foreach (ViewSchedule vsc in viewSchedules)
                                    {
                                        if (sch.ScheduleId == vsc.Id)
                                        {
                                            // Retrieve center of schedule
                                            XYZ schCenter = sch.Point;
                                            // Create schedule in the same position
                                            ScheduleSheetInstance newSch = ScheduleSheetInstance.Create(doc, newsheet.Id, vsc.Id, schCenter);
                                        }
                                        copiedElementIds.Add(vsc.Id);
                                    }
                                }
                            }

                            // Duplicate annotation elements
                            foreach (ElementId eId in elementsInViewId)
                            {
                                if (!copiedElementIds.Contains(eId))
                                {
                                    annotationElementsId.Add(eId);
                                }
                            }

                            // Copy annotation elements
                            ElementTransformUtils.CopyElements(vSheet, annotationElementsId, newsheet, null, null);
                        }

                        viewSheetSuccess.Add(newsheet.SheetNumber + " - " + newsheet.Name);

                        // Commit transaction
                        t.Commit();
                    }
                }

                // Commit group transaction
                tg.Assimilate();

                // Display result message to user
                if (viewSheetSuccess.Count > 0)
                {
                    MessageWindows.AlertMessage("Success", "The following sheets have been duplicated: \n" + string.Join("\n", viewSheetSuccess));
                }

                return Result.Succeeded;
            }
        }
    }
}