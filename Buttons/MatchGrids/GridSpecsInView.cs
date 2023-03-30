using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMicon.BIMiconToolbar.MatchGrids
{
    internal class GridSpecsInView
    {
        public bool StartBubble { get; }
        public bool EndBubble { get; }
        public Grid SelectedGrid { get; }
        public ElementId GridId { get; }
        public IList<Curve> ListCurve { get; }
        public XYZ OriginPoint { get; }
        public Curve UnderlyingCurve { get; }
        public Leader LeaderStart { get; }
        public Leader LeaderEnd { get; }

        public GridSpecsInView(Grid grid, View view)
        {
            GridId = grid.Id;
            ListCurve = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view);
            SelectedGrid = grid;
            StartBubble = grid.IsBubbleVisibleInView(DatumEnds.End0, view);
            EndBubble = grid.IsBubbleVisibleInView(DatumEnds.End1, view);
            LeaderStart = grid.GetLeader(DatumEnds.End0, view);
            LeaderEnd = grid.GetLeader(DatumEnds.End1, view);

            // Origin grid selected
            Options options = new Options
            {
                View = view
            };
            GeometryElement geoEle = grid.get_Geometry(options);
            foreach (GeometryObject geoObj in geoEle)
            {
                Line line = geoObj as Line;
                if (line != null)
                {
                    UnderlyingCurve = line as Curve;
                    OriginPoint = line.Origin;
                    break;
                }
            }
        }
    }
}
