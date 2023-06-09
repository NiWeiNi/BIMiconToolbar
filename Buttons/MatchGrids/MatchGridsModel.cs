using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.MatchGrids;
using BIMicon.BIMiconToolbar.Models;
using BIMicon.BIMiconToolbar.Models.Forms;
using System.Collections.Generic;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Buttons.MatchGrids
{
    internal class MatchGridsModel
    {
        private readonly MatchGridsViewModel viewModel;
        public MatchGridsModel(MatchGridsViewModel viewModelCons) 
        {
            viewModel = viewModelCons;
        }

        public Result Execute()
        {
            // Retrieve selected elements from viewModel
            ICollection<BaseElement> selectedBaseElements = viewModel.SelectedViews;

            // Check that elements have been selected
            if (selectedBaseElements == null)
            {
                return Result.Cancelled;
            }
            else
            {
                MatchGrids();
                return Result.Succeeded;
            }
        }

        public void MatchGrids()
        {
            // Variables to store user input
            Document doc = viewModel.Doc;
            BaseElement selectedView = viewModel.SelectedViewTemplate;
            bool copyDims = false;
            ICollection<BaseElement> selectedBaseElements = viewModel.SelectedViews;
            ElementId selectedViewId = new ElementId(selectedView.Id);
            View selectedViewTemp = doc.GetElement(selectedViewId) as View;

            // Collect grids visible in view
            FilteredElementCollector gridsCollector = new FilteredElementCollector(doc, selectedViewId)
                                                    .OfCategory(BuiltInCategory.OST_Grids)
                                                    .WhereElementIsNotElementType();

            // Grid Ids
            ICollection<ElementId> gridIds = gridsCollector.ToElementIds();

            // Template for grids display
            Dictionary<ElementId, GridSpecsInView> gridsTemplate = new Dictionary<ElementId, GridSpecsInView>();

            // Check each grid
            foreach (Element element in gridsCollector)
            {
                Grid g = element as Grid;
                GridSpecsInView gridSpecs = new GridSpecsInView(g, selectedViewTemp);
                gridsTemplate.Add(g.Id, gridSpecs);
            }

            // Retrieve views to be matched
            IEnumerable<View> viewsToMatch = selectedBaseElements.Select(bE => doc.GetElement(new ElementId(bE.Id)) as View);

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
                    foreach (Element element in gridsMatchCollector)
                    {
                        ElementId gId = element.Id;
                        Grid gMatch = element as Grid;

                        if (gridIds.Contains(gId))
                        {
                            bool startGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End0, vMatch);
                            bool endGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End1, vMatch);

                            bool hasStartBubble = gridsTemplate[gId].StartBubble;
                            bool hasEndBubble = gridsTemplate[gId].EndBubble;

                            IList<Curve> curves = gridsTemplate[gId].ListCurve;
                            Curve curve = curves[0];

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

                            // Origin grid selected
                            Options options = new Options
                            {
                                View = selectedViewTemp
                            };
                            GeometryElement geoEle = gridsTemplate[gId].SelectedGrid.get_Geometry(options);
                            XYZ originPoint = new XYZ();
                            foreach (GeometryObject geoObj in geoEle)
                            {
                                Line line = geoObj as Line;
                                if (line != null)
                                {
                                    originPoint = line.Origin;
                                    break;
                                }
                            }

                            // Match grid guide line to view direction
                            XYZ viewDir = selectedViewTemp.ViewDirection;
                            XYZ dist;

                            if (viewDir.Z == 1 || viewDir.Z == -1)
                                dist = new XYZ(0, 0, matchOrigin.Z - originPoint.Z);
                            else if (viewDir.Y == 1 || viewDir.Y == -1)
                                dist = new XYZ(0, matchOrigin.Y - originPoint.Y, 0);
                            else
                                dist = new XYZ(matchOrigin.X - originPoint.X, 0, 0);

                            // Move grid guide line to plane of the view
                            Transform trans = Transform.CreateTranslation(dist);
                            Curve transCurve = curve.CreateTransformed(trans);
                            // Set grid line extensions in view
                            gMatch.SetCurveInView(DatumExtentType.ViewSpecific, vMatch, transCurve);

                            // Match Start bubble
                            if (hasStartBubble && !startGridMatch)
                            {
                                gMatch.ShowBubbleInView(DatumEnds.End0, vMatch);
                            }
                            else if (!hasStartBubble && startGridMatch)
                            {
                                gMatch.HideBubbleInView(DatumEnds.End0, vMatch);
                            }

                            // Match End bubble
                            if (hasEndBubble && !endGridMatch)
                            {
                                gMatch.ShowBubbleInView(DatumEnds.End1, vMatch);
                            }
                            else if (!hasEndBubble && endGridMatch)
                            {
                                gMatch.HideBubbleInView(DatumEnds.End1, vMatch);
                            }

                            // Match leader start and end
                            Leader leaderStart = gridsTemplate[gId].LeaderStart;
                            if (leaderStart != null)
                            {
                                Leader leaderStartMacth = gMatch.GetLeader(DatumEnds.End0, vMatch);
                                if (leaderStartMacth == null)
                                {
                                    leaderStartMacth = gMatch.AddLeader(DatumEnds.End0, vMatch);
                                }
                                else
                                {
                                    XYZ elbowTemplate = leaderStart.Elbow;
                                    XYZ endTemplate = leaderStart.End;
                                    leaderStartMacth.Elbow = new XYZ(elbowTemplate.X, elbowTemplate.Y, matchOrigin.Z);
                                    leaderStartMacth.End = new XYZ(endTemplate.X, endTemplate.Y, matchOrigin.Z);

                                    gMatch.SetLeader(DatumEnds.End0, vMatch, leaderStartMacth);
                                }
                            }
                            Leader leaderEnd = gridsTemplate[gId].LeaderEnd;
                            if (leaderEnd != null)
                            {
                                Leader leaderEndMatch = gMatch.GetLeader(DatumEnds.End1, vMatch);
                                if (leaderEndMatch == null)
                                {
                                    leaderEndMatch = gMatch.AddLeader(DatumEnds.End1, vMatch);
                                }
                                else
                                {
                                    XYZ elbowTemplate = leaderEnd.Elbow;
                                    XYZ endTemplate = leaderEnd.End;
                                    leaderEndMatch.Elbow = new XYZ(elbowTemplate.X, elbowTemplate.Y, matchOrigin.Z);
                                    leaderEndMatch.End = new XYZ(endTemplate.X, endTemplate.Y, matchOrigin.Z);

                                    gMatch.SetLeader(DatumEnds.End1, vMatch, leaderEndMatch);
                                }
                            }

                            // Match 2D extension
                            DatumExtentType datumExtentTypeStart = gridsTemplate[gId].SelectedGrid
                                .GetDatumExtentTypeInView(DatumEnds.End0, vMatch);
                            DatumExtentType datumExtentTypeEnd = gridsTemplate[gId].SelectedGrid
                                .GetDatumExtentTypeInView(DatumEnds.End1, vMatch);

                            gMatch.SetDatumExtentType(DatumEnds.End0, vMatch, datumExtentTypeStart);
                            gMatch.SetDatumExtentType(DatumEnds.End1, vMatch, datumExtentTypeEnd);
                        }
                    }
                }

                // Copy grid dimensions
                if (copyDims)
                {
                    // Collect dimensions in selected view
                    FilteredElementCollector dimensionsCollector = new FilteredElementCollector(doc, selectedViewId)
                                                            .OfCategory(BuiltInCategory.OST_Dimensions)
                                                            .WhereElementIsNotElementType();

                    // Dimensions to copy
                    List<ElementId> dimsToCopy = new List<ElementId>();

                    // Check dimensions only take grids as references 
                    foreach (Element element in dimensionsCollector)
                    {
                        Dimension d = element as Dimension;
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
                            ElementTransformUtils.CopyElements(selectedViewTemp, dimsToCopy, v, null, cp);
                        }
                    }
                }

                gridTransacation.Commit();
            }
        }
    }
}
