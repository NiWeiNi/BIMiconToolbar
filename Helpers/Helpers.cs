using Autodesk.Revit.DB;
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

        public static void InstanceFromToRoom(ref Dictionary<string, int> instanceInRoomCount,
                                              string roomNumber,
                                              ref Dictionary<FamilyInstance, string> instanceNumbers,
                                              FamilyInstance familyInstance)
        {
            // Letters to map from integers
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            // Classify door
            if (instanceInRoomCount.ContainsKey(roomNumber))
            {
                // Check door count is one, a prefix will be added to the number
                if (instanceInRoomCount[roomNumber] == 1)
                {
                    // Retrieve door with single count from the dictionary that stores door-room relationship
                    FamilyInstance keyDoor = instanceNumbers.FirstOrDefault(x => x.Value == roomNumber).Key;
                    // Change single count door prefix
                    instanceNumbers[keyDoor] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber] - 1];
                    // Store current door and room in dict
                    instanceNumbers[familyInstance] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber]];
                }
                else
                {
                    // Store current door and room in dict
                    instanceNumbers[familyInstance] = roomNumber + "-" + letters[instanceInRoomCount[roomNumber]];
                }
                // Increase the door count in room
                instanceInRoomCount[roomNumber] = instanceInRoomCount[roomNumber] + 1;
            }
            else
            {
                // No door is in room , add door-room to dict
                instanceNumbers.Add(familyInstance, roomNumber);
                instanceInRoomCount.Add(roomNumber, 1);
            }
        }
    }
}
