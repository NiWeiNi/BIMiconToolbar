using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMiconToolbar.NumberWindows
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberWindows : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Create dictionary to store window-room values
            Dictionary<FamilyInstance, string> windowsNumbers = new Dictionary<FamilyInstance, string>();
            // Create dictionary to store number of windows in room
            Dictionary<string, int> windowsInRoomCount = new Dictionary<string, int>();

            // Select phase
            PhaseArray aphase = doc.Phases;
            Phase phase = aphase.get_Item(aphase.Size - 1);

            ElementId idPhase = phase.Id;
            ParameterValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterElementIdRule rule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter parafilter = new ElementParameterFilter(rule);

            // Collect all windows in project
            using (FilteredElementCollector windows = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Windows)
                                            .WhereElementIsNotElementType().WherePasses(parafilter))
            {
                // Retrieve rooms from windows
                foreach (FamilyInstance window in windows)
                {
                    if (window.ToRoom != null)
                    {
                        string roomNumber = window.ToRoom.Number;
                        Helpers.Helpers.InstanceFromToRoom(ref windowsInRoomCount, roomNumber, ref windowsNumbers, window);
                    }
                    else if (window.FromRoom != null)
                    {
                        string roomNumber = window.FromRoom.Number;
                        Helpers.Helpers.InstanceFromToRoom(ref windowsInRoomCount, roomNumber, ref windowsNumbers, window);
                    }
                }
            }

            // Create transaction and make changes in Revit
            using (Transaction t = new Transaction(doc, "Number Windows"))
            {
                t.Start();

                // Empty Mark parameter to avoid duplicated values
                foreach (KeyValuePair<FamilyInstance, string> entry in windowsNumbers)
                {
                    Parameter windowMark = entry.Key.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                    windowMark.Set("");
                }

                // Populate Mark parameter
                foreach (KeyValuePair<FamilyInstance, string> entry in windowsNumbers)
                {
                    Parameter windowMark = entry.Key.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                    windowMark.Set(entry.Value);
                }

                t.Commit();
            }

            return Result.Succeeded;
        }
    }
}
