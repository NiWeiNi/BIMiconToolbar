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
			UIDocument uidoc = commandData.Application.ActiveUIDocument;

			string nameLevel = "Level 1";
			// Retrieve specified level
			FilteredElementCollector collector = new FilteredElementCollector(doc);
			ICollection<Element> levels = collector.OfClass(typeof(Level)).ToElements();
			var level = collector.Where(x => x.Name == nameLevel).FirstOrDefault() as Level;

			// Retrieve rooms from document
			var rooms = new FilteredElementCollector(doc)
						.OfCategory(BuiltInCategory.OST_Rooms)
						.Where(x => x.LevelId == level.LevelId);

            return Result.Succeeded;
        }
    }
}
