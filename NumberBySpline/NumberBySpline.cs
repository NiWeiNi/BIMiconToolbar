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

                XYZ[] points = Helpers.HelpersGeometry.DivideEquallySpline(curve, 150);

                // Retrieve elements of selected category
                Category cat = customWindow.SelectedComboItemCategories.Tag as Category;
                Level level = customWindow.SelectedComboItemLevels.Tag as Level;
                string startNumber = customWindow.StartNumber;

                ElementLevelFilter levelFilter = new ElementLevelFilter(level.Id);
                FilteredElementCollector collectElements = new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType();

                // Create two list that contains all selected elements
                List<Element> selElements = new List<Element>();
                List<Element> selElementsCopy = new List<Element>();

                foreach (Element element in collectElements)
                {
                    if (element.LevelId == level.Id)
                    {
                        selElements.Add(element);
                        selElementsCopy.Add(element);
                    }
                }

                // Renumber selected elements
                using (Transaction tx = new Transaction(doc))
                {
                    tx.Start("Draw Curves at Points");

                    int number = int.Parse(startNumber);

                    // Loop through each point to check if it is inside the selected elements
                    foreach (XYZ point in points)
                    {
                        // Inner loop to check point is inside the element, if so, remove element from list and proceed until end of list
                        for (int j = 0; j < selElementsCopy.Count; j++)
                        {
                            // Retrieve bounding box of element
                            BoundingBoxXYZ bBox = selElementsCopy[j].get_BoundingBox(null);
                            int intersResult = Helpers.HelpersGeometry.IsPointInsideRectangle(point, bBox.Min, bBox.Max);

                            if (intersResult != 0)
                            {
                                Parameter param = selElementsCopy[j].LookupParameter("Number");
                                param.Set("R" + number.ToString());
                                selElementsCopy.Remove(selElementsCopy[j]);

                                number++;
                                break;
                            }
                        }
                    }
                    tx.Commit();
                }
                return Result.Succeeded;
            }
        }
    }
}
