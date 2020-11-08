using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;

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

            // Prompt window to collect user input
            using (InteriorElevationsWindow customWindow = new InteriorElevationsWindow(commandData))
            {
                customWindow.ShowDialog();
                selectedIntIds = customWindow.IntegerIds;
            }

            if (selectedIntIds != null)
            {
                // Collect rooms
                foreach(int id in selectedIntIds)
                {
                    Room room = doc.GetElement(new ElementId(id)) as Room;

                    // Retrieve boundaries
                    IList<IList<BoundarySegment>> boundaries = Helpers.Helpers.SpatialBoundaries(room);

                    if (boundaries != null)
                    {
                        List<XYZ> points = Helpers.Helpers.BoundaPoints(boundaries);
                        XYZ centroid = Helpers.Helpers.Centroid(points); 

                        if (boundaries.Count == 4)
                        {
                    
                        }
                    }
                }

            }

            return Result.Succeeded;
        }
    }
}
