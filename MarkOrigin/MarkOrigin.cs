using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;
using BIMiconToolbar.Helpers;

namespace BIMiconToolbar.MarkOrigin
{
    [TransactionAttribute(TransactionMode.Manual)]
    class MarkOrigin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Retrieve active View
            View activeView = doc.ActiveView;
            ViewType actViewType = activeView.ViewType;

            // Array of view types excluded
            ViewType[] viewTypes = { ViewType.Report, ViewType.Schedule, ViewType.ThreeD, ViewType.Walkthrough,
                ViewType.Undefined};

            // Check current view has origin and it can be drawn on a plane
            if (viewTypes.Contains(actViewType) == false)
            {
                // Retieve normal to view
                XYZ normal = activeView.ViewDirection;

                // Define horizontal and vertical projection length
                double length = 10;
                double x = 0;
                double y = 0;
                double z = 0;

                // Assing value to coordinates that are not perpendicular to view
                if (normal.X == 0)
                {
                    x = length;
                }
                if (normal.Y == 0)
                {
                    y = length;
                }
                if (normal.Z == 0)
                {
                    z = length;
                }

                // Create points coordinates
                XYZ origin = XYZ.Zero;

                XYZ p1 = new XYZ(x, y, z);
                XYZ p2 = new XYZ(-x, -y, -z);
                XYZ p3 = new XYZ(-x, y, -z);
                XYZ p4 = new XYZ(x, -y, z);


                Transaction t = new Transaction(doc, "Creteate Origin Marker");
                t.Start();

                // Create line
                Line diagonal1 = Line.CreateBound(p1, p2);
                Line diagonal2 = Line.CreateBound(p3, p4);

                doc.Create.NewDetailCurve(activeView, diagonal1);
                doc.Create.NewDetailCurve(activeView, diagonal2);

                t.Commit();

                return Result.Succeeded;
            }
            else
            {
                MessageWindows.AlertMessage("Error", "ViewType not supported, please open a supported View \nlike Plan, Section or Drafting View.");

                return Result.Failed;
            }
        }
    }
}
