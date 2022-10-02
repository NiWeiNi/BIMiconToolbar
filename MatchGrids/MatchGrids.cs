using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace BIMiconToolbar.MatchGrids
{
    [TransactionAttribute(TransactionMode.Manual)]
    class MatchGrids : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Variables to store user input
            View selectedView;
            bool copyDims;
            List<int> selectedIntIds;

            // Prompt window to collect user input
            using (MatchGridsWPF customWindow = new MatchGridsWPF(commandData))
            {
                customWindow.ShowDialog();
                copyDims = customWindow.copyDim;
                selectedView = customWindow.SelectedComboItem.Tag as View;
                selectedIntIds = customWindow.IntegerIds;

                // Check that elements have been selected
                if (selectedIntIds == null)
                {
                    return Result.Cancelled;
                }
                else if (selectedIntIds.Count == 0)
                {
                    TaskDialog.Show("Warning", "No views have been selected");
                    return Result.Cancelled;
                }
                else if (selectedIntIds.Count != 0)
                {
                    // Collect grids visible in view
                    FilteredElementCollector gridsCollector = new FilteredElementCollector(doc, selectedView.Id)
                                                            .OfCategory(BuiltInCategory.OST_Grids)
                                                            .WhereElementIsNotElementType();

                    // Grid Ids
                    ICollection<ElementId> gridIds = gridsCollector.ToElementIds();

                    // Template for grids display
                    Dictionary<ElementId, GridSpecsInView> gridsTemplate = new Dictionary<ElementId, GridSpecsInView>();

                    // Check each grid
                    foreach (Grid g in gridsCollector)
                    {
                        GridSpecsInView gridSpecs = new GridSpecsInView(g, selectedView);
                        gridsTemplate.Add(g.Id, gridSpecs);
                    }

                    // Retrieve views to be matched
                    List<View> viewsToMatch = new List<View>();

                    foreach (int intId in selectedIntIds)
                    {
                        ElementId eId = new ElementId(intId);
                        View view = doc.GetElement(eId) as View;

                        viewsToMatch.Add(view);
                    }

                    // Transaction
                    using (Transaction gridTransacation = new Transaction(doc, "Match grids"))
                    {
                        gridTransacation.Start();

                        foreach (View vMatch in viewsToMatch)
                        {
                            // Collect grids visible in view
                            FilteredElementCollector gridsMatchCollector = new FilteredElementCollector(doc, vMatch.Id)
                                                                    .OfCategory(BuiltInCategory.OST_Grids)
                                                                    .WhereElementIsNotElementType();

                            // Match each visible grid in view
                            foreach (Grid gMatch in gridsMatchCollector)
                            {
                                ElementId gId = gMatch.Id;

                                if (gridIds.Contains(gId))
                                {
                                    bool startGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End0, vMatch);
                                    bool endGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End1, vMatch);

                                    bool hasStartBubble = gridsTemplate[gId].StartBubble;
                                    bool hasEndBubble = gridsTemplate[gId].EndBubble;

                                    // Match Start bubble
                                    if (hasStartBubble == true && startGridMatch == false)
                                    {
                                        gMatch.ShowBubbleInView(DatumEnds.End0, vMatch);
                                    }
                                    else if (hasStartBubble == false && startGridMatch)
                                    {
                                        gMatch.HideBubbleInView(DatumEnds.End0, vMatch);
                                    }

                                    // Match End bubble
                                    if (hasEndBubble == true && endGridMatch == false)
                                    {
                                        gMatch.ShowBubbleInView(DatumEnds.End1, vMatch);
                                    }
                                    else if (hasEndBubble == false && endGridMatch)
                                    {
                                        gMatch.HideBubbleInView(DatumEnds.End1, vMatch);
                                    }

                                    // Match 2D extension
                                    DatumExtentType datumExtentTypeStart = gridsTemplate[gId].SelectedGrid
                                        .GetDatumExtentTypeInView(DatumEnds.End0, vMatch);
                                    DatumExtentType datumExtentTypeEnd = gridsTemplate[gId].SelectedGrid
                                        .GetDatumExtentTypeInView(DatumEnds.End1, vMatch);

                                    gMatch.SetDatumExtentType(DatumEnds.End0, vMatch, datumExtentTypeStart);
                                    gMatch.SetDatumExtentType(DatumEnds.End1, vMatch, datumExtentTypeEnd);

                                    double tempLevel = selectedView.GenLevel.Elevation;
                                    double matchLevel = vMatch.GenLevel.Elevation;

                                    double diffLevel = matchLevel - tempLevel;

                                    IList<Curve> curves = gridsTemplate[gId].ListCurve;
                                    Curve curve = curves[0];

                                    Plane planeSelected = selectedView.SketchPlane.GetPlane();
                                    Plane planeDest = vMatch.SketchPlane.GetPlane();

                                    // Origin grid selected
                                    Options options = new Options
                                    {
                                        View = selectedView
                                    };
                                    GeometryElement geoEle = gridsTemplate[gId].SelectedGrid.get_Geometry(options);
                                    XYZ selOrigin = new XYZ();
                                    foreach (GeometryObject geoObj in geoEle)
                                    {
                                        Line line = geoObj as Line;
                                        if (line != null)
                                        {
                                            selOrigin = line.Origin;
                                            break;
                                        }
                                    }

                                    // Origin grid to match
                                    Options optMatch = new Options
                                    {
                                        View = vMatch
                                    };
                                    GeometryElement geoEleMatch = gMatch.get_Geometry(optMatch);
                                    XYZ matchOrigin = new XYZ();
                                    foreach (GeometryObject geoObj in geoEleMatch)
                                    {
                                        Line line = geoObj as Line;
                                        if (line != null)
                                        {
                                            matchOrigin = line.Origin;
                                            break;
                                        }
                                    }

                                    XYZ gMatchOrigin = gMatch.Curve.GetEndPoint(0);

                                    XYZ dist = planeDest.Origin - planeSelected.Origin;

                                    dist = new XYZ(0, 0, matchOrigin.Z - selOrigin.Z);
                                    Transform trans = Transform.CreateTranslation(dist);

                                    Curve transCurve = curve.CreateTransformed(trans);

                                    gMatch.SetCurveInView(DatumExtentType.ViewSpecific, vMatch, transCurve);

                                }
                            }
                        }

                        // Copy grid dimensions
                        if (copyDims)
                        {
                            // Collect dimensions in selected view
                            FilteredElementCollector dimensionsCollector = new FilteredElementCollector(doc, selectedView.Id)
                                                                    .OfCategory(BuiltInCategory.OST_Dimensions)
                                                                    .WhereElementIsNotElementType();

                            // Dimensions to copy
                            List<ElementId> dimsToCopy = new List<ElementId>();

                            // Check dimensions only take grids as references 
                            foreach (Dimension d in dimensionsCollector)
                            {
                                ReferenceArray dReferences = d.References;
                                bool gridDim = true;

                                foreach (Reference dRef in dReferences)
                                {
                                    ElementId dRefId = dRef.ElementId;

                                    if (!gridIds.Contains(dRefId))
                                    {
                                        gridDim = false;
                                        break;
                                    }
                                }

                                if (gridDim)
                                {
                                    dimsToCopy.Add(d.Id);
                                }
                            }

                            // Copy dimensions
                            if (dimsToCopy.Count > 0)
                            {
                                CopyPasteOptions cp = new CopyPasteOptions();

                                foreach (View v in viewsToMatch)
                                {
                                    ElementTransformUtils.CopyElements(selectedView, dimsToCopy, v, null, cp);
                                }
                            }
                        }

                        gridTransacation.Commit();
                    }
                }

                return Result.Succeeded;
            }
        }
    }
}
