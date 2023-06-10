using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.MatchGrids;
using BIMicon.BIMiconToolbar.Models;
using System.Collections.Generic;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Buttons.MatchGrids
{
    internal class MatchGridsModel
    {
        // Private Properties
        private readonly MatchGridsViewModel _viewModel;
        private Document _doc;
        private bool _copyDims;
        private ICollection<BaseElement> _selectedBaseElements;

        private ElementId _selectedViewId;
        private View _selectedViewTemp;
        private IEnumerable<View> _viewsToMatch;

        private FilteredElementCollector _gridsInViewTempCollector;
        private ICollection<ElementId> _gridIds;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="viewModel"></param>
        public MatchGridsModel(MatchGridsViewModel viewModel) 
        {
            _viewModel = viewModel;
        }

        /// <summary>
        /// Method to run logic
        /// </summary>
        /// <returns></returns>
        public Result Execute()
        {
            // Retrieve selected elements from viewModel
            UpdatePropertiesFromViewModel();

            // Check that elements have been selected
            if (_selectedBaseElements == null)
            {
                return Result.Cancelled;
            }
            else
            {
                CollectGrids();
                MatchGrids();
                return Result.Succeeded;
            }
        }

        /// <summary>
        /// Store porperties from viewModel to private porperties in model for easy access
        /// </summary>
        private void UpdatePropertiesFromViewModel()
        {
            // Variables to store user input
            _doc = _viewModel.Doc;
            _copyDims = false;
            _selectedBaseElements = _viewModel.SelectedViews;

            // Retrieve views to be matched
            _viewsToMatch = _selectedBaseElements.Select(bE => _doc.GetElement(new ElementId(bE.Id)) as View);
            _selectedViewId = new ElementId(_viewModel.SelectedViewTemplate.Id);
            _selectedViewTemp = _doc.GetElement(_selectedViewId) as View;
        }

        private void CollectGrids()
        {
            // Collect grids visible in view
            _gridsInViewTempCollector = new FilteredElementCollector(_doc, _selectedViewId)
                                                    .OfCategory(BuiltInCategory.OST_Grids)
                                                    .WhereElementIsNotElementType();
            // Grid Ids
            _gridIds = _gridsInViewTempCollector.ToElementIds();
        }

        private void CopyDimensions()
        {
            // Collect dimensions in selected view
            FilteredElementCollector dimensionsCollector = new FilteredElementCollector(_doc, _selectedViewId)
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

                    if (!_gridIds.Contains(dRefId))
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

                foreach (View v in _viewsToMatch)
                {
                    ElementTransformUtils.CopyElements(_selectedViewTemp, dimsToCopy, v, null, cp);
                }
            }
        }

        private void MatchGridBubble(GridSpecsInView gridSpecs, View vMatch, Grid gridToMatch, DatumEnds datumEnds)
        {
            bool hasBubble;
            bool hasBubbleGridToMatch = gridToMatch.IsBubbleVisibleInView(datumEnds, vMatch);

            if (datumEnds == DatumEnds.End0)
                hasBubble = gridSpecs.StartBubble;
            else
                hasBubble = gridSpecs.EndBubble;

            if (hasBubble && !hasBubbleGridToMatch)
            {
                gridToMatch.ShowBubbleInView(datumEnds, vMatch);
            }
            else if (!hasBubble && hasBubbleGridToMatch)
            {
                gridToMatch.HideBubbleInView(datumEnds, vMatch);
            }
        }

        private void MatchGrid2DExtents(GridSpecsInView gridSpecs, View vMatch, Grid gridToMatch, DatumEnds datumEnds)
        {
            DatumExtentType datumExtent = gridSpecs.SelectedGrid.GetDatumExtentTypeInView(datumEnds, vMatch);
            gridToMatch.SetDatumExtentType(datumEnds, vMatch, datumExtent);
        }

        private void MatchGridLeader(GridSpecsInView gridSpecs, View vMatch, Grid gridToMatch, XYZ matchOrigin, DatumEnds datumEnds)
        {
            Leader leaderEnd;
            if (datumEnds == DatumEnds.End0)
                leaderEnd = gridSpecs.LeaderStart;
            else
                leaderEnd = gridSpecs.LeaderEnd;

            if (leaderEnd != null)
            {
                Leader leaderEndMatch = gridToMatch.GetLeader(datumEnds, vMatch);
                if (leaderEndMatch == null)
                {
                    leaderEndMatch = gridToMatch.AddLeader(datumEnds, vMatch);
                }
                else
                {
                    XYZ elbowTemplate = leaderEnd.Elbow;
                    XYZ endTemplate = leaderEnd.End;
                    leaderEndMatch.Elbow = new XYZ(elbowTemplate.X, elbowTemplate.Y, matchOrigin.Z);
                    leaderEndMatch.End = new XYZ(endTemplate.X, endTemplate.Y, matchOrigin.Z);

                    gridToMatch.SetLeader(datumEnds, vMatch, leaderEndMatch);
                }
            }
        }

        private void MatchGridCurve(XYZ matchOrigin, XYZ originPoint, GridSpecsInView gridSpecs, View vMatch, Grid gridToMatch)
        {
            // Match grid guide line to view direction
            XYZ viewDir = _selectedViewTemp.ViewDirection;
            XYZ dist;

            if (viewDir.Z == 1 || viewDir.Z == -1)
                dist = new XYZ(0, 0, matchOrigin.Z - originPoint.Z);
            else if (viewDir.Y == 1 || viewDir.Y == -1)
                dist = new XYZ(0, matchOrigin.Y - originPoint.Y, 0);
            else
                dist = new XYZ(matchOrigin.X - originPoint.X, 0, 0);

            // Curve defined in view
            IList<Curve> curves = gridSpecs.ListCurve;
            Curve curve = curves[0];

            // Move grid guide line to plane of the view
            Transform trans = Transform.CreateTranslation(dist);
            Curve transCurve = curve.CreateTransformed(trans);
            // Set grid line extensions in view
            gridToMatch.SetCurveInView(DatumExtentType.ViewSpecific, vMatch, transCurve);
        }

        private void MatchGrids()
        {
            // List to store grid display settings
            IEnumerable<GridSpecsInView> gridsTemplates = _gridsInViewTempCollector.Select(e => new GridSpecsInView((e as Grid), _selectedViewTemp));

            // Transaction
            using (Transaction gridTransacation = new Transaction(_doc, "Match grids"))
            {
                gridTransacation.Start();

                foreach (View vMatch in _viewsToMatch)
                {
                    // Collect grids visible in view
                    FilteredElementCollector gridsMatchCollector = new FilteredElementCollector(_doc, vMatch.Id)
                                                            .OfCategory(BuiltInCategory.OST_Grids)
                                                            .WhereElementIsNotElementType();

                    // Match each visible grid in view
                    foreach (Element element in gridsMatchCollector)
                    {
                        ElementId gId = element.Id;
                        Grid gMatch = element as Grid;

                        if (_gridIds.Contains(gId))
                        {
                            GridSpecsInView gridSpecs = gridsTemplates.First(g => g.GridId == gId);

                            // Points from datum plane in grid template and destination grid
                            XYZ matchOrigin = GetPointInCurve(vMatch, gMatch);
                            XYZ originPoint = GetPointInCurve(_selectedViewTemp, gridSpecs.SelectedGrid);

                            // Match grid curve
                            MatchGridCurve(matchOrigin, originPoint, gridSpecs, vMatch, gMatch);

                            // Match start and end bubble
                            MatchGridBubble(gridSpecs, vMatch, gMatch, DatumEnds.End0);
                            MatchGridBubble(gridSpecs, vMatch, gMatch, DatumEnds.End1);

                            // Match start and end leader 
                            MatchGridLeader(gridSpecs, vMatch, gMatch, matchOrigin, DatumEnds.End0);
                            MatchGridLeader(gridSpecs, vMatch, gMatch, matchOrigin, DatumEnds.End1);

                            // Match 2D extension
                            MatchGrid2DExtents(gridSpecs, vMatch, gMatch, DatumEnds.End0);
                            MatchGrid2DExtents(gridSpecs, vMatch, gMatch, DatumEnds.End1);
                        }
                    }
                }

                if (_copyDims)
                    CopyDimensions();

                gridTransacation.Commit();
            }
        }
        private XYZ GetPointInCurve(View view, Grid grid)
        {
            Options optMatch = new Options { View = view };
            GeometryElement geoEleMatch = grid.get_Geometry(optMatch);
            XYZ point = new XYZ();
            foreach (GeometryObject geoObj in geoEleMatch)
            {
                if (geoObj is Line)
                {
                    point = (geoObj as Line).Origin;
                    break;
                }
                else if (geoObj is Arc)
                {
                    point = (geoObj as Arc).Center;
                    break;
                }
            }

            return point;
        }
    }
}