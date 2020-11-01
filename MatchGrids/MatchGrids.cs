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
                selectedView = customWindow.SelectedComboItem.Tag as View;
                selectedIntIds = customWindow.IntegerIds;
            }

            // Collect grids visible in view
            FilteredElementCollector gridsCollector = new FilteredElementCollector(doc, selectedView.Id)
                                                    .OfCategory(BuiltInCategory.OST_Grids)
                                                    .WhereElementIsNotElementType();

            // Template for grids display
            var gridsTemplate = new Dictionary<ElementId, bool[]>();

            // Check each grid
            foreach (Grid g in gridsCollector)
            {
                bool end0 = g.IsBubbleVisibleInView(DatumEnds.End0, selectedView);
                bool end1 = g.IsBubbleVisibleInView(DatumEnds.End1, selectedView);

                bool[] endSettings = { end0, end1 };

                gridsTemplate[g.Id] = endSettings;
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
            Transaction gridTransacation = new Transaction(doc, "Match grids");
            gridTransacation.Start();

            foreach (View vMatch in viewsToMatch)
            {
                foreach (Grid gMatch in gridsCollector)
                {
                    bool end0GridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End0, vMatch);
                    bool end1GridMatch = gMatch.IsBubbleVisibleInView(DatumEnds.End1, vMatch);

                    ElementId gId = gMatch.Id;

                    bool end0Temp = gridsTemplate[gId][0];
                    bool end1Temp = gridsTemplate[gId][1];

                    if (end0Temp == true && end0GridMatch == false)
                    {
                        gMatch.ShowBubbleInView(DatumEnds.End0, vMatch);
                    }
                    else if (end0Temp == false && end0GridMatch)
                    {
                        gMatch.HideBubbleInView(DatumEnds.End0, vMatch);
                    }
                    else if (end1Temp == true && end1GridMatch == false)
                    {
                        gMatch.ShowBubbleInView(DatumEnds.End1, vMatch);
                    }
                    else if (end1Temp == false && end1GridMatch)
                    {
                        gMatch.HideBubbleInView(DatumEnds.End1, vMatch);
                    }
                }
            }

            gridTransacation.Commit();

            return Result.Succeeded;
        }
    }
}
