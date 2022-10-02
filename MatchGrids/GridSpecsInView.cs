using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMiconToolbar.MatchGrids
{
    internal class GridSpecsInView
    {
        public bool StartBubble { get; }
        public bool EndBubble { get; }
        public Grid SelectedGrid { get; }
        public ElementId GridId { get; }
        public IList<Curve> ListCurve { get; }

        public GridSpecsInView(Grid grid, View view)
        {
            GridId = grid.Id;
            ListCurve = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view);
            SelectedGrid = grid;
            StartBubble = grid.IsBubbleVisibleInView(DatumEnds.End0, view);
            EndBubble = grid.IsBubbleVisibleInView(DatumEnds.End1, view);
        }
    }
}
