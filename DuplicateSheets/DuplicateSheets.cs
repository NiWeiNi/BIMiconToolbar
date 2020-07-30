using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMiconToolbar.DuplicateSheets
{
    [TransactionAttribute(TransactionMode.Manual)]
    class DuplicateSheets : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Check the current active view
            View selView = doc.ActiveView;
            ViewSheet vSheet = doc.ActiveView as ViewSheet;
            // Retrieve titleblock from current sheet and all elements in view
            var titleblock = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance))
                            .OfCategory(BuiltInCategory.OST_TitleBlocks).Cast<FamilyInstance>()
                            .First(q => q.OwnerViewId == vSheet.Id);
            var elementsInViewId = new FilteredElementCollector(doc, selView.Id).ToElementIds();
            // Retrieve viewports in view
            FilteredElementCollector viewPorts = new FilteredElementCollector(doc, selView.Id).OfClass(typeof(Viewport));
            // Retrieve schedules in view
            FilteredElementCollector schedules = new FilteredElementCollector(doc).OwnedByView(selView.Id)
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
                newsheet.SheetNumber = vSheet.SheetNumber + "-DUP";
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
                        if (origView.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
                        {
                            ElementId newViewId = origView.Duplicate(ViewDuplicateOption.WithDetailing);
                            newView = doc.GetElement(newViewId) as View;
                            newView.Name = origView.Name + "-DUP";
                        }
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
                ElementTransformUtils.CopyElements( selView, annotationElementsId, newsheet, null, null );

                // Commit transaction
                t.Commit();

                return Result.Succeeded;
            }
        }
    }
}