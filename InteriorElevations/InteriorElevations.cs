using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMiconToolbar.InteriorElevations
{
    [TransactionAttribute(TransactionMode.Manual)]
    class InteriorElevations : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Variables to store user input
            List<int> selectedIntIds;
            Element titleBlock;
            View viewTemplate;
            ViewFamilyType viewFamilyType;

            // Prompt window to collect user input
            using (InteriorElevationsWindow customWindow = new InteriorElevationsWindow(commandData))
            {
                customWindow.ShowDialog();
                selectedIntIds = customWindow.IntegerIds;
                titleBlock = customWindow.SelectedComboItemTitleBlock.Tag as Element;
                viewTemplate = customWindow.SelectedComboItemViewTemplate.Tag as View;
                viewFamilyType = customWindow.SelectedComboItemViewType.Tag as ViewFamilyType;
            }

            // No required elements loaded
            if (titleBlock == null && viewTemplate == null && viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load a Elevation, a Title Block and create a View Template");
                return Result.Cancelled;
            }
            // No title block and elevation loaded
            else if (titleBlock == null && viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load a Elevation and a Title Block");
                return Result.Cancelled;
            }
            // No title block and view template
            else if (titleBlock == null && viewTemplate == null)
            {
                TaskDialog.Show("Warning", "Please load a Title Block and create a View Template");
                return Result.Cancelled;
            }
            // No elevation and view template
            else if (viewFamilyType == null && viewTemplate == null)
            {
                TaskDialog.Show("Warning", "Please load an Elevation and create a View Template");
                return Result.Cancelled;
            }
            // No elevation
            else if (viewFamilyType == null)
            {
                TaskDialog.Show("Warning", "Please load an Elevation");
                return Result.Cancelled;
            }
            // No title block
            else if (titleBlock == null)
            {
                TaskDialog.Show("Warning", "Please load a Title Block");
                return Result.Cancelled;
            }
            // No view template
            else if (titleBlock == null)
            {
                TaskDialog.Show("Warning", "Please create a view template");
                return Result.Cancelled;
            }
            // Room selected
            else if (selectedIntIds != null)
            {
                // Select first plan view
                FilteredElementCollector floorPlansCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
                View floorPlan = floorPlansCollector.Cast<View>().Where(v =>
                                   v.ViewType == ViewType.FloorPlan).Where(v => v.IsTemplate == false).FirstOrDefault();

                if (floorPlan == null)
                {
                    TaskDialog.Show("Warning", "Plese create a floor plan");
                    return Result.Cancelled;
                }

                // Collect rooms
                foreach (int id in selectedIntIds)
                {
                    Room room = doc.GetElement(new ElementId(id)) as Room;

                    // Retrieve boundaries
                    IList<IList<BoundarySegment>> boundaries = Helpers.Helpers.SpatialBoundaries(room);

                    if (boundaries != null)
                    {
                        if (boundaries.Count == 1)
                        {
                            // Transaction
                            Transaction t = new Transaction(doc, "Create Interior Elevations");
                            t.Start();

                            List<XYZ> points = Helpers.Helpers.BoundaPoints(boundaries);
                            XYZ centroid = Helpers.Helpers.Centroid(points);

                            // Create sheet
                            ViewSheet sheet = ViewSheet.Create(doc, titleBlock.Id);
                            sheet.Name = room.Number + "-" + "INTERIOR ELEVATIONS";

                            // Retrieve title block
                            FamilyInstance tBlock = new FilteredElementCollector(doc, sheet.Id)
                                                    .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                    .FirstElement() as FamilyInstance;

                            // Retrieve title block size
                            double sheetHeight = tBlock.get_Parameter(BuiltInParameter.SHEET_HEIGHT).AsDouble();
                            double sheetWidth = tBlock.get_Parameter(BuiltInParameter.SHEET_WIDTH).AsDouble();

                            // Center of title block
                            XYZ centerTitleBlock = new XYZ(sheetWidth / 2, sheetHeight / 2, 0);

                            // Create elevation marker
                            ElevationMarker marker = ElevationMarker.CreateElevationMarker(doc, viewFamilyType.Id, centroid, viewTemplate.Scale);

                            // Get settings of current document
                            Settings documentSettings = doc.Settings;

                            // Retrieve annotation categories
                            Categories cats = documentSettings.Categories;

                            var annoCategories = new List<ElementId>();

                            foreach(Category cat in cats)
                            {
                                if (cat.CategoryType == CategoryType.Annotation)
                                {
                                    annoCategories.Add(cat.Id);
                                }
                            }

                            // Viewport dimensions
                            var vPOutlines = new List<Outline>();
                            var labelOutlines = new List<Outline>();

                            var viewports = new List<Viewport>();

                            // Place views on sheet
                            for (int i = 0; i < 4; i++)
                            {
                                View view = marker.CreateElevation(doc, floorPlan.Id, i);
                                view.ViewTemplateId = viewTemplate.Id;

                                // Hide annotation categories to reduce viewport outline to minimum size
                                // This allows labels to align to the base
                                view.HideCategoriesTemporary(annoCategories);

                                // Regenerate document to pick view scale for title
                                doc.Regenerate();

                                // Create viewports
                                Viewport viewP = Viewport.Create(doc, sheet.Id, view.Id, new XYZ());

                                // Retrieve outlines
                                Outline vPOutline = viewP.GetBoxOutline();
                                Outline labelOutline = viewP.GetLabelOutline();
                                vPOutlines.Add(vPOutline);
                                labelOutlines.Add(labelOutline);

                                // Disable temporary hide
                                view.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

                                // Store viewports
                                viewports.Add(viewP);
                            }

                            // Dictitionary to store viewport dimensions
                            var viewportDims = new Dictionary<Viewport, double[]>();

                            foreach(Viewport vp in viewports)
                            {
                                Outline vpOut = vp.GetBoxOutline();
                                Outline labelOut = vp.GetLabelOutline();

                                // Viewport dimensions
                                XYZ maxPoint = vpOut.MaximumPoint;
                                XYZ minPoint = vpOut.MinimumPoint;

                                double vPxMax = maxPoint.X;
                                double vPxMin = minPoint.X;

                                double vPyMax = maxPoint.Y;
                                double vPyMin = minPoint.Y;

                                double vPxDist = vPxMax - vPxMin;
                                double vPyDist = vPyMax - vPyMin;

                                // Label dimensions
                                XYZ labelMaxPoint = labelOut.MaximumPoint;
                                XYZ labelMinPoint = labelOut.MinimumPoint;

                                double labelxMax = labelMaxPoint.X;
                                double labelxMin = labelMinPoint.X;

                                double labelyMax = labelMaxPoint.Y;
                                double labelyMin = labelMinPoint.Y;

                                double labelxDist = labelxMax - labelxMin;
                                double labelyDist = labelyMax - labelyMin;

                                // Store results
                                double[] dims = { vPxDist, vPyDist };
                                viewportDims.Add(vp, dims); 
                            }

                            // Retrieve overall dimensions
                            List<double> firstRowX = new List<double>();
                            List<double> firstRowY = new List<double>();

                            List<double> secondRowX = new List<double>();
                            List<double> secondRowY = new List<double>();

                            foreach (KeyValuePair<Viewport, double[]> entry in viewportDims)
                            {
                                Viewport vp = entry.Key;
                                string detailNumber = vp.get_Parameter(BuiltInParameter.VIEWER_DETAIL_NUMBER).AsString();
                                
                                if (detailNumber == "1" || detailNumber == "2")
                                {
                                    firstRowX.Add(entry.Value[0]);
                                    firstRowY.Add(entry.Value[1]);
                                }
                                else
                                {
                                    secondRowX.Add(entry.Value[0]);
                                    secondRowY.Add(entry.Value[1]);
                                }
                            }

                            // Calculate X spacing
                            double spacingViewX = Helpers.Helpers.MillimetersToFeet(30);
                            double overallX = spacingViewX;

                            if (firstRowX.Sum() > secondRowX.Sum())
                            {
                                overallX += firstRowX.Sum();
                            }
                            else
                            {
                                overallX += secondRowX.Sum();
                            }

                            // Calculate Y spacing
                            double spacingViewY = Helpers.Helpers.MillimetersToFeet(30);
                            double overallY = spacingViewY;

                            if (firstRowY.Sum() > secondRowY.Sum())
                            {
                                overallY += firstRowY.Sum();
                            }
                            else
                            {
                                overallY += secondRowY.Sum();
                            }

                            // Closest and furthest X points
                            double centerTitleBlockX = centerTitleBlock.X;
                            double furthestX = overallX / 2 + centerTitleBlockX;
                            double closestX = centerTitleBlockX - overallX / 2;

                            // Closest and furthest Y points
                            double centerTitleBlockY = centerTitleBlock.Y;
                            double furthestY = overallY / 2 + centerTitleBlockY;
                            double closestY = centerTitleBlockY - overallY / 2;

                            // Points boundary
                            XYZ lowestRight = new XYZ(furthestX, closestY, 0);
                            XYZ lowestLeft = new XYZ(closestX, closestY, 0);
                            XYZ heightestLeft = new XYZ(closestX, furthestY, 0);
                            XYZ heightestRight = new XYZ(furthestX, furthestY, 0);

                            // Move viewports
                            foreach (Viewport vp in viewports)
                            {
                                Outline vpOut = vp.GetBoxOutline();
                                Outline labelOut = vp.GetLabelOutline();

                                // Viewport dimensions
                                XYZ maxPoint = vpOut.MaximumPoint;
                                XYZ minPoint = vpOut.MinimumPoint;

                                string detailNumber = vp.get_Parameter(BuiltInParameter.VIEWER_DETAIL_NUMBER).AsString();

                                if (detailNumber == "4")
                                {
                                    XYZ lowRightPoint = new XYZ(maxPoint.X, minPoint.Y, 0);
                                    XYZ moveVec = lowestRight - lowRightPoint;

                                    ElementTransformUtils.MoveElement(doc, vp.Id, moveVec);
                                }
                                else if (detailNumber == "3")
                                {
                                    XYZ lowLeftPoint = new XYZ(minPoint.X, minPoint.Y, 0);
                                    XYZ moveVec = lowestLeft - lowLeftPoint;

                                    ElementTransformUtils.MoveElement(doc, vp.Id, moveVec);
                                }
                                else if (detailNumber == "2")
                                {
                                    XYZ highRightPoint = new XYZ(maxPoint.X, maxPoint.Y, 0);
                                    XYZ moveVec = heightestRight - highRightPoint;

                                    ElementTransformUtils.MoveElement(doc, vp.Id, moveVec);
                                }
                                else if (detailNumber == "1")
                                {
                                    XYZ highLeftPoint = new XYZ(minPoint.X, maxPoint.Y, 0);
                                    XYZ moveVec = heightestLeft - highLeftPoint;

                                    ElementTransformUtils.MoveElement(doc, vp.Id, moveVec);
                                }
                            }
                            // Commit transaction
                            t.Commit();
                        }
                        // When room has more than 4 sides
                        else
                        {
                            string messageWarning = "Elevation not created for room: " + room.Number + " - " + room.Name;
                            string messageReason = "This part of the script is still WIP, apologies for any inconvenience";
                            TaskDialog.Show("Warning", messageWarning + "\n" + messageReason);
                        }
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
