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
                var rooms = new List<Room>();

                foreach(int id in selectedIntIds)
                {
                    Room room = doc.GetElement(new ElementId(id)) as Room;
                    rooms.Add(room);
                }

                // Retrieve boudnaries
                IList<IList<BoundarySegment>> segments = rooms[0].GetBoundarySegments(new SpatialElementBoundaryOptions());

                if (segments != null)
                {
                

                    foreach (IList<BoundarySegment> segmentList in segments)
                    { 
                
                    }
                    

                    if (segments.Count == 4)
                    {
                    
                    }
                }
            }

            return Result.Succeeded;
        }
    }
}
