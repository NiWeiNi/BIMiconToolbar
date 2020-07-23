using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

namespace BIMiconToolbar.NumberDoors
{
    [TransactionAttribute(TransactionMode.Manual)]
    class NumberDoors2020 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Create dictionary to store door-room values
            Dictionary<FamilyInstance, string> doorNumbers = new Dictionary<FamilyInstance, string>();
            // Create dictionary to store number of doors in room
            Dictionary<string, int> doorsInRoomCount = new Dictionary<string, int>();

            // Collect all doors in project
            using (FilteredElementCollector doors = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Doors)
                                            .WhereElementIsNotElementType())
            {
                // Retrieve rooms from doors
                foreach (FamilyInstance door in doors)
                {
                    if (door.ToRoom != null)
                    {
                        string roomNumber = door.ToRoom.Number;
                        ClassifyDoors(doorsInRoomCount, roomNumber, doorNumbers, door);
                    }
                    else if (door.FromRoom != null)
                    {
                        string roomNumber = door.FromRoom.Number;
                        ClassifyDoors(doorsInRoomCount, roomNumber, doorNumbers, door);
                    }
                }
            }

            // Create transaction and make changes in Revit
            using (Transaction t = new Transaction(doc, "Number Doors"))
            {
                t.Start();

                // Empty Mark parameter to avoid duplicated values
                foreach (KeyValuePair<FamilyInstance, string> entry in doorNumbers)
                {
                    Parameter doorMark = entry.Key.get_Parameter(BuiltInParameter.DOOR_NUMBER);
                    doorMark.Set("");
                }

                // Populate door numbers in Mark parameter
                foreach (KeyValuePair<FamilyInstance, string> entry in doorNumbers)
                {
                    Parameter doorMark = entry.Key.get_Parameter(BuiltInParameter.DOOR_NUMBER);
                    doorMark.Set(entry.Value);
                }

                t.Commit();
            }

            return Result.Succeeded;
        }

        private void ClassifyDoors(Dictionary<string, int> doorsInRoomCount, string roomNumber,
                     Dictionary<FamilyInstance, string> doorNumbers, FamilyInstance door)
        {
            // Letters to map from integers
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Classify door
            if (doorsInRoomCount.ContainsKey(roomNumber))
            {
                // Check door count is one, a prefix will be added to the number
                if (doorsInRoomCount[roomNumber] == 1)
                {
                    // Retrieve door with single count from the dictionary that stores door-room relationship
                    FamilyInstance keyDoor = doorNumbers.FirstOrDefault(x => x.Value == roomNumber).Key;
                    // Change single count door prefix
                    doorNumbers[keyDoor] = roomNumber + "-" + letters[doorsInRoomCount[roomNumber] - 1];
                    // Store current door and room in dict
                    doorNumbers[door] = roomNumber + "-" + letters[doorsInRoomCount[roomNumber]];
                }
                else
                {
                    // Store current door and room in dict
                    doorNumbers[door] = roomNumber + "-" + letters[doorsInRoomCount[roomNumber]];
                }
                // Increase the door count in room
                doorsInRoomCount[roomNumber] = doorsInRoomCount[roomNumber] + 1;
            }
            else
            {
                // No door is in room , add door-room to dict
                doorNumbers.Add(door, roomNumber);
                doorsInRoomCount.Add(roomNumber, 1);
            }
        }
    }
}
