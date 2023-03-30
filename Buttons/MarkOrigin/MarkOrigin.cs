using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System.Linq;
using System;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.MarkOrigin
{
    [TransactionAttribute(TransactionMode.Manual)]
    class MarkOrigin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            // Retrieve active View
            View activeView = doc.ActiveView;
            ViewType actViewType = activeView.ViewType;

            // Array of view types excluded
            ViewType[] viewTypes = { ViewType.Report, ViewType.Schedule, ViewType.Walkthrough,
                ViewType.Undefined};

            // Check current view has origin and it can be drawn on a plane
            if (viewTypes.Contains(actViewType) == false)
            {
                // Coordinate that determines horizontal and vertical projection length
                double sideCoordinate = 10;

                // Points to draw diagonals
                XYZ rightTop = new XYZ(sideCoordinate, 0, sideCoordinate);
                XYZ rightBottom = new XYZ(sideCoordinate, 0, -1 * sideCoordinate);
                XYZ leftTop = new XYZ(-1 * sideCoordinate, 0, sideCoordinate);
                XYZ leftBottom = new XYZ(-1 * sideCoordinate, 0, -1 * sideCoordinate);
                // Project point to XY plane to get angle between view direction and this initial point
                XYZ projectedRightTop = new XYZ(rightTop.X, rightTop.Y, 0);

                // Retrieve normal to view
                XYZ viewDir = activeView.ViewDirection;

                // Angle between viewDir and rightTop and rotation
                double angleRadians = HelpersGeometry.AngleBetweenVectors(viewDir, projectedRightTop);
                double angleToRotate = Math.PI / 2 - angleRadians;
                Transform rotation = Transform.CreateRotation(XYZ.BasisZ, angleToRotate);

                // Create points coordinates
                XYZ transRightTop = rotation.OfPoint(rightTop);
                XYZ transRightBottom = rotation.OfPoint(rightBottom);
                XYZ transLeftTop = rotation.OfPoint(leftTop);
                XYZ transLeftBottom = rotation.OfPoint(leftBottom);

                // Define new points for plan
                if ((viewDir.Z == 1 || viewDir.Z == -1) || activeView.ViewType == ViewType.ThreeD)
                {
                    transRightTop = new XYZ(sideCoordinate, sideCoordinate, 0);
                    transRightBottom = new XYZ(sideCoordinate, -1 * sideCoordinate, 0);
                    transLeftTop = new XYZ(-1 * sideCoordinate, sideCoordinate, 0);
                    transLeftBottom = new XYZ(-1 * sideCoordinate, -1 * sideCoordinate, 0);
                }

                // Open transaction to create marker
                Transaction t = new Transaction(doc, "Creteate Origin Marker");
                t.Start();

                // Create lines
                Line diagonal00 = Line.CreateBound(transRightTop, transLeftBottom);
                Line diagonal01 = Line.CreateBound(transLeftTop, transRightBottom);
                // Store marker lines id
                ICollection<ElementId> elementIds;

                // Create markers for type of views
                if (activeView.ViewType == ViewType.ThreeD)
                {
                    Plane originPlane = Plane.CreateByNormalAndOrigin(XYZ.BasisZ, new XYZ());
                    SketchPlane sketchPlane = SketchPlane.Create(doc, originPlane);
                    ModelCurve modelLine01;
                    ModelCurve modelLine00;
                    if (doc.IsFamilyDocument)
                    {
                        modelLine00 = doc.FamilyCreate.NewModelCurve(diagonal00, sketchPlane);
                        modelLine01 = doc.FamilyCreate.NewModelCurve(diagonal01, sketchPlane);
                    }
                    else
                    {
                        modelLine00 = doc.Create.NewModelCurve(diagonal00, sketchPlane);
                        modelLine01 = doc.Create.NewModelCurve(diagonal01, sketchPlane);
                    }

                    elementIds = new List<ElementId>
                    {
                        modelLine00.Id,
                        modelLine01.Id
                    };
                }
                else
                {
                    DetailCurve detailLine00;
                    DetailCurve detailLine01;
                    if (doc.IsFamilyDocument)
                    {
                        try
                        {
                            detailLine00 = doc.FamilyCreate.NewDetailCurve(activeView, diagonal00);
                            detailLine01 = doc.FamilyCreate.NewDetailCurve(activeView, diagonal01);
                        }
                        catch(Autodesk.Revit.Exceptions.InvalidOperationException)
                        {
                            MessageWindows.AlertMessage("Error", "Detail Lines cannot be drawn in this type of document,\n" +
                                "try a 3D view instead to use Model Lines");

                            t.RollBack();
                            return Result.Failed;
                        }
                    }
                    else
                    {
                        detailLine00 = doc.Create.NewDetailCurve(activeView, diagonal00);
                        detailLine01 = doc.Create.NewDetailCurve(activeView, diagonal01);
                    }

                    elementIds = new List<ElementId>
                    {
                        detailLine00.Id,
                        detailLine01.Id
                    };
                }

                // Create group and highlight marker for easy user location
                Group group = HelpersSelection.CreateGroupFromElementIds(doc, elementIds);
                ICollection<ElementId> selectionElIds = new List<ElementId>
                {
                    group.Id
                };
                uidoc.Selection.SetElementIds(selectionElIds);

                t.Commit();

                return Result.Succeeded;
            }
            else
            {
                MessageWindows.AlertMessage("Error", "ViewType not supported, please open a supported View \nsuch as a Plan, Section or Drafting View.");

                return Result.Failed;
            }
        }
    }
}
