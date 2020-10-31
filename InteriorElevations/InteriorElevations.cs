using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMiconToolbar.InteriorElevations
{
    [TransactionAttribute(TransactionMode.Manual)]
    class InteriorElevations : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Collect rooms
            Room room = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms).FirstOrDefault() as Room;

            // Retrieve boudnaries
            IList<IList<BoundarySegment>> segments = room.GetBoundarySegments(new SpatialElementBoundaryOptions());

            if (segments != null)
            {
                

                foreach (IList<BoundarySegment> segmentList in segments)
                { 
                
                }
                    

                if (segments.Count == 4)
                {
                    
                }
            }

            TaskDialog.Show("Warning", "Sorry, the tool is WIP");

            return Result.Succeeded;
        }
    }
}
