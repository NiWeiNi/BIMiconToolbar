using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMiconToolbar.NumberBySpline
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberBySpline : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
			Document doc = commandData.Application.ActiveUIDocument.Document;

            // Call WPF for user input
            using (NumberBySplineWPF customWindow = new NumberBySplineWPF(commandData))
            {
                // Revit application as window's owner
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();

                ElementId curveId = customWindow.CurveId;
                CurveElement eCurve = doc.GetElement(curveId) as CurveElement;
                Curve curve = eCurve.GeometryCurve as Curve;

                XYZ[] points = Helpers.HelpersGeometry.DivideEquallySpline(curve, 50);

                // Place a marker circle at each point.
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Draw Curves at Points");

                    foreach (XYZ pt in points)
                    {
                        Helpers.HelpersGeometry.CreateCircle(doc, pt, 1);
                    }

                    tx.Commit();
                }
            }

            return Result.Succeeded;
        }
    }
}
