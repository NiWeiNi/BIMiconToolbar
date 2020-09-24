using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMiconToolbar.Helpers
{
    class Helpers
    {
        public static string WriteSafeReadAllLines(string path)
        {
            using (var csv = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(csv))
            {
                List<string> file = new List<string>();
                while (!sr.EndOfStream)
                {
                    file.Add(sr.ReadLine());
                }

                return String.Join("", file.ToArray());
            }
        }

        public static void numberFamilyInstance(Document doc,
                                                BuiltInCategory builtInCategory)
        {
            // Create dictionary to store window-room values
            Dictionary<FamilyInstance, string> instanceNumbers = new Dictionary<FamilyInstance, string>();
            // Create dictionary to store number of windows in room
            Dictionary<string, int> instancesInRoomCount = new Dictionary<string, int>();

            // Design option filter
            ElementDesignOptionFilter designOptionFilter = new ElementDesignOptionFilter(ElementId.InvalidElementId);

            // Select phase
            PhaseArray aPhase = doc.Phases;
            Phase phase = aPhase.get_Item(aPhase.Size - 1);

            ElementId idPhase = phase.Id;
            ParameterValueProvider provider = new ParameterValueProvider(new ElementId((int)BuiltInParameter.PHASE_CREATED));
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            FilterElementIdRule rule = new FilterElementIdRule(provider, evaluator, idPhase);
            ElementParameterFilter paraFilter = new ElementParameterFilter(rule);

            // Collect all windows in project
            using (FilteredElementCollector instances = new FilteredElementCollector(doc).OfCategory(builtInCategory)
                                            .WhereElementIsNotElementType().WherePasses(designOptionFilter).WherePasses(paraFilter))
            {
                // Retrieve rooms from windows
                foreach (FamilyInstance inst in instances)
                {
                    if (inst.ToRoom != null)
                    {
                        string roomNumber = inst.ToRoom.Number;
                        Helpers.InstanceFromToRoom(instancesInRoomCount, roomNumber, instanceNumbers, inst);
                    }
                    else if (inst.FromRoom != null)
                    {
                        string roomNumber = inst.FromRoom.Number;
                        Helpers.InstanceFromToRoom(instancesInRoomCount, roomNumber, instanceNumbers, inst);
                    }
                }
            }

            // Create transaction and make changes in Revit
            using (Transaction t = new Transaction(doc, "Number Instances"))
            {
                t.Start();

                // Empty Mark parameter to avoid duplicated values
                foreach (KeyValuePair<FamilyInstance, string> entry in instanceNumbers)
                {
                    Parameter instanceMark = entry.Key.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                    instanceMark.Set("");
                }

                // Populate Mark parameter
                foreach (KeyValuePair<FamilyInstance, string> entry in instanceNumbers)
                {
                    Parameter instanceMark = entry.Key.get_Parameter(BuiltInParameter.ALL_MODEL_MARK);
                    instanceMark.Set(entry.Value);
                }

                t.Commit();
            }
        }

        public static void InstanceFromToRoom(Dictionary<string, int> instanceInRoomCount,
                                              string roomNumber,
                                              Dictionary<FamilyInstance, string> instanceNumbers,
                                              FamilyInstance familyInstance)
        {
            // Letters to map from integers
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Classify family instances
            if (instanceInRoomCount.ContainsKey(roomNumber))
            {
                // Check instance count is one, a prefix will be added to the number
                if (instanceInRoomCount[roomNumber] == 1)
                {
                    // Retrieve instance with single count from the dictionary that stores instance-room relationship
                    FamilyInstance keyInstance = instanceNumbers.FirstOrDefault(x => x.Value == roomNumber).Key;
                    // Change single count instance prefix
                    instanceNumbers[keyInstance] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber] - 1];
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber]];
                }
                else
                {
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber]];
                }
                // Increase the instance count in room
                instanceInRoomCount[roomNumber] = instanceInRoomCount[roomNumber] + 1;
            }
            else
            {
                // No instance is in room , add instance-room to dict
                instanceNumbers.Add(familyInstance, roomNumber);
                instanceInRoomCount.Add(roomNumber, 1);
            }
        }
    }
}
