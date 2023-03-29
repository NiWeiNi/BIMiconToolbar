using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System;
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

            // Check document is not a family document
            if (RevitDocument.IsDocumentNotProjectDoc(doc))
            {
                return Result.Failed;
            }
            else
            {
                // Variables to store user input
                List<int> selectedIntIds;
                Element titleBlock;
                View viewTemplate;
                ViewFamilyType viewFamilyType;
                double sheetDrawingHeight;
                double sheetDrawingWidth;

                // Prompt window to collect user input
                using (InteriorElevationsWindow customWindow = new InteriorElevationsWindow(commandData))
                {
                    customWindow.ShowDialog();
                    selectedIntIds = customWindow.IntegerIds;
                    titleBlock = customWindow.SelectedComboItemTitleBlock.Tag as Element;
                    viewTemplate = customWindow.SelectedComboItemViewTemplate.Tag as View;
                    viewFamilyType = customWindow.SelectedComboItemViewType.Tag as ViewFamilyType;

                    #region Required elements for this tool

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
                    #endregion

                    // Room selected
                    else if (selectedIntIds != null)
                    {
                        // Select first plan view
                        FilteredElementCollector floorPlansCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
                        View floorPlan = floorPlansCollector.Cast<View>().Where(v =>
                                           v.ViewType == ViewType.FloorPlan).Where(v => v.IsTemplate == false).FirstOrDefault();

                        if (floorPlan == null)
                        {
                            TaskDialog.Show("Warning", "Please create a floor plan");
                            return Result.Cancelled;
                        }

                        // Store rooms with elevations created
                        List<string> roomsSucceeded = new List<string>();

                        // Collect rooms
                        foreach (int id in selectedIntIds)
                        {
                            Room room = doc.GetElement(new ElementId(id)) as Room;

                            // Retrieve boundaries
                            IList<IList<BoundarySegment>> boundaries = GeneralHelpers.SpatialBoundaries(room);

                            // Check boundaries list is not empty
                            if (boundaries != null)
                            {
                                // Retrieve doc annotation categories
                                var annoCategories = GeneralHelpers.AnnoCatIds(doc);

                                #region Rectangular rooms without interior boundaries
                                if (boundaries[0].Count == 4 && boundaries.Count == 1 && GeneralHelpers.IsRectangle(boundaries[0]))
                                {
                                    // Transaction
                                    Transaction t = new Transaction(doc, "Create Single Marker Interior Elevations");
                                    t.Start();

                                    List<XYZ> points = GeneralHelpers.BoundaPoints(boundaries);
                                    XYZ centroid = GeneralHelpers.Centroid(points);

                                    // Create sheet
                                    ViewSheet sheet = HelpersView.CreateSheet(doc,
                                                                                    titleBlock.Id,
                                                                                    room.Number + "-" + "INTERIOR ELEVATIONS");

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

                                    // Place views on sheet
                                    var viewports = new List<Viewport>();

                                    for (int i = 0; i < 4; i++)
                                    {
                                        // Create elevation
                                        View view = HelpersView.CreateViewElevation(doc, marker, floorPlan, i, viewTemplate,
                                                                                            annoCategories);
                                        HelpersView.CreateViewport(doc, sheet, ref viewports, view);
                                    }

                                    // Dictionary to store viewport dimensions
                                    var viewportDims = HelpersView.ViewportDimensions(viewports);

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
                                    double spacingViewX = GeneralHelpers.MillimetersToFeet(30);
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
                                    double spacingViewY = GeneralHelpers.MillimetersToFeet(30);
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

                                    // Append room number to success list
                                    roomsSucceeded.Add(room.Number);

                                    // Commit transaction
                                    t.Commit();
                                }
                                #endregion

                                #region Rest of rooms
                                // When room has more or less than 4 sides
                                else
                                {
                                    // Offset distanceof the elevation marker
                                    double offsetElevation = GeneralHelpers.MillimetersToFeet(-1000);
                                    // Vector Z
                                    XYZ zAxis = new XYZ(0, 0, 1);

                                    // Transaction to create single elevations
                                    Transaction t2 = new Transaction(doc, "Create single elevations");
                                    t2.Start();

                                    // Create sheet
                                    ViewSheet sheet = HelpersView.CreateSheet(doc,
                                                         titleBlock.Id,
                                                         room.Number + "-" + "INTERIOR ELEVATIONS");

                                    // Retrieve title block
                                    FamilyInstance tBlock = new FilteredElementCollector(doc, sheet.Id)
                                                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                            .FirstElement() as FamilyInstance;

                                    // Retrieve title block size
                                    double sheetHeight = tBlock.get_Parameter(BuiltInParameter.SHEET_HEIGHT).AsDouble();
                                    double sheetWidth = tBlock.get_Parameter(BuiltInParameter.SHEET_WIDTH).AsDouble();

                                    // Store viewports on sheet
                                    var viewports = new List<Viewport>();

                                    // Loop through each boundary
                                    foreach (var boundary in boundaries[0])
                                    {
                                        Curve originalCurve = boundary.GetCurve();
                                        Curve offsetCurve = originalCurve.CreateOffset(offsetElevation, zAxis);

                                        // Curve centers
                                        XYZ origCenter = (originalCurve.GetEndPoint(0) + originalCurve.GetEndPoint(1)) / 2;
                                        XYZ offsetCenter = (offsetCurve.GetEndPoint(0) + offsetCurve.GetEndPoint(1)) / 2;

                                        // Vector marker to center of original boundary
                                        XYZ vec = origCenter - offsetCenter;

                                        // Create elevation marker
                                        ElevationMarker marker = ElevationMarker.CreateElevationMarker(doc, viewFamilyType.Id, offsetCenter, viewTemplate.Scale);

                                        // Calculate rotation angle
                                        double angle = GeneralHelpers.AngleTwoVectors(new XYZ(0, 100, 0), vec);

                                        // Check the component X of the translated vector to new origin is positive
                                        // this means angle to rotate clockwise
                                        if (angle != 0 && vec.X > 0)
                                        {
                                            angle = angle * -1;
                                        }
                                        else if (angle != 0 && origCenter.X < offsetCenter.X && origCenter.X > 0)
                                        {
                                            // Angle remains the same
                                        }
                                        // Check if rotation needs to be 180 degrees
                                        else if (angle == 0 && origCenter.Y < offsetCenter.Y)
                                        {
                                            angle = Math.PI;
                                        }
                                        // Line along z axis
                                        Line zLine = Line.CreateBound(new XYZ(offsetCenter.X, offsetCenter.Y, offsetCenter.Z), new XYZ(offsetCenter.X, offsetCenter.Y, offsetCenter.Z + 10));

                                        // Create Elevation View as marker needs to have at least one elevation to rotate
                                        View view = HelpersView.CreateViewElevation(doc, marker, floorPlan, 1, viewTemplate, annoCategories);

                                        // Rotate in increments as Revit API adds 180 degrees to certain positive angles
                                        if (angle > 0)
                                        {
                                            double angleRemainder = angle;
                                            while (angleRemainder > 0)
                                            {
                                                double rotAngle = 0;
                                                if (angleRemainder > 0.6)
                                                    rotAngle = 0.6;
                                                else
                                                    rotAngle = angleRemainder;

                                                ElementTransformUtils.RotateElement(doc, marker.Id, zLine, rotAngle);

                                                angleRemainder -= rotAngle;
                                            }
                                        }
                                        // Rotate normally
                                        else
                                        {
                                            ElementTransformUtils.RotateElement(doc, marker.Id, zLine, angle);
                                        }

                                        // Retrieve crop shape
                                        ViewCropRegionShapeManager cropShapeManag = view.GetCropRegionShapeManager();
                                        IList<CurveLoop> cropCurves = cropShapeManag.GetCropShape();

                                        // Retrieve max and min height
                                        double minHeight = 0;
                                        double maxHeight = 0;

                                        foreach (Curve curve in cropCurves[0])
                                        {
                                            XYZ point = curve.GetEndPoint(0);
                                            double height = point.Z;

                                            if (height > maxHeight)
                                            {
                                                maxHeight = height;
                                            }
                                            else if (height < minHeight)
                                            {
                                                minHeight = height;
                                            }
                                        }

                                        // Crop region offset
                                        double topOffset = GeneralHelpers.MillimetersToFeet(25);

                                        // Create curves for crop region
                                        XYZ bottomStartPoint = offsetCurve.GetEndPoint(0);
                                        XYZ bottomEndPoint = offsetCurve.GetEndPoint(1);

                                        XYZ startBase = new XYZ(bottomStartPoint.X, bottomStartPoint.Y, bottomStartPoint.Z + minHeight);
                                        XYZ endBase = new XYZ(bottomEndPoint.X, bottomEndPoint.Y, bottomEndPoint.Z + minHeight);

                                        XYZ startTop = new XYZ(bottomStartPoint.X, bottomStartPoint.Y, bottomStartPoint.Z + maxHeight + topOffset);
                                        XYZ endTop = new XYZ(bottomEndPoint.X, bottomEndPoint.Y, bottomEndPoint.Z + maxHeight + topOffset);

                                        // Create CurveLoop for new crop shape
                                        List<Curve> curvesNewCrop = new List<Curve>();

                                        // Create contiguous lines
                                        Line sideOne = Line.CreateBound(startBase, startTop);
                                        Line sideTwo = Line.CreateBound(endTop, endBase);
                                        Line top = Line.CreateBound(startTop, endTop);
                                        Line baseLine = Line.CreateBound(endBase, startBase);

                                        // Add the curves in order to the CurveLoop list
                                        curvesNewCrop.Add(baseLine);
                                        curvesNewCrop.Add(sideOne);
                                        curvesNewCrop.Add(top);
                                        curvesNewCrop.Add(sideTwo);

                                        // Apply new crop shape
                                        cropShapeManag.SetCropShape(CurveLoop.Create(curvesNewCrop));
                                        // Update document to reflect new crop shape when placing viewports
                                        doc.Regenerate();

                                        // Create viewports
                                        HelpersView.CreateViewport(doc, sheet, ref viewports, view);
                                    }

                                    // Final viewport translation coordinates
                                    sheetDrawingHeight = GeneralHelpers.MillimetersToFeet(customWindow.SheetDrawingHeight);
                                    sheetDrawingWidth = GeneralHelpers.MillimetersToFeet(customWindow.SheetDrawingWidth);

                                    var viewportDims = HelpersView.ViewportDimensions(viewports);
                                    var coordinates = HelpersView.ViewportRowsColumns(viewportDims, sheetDrawingWidth, sheetDrawingHeight);

                                    for (int i = 0; i < viewports.Count; i++)
                                    {
                                        ElementTransformUtils.MoveElement(doc, viewports[i].Id, coordinates[i]);
                                    }

                                    // Append room number to success list
                                    roomsSucceeded.Add(room.Number);

                                    // Commit transaction
                                    t2.Commit();
                                }
                                #endregion
                            }
                        }

                        #region Display results to user
                        // Display results to user
                        if (roomsSucceeded.Count > 0)
                        {
                            string messageSuccess = string.Join("\n", roomsSucceeded.ToArray());
                            TaskDialog.Show("Success", "The following room elevations have been created: " + "\n" + messageSuccess);
                        }
                        else
                        {
                            TaskDialog.Show("Error", "No room elevations have been created");
                        }
                        #endregion
                    }
                }

                return Result.Succeeded;
            }
        }
    }
}
