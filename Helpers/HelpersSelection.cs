using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace BIMiconToolbar.Helpers
{
    class HelpersSelection
    {
		/// <summary>
		/// Function for user to pick up line
		/// </summary>
		/// <param name="uidoc"></param>
		/// <returns></returns>
		public Curve PickLine(UIDocument uidoc)
		{
			Document doc = uidoc.Document;

			// Selection filter
			ISelectionFilter selFilter = new LineSelectionFilter();

			try
			{
				// Prompt user to select curve that intersects with rooms
				ElementId curveId = uidoc.Selection.PickObject(ObjectType.Element, selFilter).ElementId;

				// Retrieve model curve
				CurveElement eCurve = doc.GetElement(curveId) as CurveElement;
				Curve curve = eCurve.GeometryCurve as Curve;

				// TaskDialog.Show("Curve Picked", curve.Name);
				return curve;
			}
			catch (Autodesk.Revit.Exceptions.OperationCanceledException e)
			{
				TaskDialog.Show("Operation Canceled", e.Message);
				return null;
			}
		}

		/// <summary>
		/// Class filter to pick up only lines
		/// </summary>
		public class LineSelectionFilter : ISelectionFilter
		{
			public bool AllowElement(Element element)
			{
				if (element.Category.Name == "Lines")
				{
					return true;
				}
				return false;
			}
			// Overrride AllowReference method
			public bool AllowReference(Reference refer, XYZ point)
			{
				return false;
			}
		}
	}
}
