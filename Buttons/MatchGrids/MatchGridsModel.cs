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
        private BaseElement _selectedView;
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
            _selectedView = _viewModel.SelectedViewTemplate;
            _copyDims = false;
            _selectedBaseElements = _viewModel.SelectedViews;

            // Retrieve views to be matched
            _viewsToMatch = _selectedBaseElements.Select(bE => _doc.GetElement(new ElementId(bE.Id)) as View);
            _selectedViewId = new ElementId(_selectedView.Id);
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
                            bool startGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End0, vMatch);
                            bool endGridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End1, vMatch);

                            GridSpecsInView gridSpecs = gridsTemplates.First(g => g.GridId == gId);

                            bool hasStartBubble = gridSpecs.StartBubble;
                            bool hasEndBubble = gridSpecs.EndBubble;

                            IList<Curve> curves = gridSpecs.ListCurve;
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
                                View = _selectedViewTemp
                            };
                            GeometryElement geoEle = gridSpecs.SelectedGrid.get_Geometry(options);
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
                            XYZ viewDir = _selectedViewTemp.ViewDirection;
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
                            Leader leaderStart = gridSpecs.LeaderStart;
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
                            Leader leaderEnd = gridSpecs.LeaderEnd;
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
                            DatumExtentType datumExtentTypeStart = gridSpecs.SelectedGrid
                                .GetDatumExtentTypeInView(DatumEnds.End0, vMatch);
                            DatumExtentType datumExtentTypeEnd = gridSpecs.SelectedGrid
                                .GetDatumExtentTypeInView(DatumEnds.End1, vMatch);

                            gMatch.SetDatumExtentType(DatumEnds.End0, vMatch, datumExtentTypeStart);
                            gMatch.SetDatumExtentType(DatumEnds.End1, vMatch, datumExtentTypeEnd);
                        }
                    }
                }

                if (_copyDims)
                    CopyDimensions();

                gridTransacation.Commit();
            }
        }
    }
}
