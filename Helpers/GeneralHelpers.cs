using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class GeneralHelpers
    {
        /// <summary>
        /// Check if an array is null or empty
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(Array array)
        {
            return (array == null || array.Length == 0);
        }

        /// <summary>
        /// A function to read and store content of a file even if it is in use
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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

        /// <summary>
        /// A function to number Family Instances
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="builtInCategory"></param>
        public static void numberFamilyInstance(Document doc,
                                                Phase phase,
                                                Boolean numeric,
                                                string separator,
                                                BuiltInCategory builtInCategory,
                                                ref int countInstances,
                                                Parameter parameter)
        {
            // Create dictionary to store window-room values
            Dictionary<FamilyInstance, string> instanceNumbers = new Dictionary<FamilyInstance, string>();
            // Create dictionary to store number of windows in room
            Dictionary<string, int> instancesInRoomCount = new Dictionary<string, int>();

            // Design option filter
            ElementDesignOptionFilter designOptionFilter = new ElementDesignOptionFilter(ElementId.InvalidElementId);

            // Collect all instances in phase in project
            ElementId idPhase = phase.Id;
            var instances = new FilteredElementCollector(doc).OfCategory(builtInCategory)
                                            .WhereElementIsNotElementType().WherePasses(designOptionFilter).Where(e => (e as Element).CreatedPhaseId == idPhase);

            string roomNumber = "";
            // Retrieve rooms from windows
            foreach (FamilyInstance inst in instances)
            {
                if (builtInCategory == BuiltInCategory.OST_Doors)
                {
                    if (inst.ToRoom != null)
                    {
                        roomNumber = inst.ToRoom.Number;
                    }
                    else if (inst.FromRoom != null)
                    {
                        roomNumber = inst.FromRoom.Number;
                    }
                }
                else
                {
                    if (inst.FromRoom != null)
                    {
                        roomNumber = inst.FromRoom.Number;
                    }
                    else if (inst.ToRoom != null)
                    {
                        roomNumber = inst.ToRoom.Number;
                    }
                }
                if (numeric)
                {
                    GeneralHelpers.InstanceFromToRoomNumber(instancesInRoomCount, roomNumber, separator, instanceNumbers, inst);
                }
                else
                {
                    GeneralHelpers.InstanceFromToRoom(instancesInRoomCount, roomNumber, separator, instanceNumbers, inst);
                }
            }
            
            // Create transaction and make changes in Revit
            using (Transaction t = new Transaction(doc, "Number Instances"))
            {
                t.Start();

                // Empty Mark parameter to avoid duplicated values
                foreach (KeyValuePair<FamilyInstance, string> entry in instanceNumbers)
                {
                    Parameter instanceMark = entry.Key.LookupParameter(parameter.Definition.Name);
                    instanceMark.Set("");
                }

                // Populate Mark parameter
                foreach (KeyValuePair<FamilyInstance, string> entry in instanceNumbers)
                {
                    Parameter instanceMark = entry.Key.LookupParameter(parameter.Definition.Name);
                    if (entry.Value != "")
                    {
                        instanceMark.Set(entry.Value);
                        countInstances += 1;
                    }
                }

                t.Commit();
            }
        }

        /// <summary>
        /// A function to count number of windows or doors in room
        /// </summary>
        /// <param name="instanceInRoomCount"></param>
        /// <param name="roomNumber"></param>
        /// <param name="instanceNumbers"></param>
        /// <param name="familyInstance"></param>
        public static void InstanceFromToRoom(Dictionary<string, int> instanceInRoomCount,
                                              string roomNumber,
                                              string separator,
                                              Dictionary<FamilyInstance, string> instanceNumbers,
                                              FamilyInstance familyInstance)
        {
            // Classify family instances
            if (instanceInRoomCount.ContainsKey(roomNumber))
            {
                // Check instance count is one, a prefix will be added to the number
                if (instanceInRoomCount[roomNumber] == 1)
                {
                    // Retrieve instance with single count from the dictionary that stores instance-room relationship
                    FamilyInstance keyInstance = instanceNumbers.FirstOrDefault(x => x.Value == roomNumber).Key;
                    // Change single count instance prefix
                    instanceNumbers[keyInstance] = roomNumber + separator + numberToLetter(instanceInRoomCount[roomNumber] - 1);
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + separator + numberToLetter(instanceInRoomCount[roomNumber]);
                }
                else
                {
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + separator + numberToLetter(instanceInRoomCount[roomNumber]);
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

        /// <summary>
        /// A function to count number of windows or doors in room
        /// </summary>
        /// <param name="instanceInRoomCount"></param>
        /// <param name="roomNumber"></param>
        /// <param name="instanceNumbers"></param>
        /// <param name="familyInstance"></param>
        public static void InstanceFromToRoomNumber(Dictionary<string, int> instanceInRoomCount,
                                              string roomNumber,
                                              string separator,
                                              Dictionary<FamilyInstance, string> instanceNumbers,
                                              FamilyInstance familyInstance)
        {
            // Classify family instances
            if (instanceInRoomCount.ContainsKey(roomNumber))
            {
                // Check instance count is one, a prefix will be added to the number
                if (instanceInRoomCount[roomNumber] == 1)
                {
                    // Retrieve instance with single count from the dictionary that stores instance-room relationship
                    FamilyInstance keyInstance = instanceNumbers.FirstOrDefault(x => x.Value == roomNumber).Key;
                    // Change single count instance prefix
                    instanceNumbers[keyInstance] = roomNumber + separator + (instanceInRoomCount[roomNumber]).ToString();
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + separator + (instanceInRoomCount[roomNumber] + 1).ToString();
                }
                else
                {
                    // Store current instance and room in dict
                    instanceNumbers[familyInstance] = roomNumber + separator + (instanceInRoomCount[roomNumber] + 1).ToString();
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

        /// <summary>
        /// Map a number to a letter
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        static string numberToLetter(int index)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (index >= letters.Length)
            {
                int orderPrefix = index / letters.Length;
                index = index % letters.Length;

                value += orderPrefix.ToString();
            }

            value += letters[index];

            return value;
        }

        /// <summary>
        /// A function to check if view is on any sheet
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static bool IsViewOnSheet(Document doc, View view)
        {
            // Element Id of view to compare
            ElementId viewId = view.Id;

            // Select all sheets
            List<ViewSheet> sheets = new FilteredElementCollector(doc)
                                    .OfClass(typeof(ViewSheet))
                                    .WhereElementIsNotElementType()
                                    .Cast<ViewSheet>()
                                    .Where(i => i.IsPlaceholder == false)
                                    .ToList();

            // Check if views placed on sheet match view
            foreach (ViewSheet sheet in sheets)
            {
                // Collect all views on sheets
                ISet<ElementId> viewsOnSheets = sheet.GetAllPlacedViews();

                // Compare views on sheet to view to check
                foreach (ElementId eId in viewsOnSheets)
                {
                    if (eId == viewId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Check if Uri is valid
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool IsValidUri(string uri)
        {
            if (!Uri.IsWellFormedUriString(uri, UriKind.Absolute))
                return false;
            if (!Uri.TryCreate(uri, UriKind.Absolute, out Uri tmp))
                return false;
            return tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps;
        }

        /// <summary>
        /// Open Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static bool OpenUri(string uri)
        {
            if (!IsValidUri(uri))
                return false;
            System.Diagnostics.Process.Start(uri);
            return true;
        }

        /// <summary>
        /// Retrieve spatial element boundaries
        /// </summary>
        /// <param name="spaEl"></param>
        /// <returns></returns>
        public static IList<IList<BoundarySegment>> SpatialBoundaries(SpatialElement spaEl)
        {
            var boundaries = spaEl.GetBoundarySegments(new SpatialElementBoundaryOptions());
            return boundaries;
        }

        /// <summary>
        /// Function to extract points from boundary
        /// </summary>
        /// <param name="segs"></param>
        /// <returns></returns>
        public static List<XYZ> BoundaPoints(IList<IList<BoundarySegment>> boundary)
        {
            var points = new List<XYZ>();

            foreach(var segments in boundary)
            {
                foreach(var s in segments)
                {
                    Curve curve = s.GetCurve();
                    XYZ point = curve.GetEndPoint(0);

                    points.Add(point);
                }
            }

            return points;
        }

        /// <summary>
        /// Function to retrieve centroid
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static XYZ Centroid(List<XYZ> points)
        {
            // Retrieve single coordinates
            IEnumerable<double> xCoor = from XYZ point in points select point.X;
            IEnumerable<double> yCoor = from XYZ point in points select point.Y;
            IEnumerable<double> zCoor = from XYZ point in points select point.Z;

            int pointsCount = points.Count;

            // Center coordinates
            double xCenter = xCoor.Sum() / pointsCount;
            double yCenter = yCoor.Sum() / pointsCount;
            double zCenter = zCoor.Sum() / pointsCount;

            return new XYZ(xCenter, yCenter, zCenter);
        }

        /// <summary>
        /// Method to convert millimeters to feet
        /// </summary>
        /// <param name="millimeters"></param>
        /// <returns></returns>
        public static double MillimetersToFeet(double millimeters)
        {
            double feet;

            if (millimeters != 0)
            {
                feet = millimeters / 304.8;
                return feet;
            }
            else
            {
                throw new DivideByZeroException();
            }
        }

        /// <summary>
        /// Method to convert feet to milimeters
        /// </summary>
        /// <param name="feet"></param>
        /// <returns></returns>
        public static double FeetToMillimeters(double feet)
        {
            double millimeters;

            millimeters = feet * 304.8;
            return millimeters;
        }

        /// <summary>
        /// Method to check if a polygon is a rectangle
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static bool IsRectangle(IList<BoundarySegment> curves)
        {
            // If there are more than 4 sides or it is empty, not a rectangle
            if (curves.Count > 4 || curves == null)
            {
                return false;
            }

            // Create list of vertices
            List<XYZ> vertices = new List<XYZ>();

            foreach (var curve in curves)
            {
                vertices.Add(curve.GetCurve().GetEndPoint(0));
            }

            // Check the distance from center to vertices are equal
            int round = 10;

            double cx, cy;
            double dd1, dd2, dd3, dd4;

            cx = (vertices[0].X + vertices[1].X + vertices[2].X + vertices[3].X) / 4;
            cy = (vertices[0].Y + vertices[1].Y + vertices[2].Y + vertices[3].Y) / 4;

            dd1 = Math.Round(Math.Pow(cx - vertices[0].X, 2) + Math.Pow(cy - vertices[0].Y, 2), round);
            dd2 = Math.Round(Math.Pow(cx - vertices[1].X, 2) + Math.Pow(cy - vertices[1].Y, 2), round);
            dd3 = Math.Round(Math.Pow(cx - vertices[2].X, 2) + Math.Pow(cy - vertices[2].Y, 2), round);
            dd4 = Math.Round(Math.Pow(cx - vertices[3].X, 2) + Math.Pow(cy - vertices[3].Y, 2), round);

            return dd1 == dd2 && dd1 == dd3 && dd1 == dd4;
        }

        /// <summary>
        /// Function to calculate angle between two vectors
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static double AngleTwoVectors(XYZ v1, XYZ v2)
        {
            double crossProduct = v1.X * v2.X + v1.Y * v2.Y;

            double v1Module = Math.Sqrt(Math.Pow(v1.X, 2) + Math.Pow(v1.Y, 2));
            double v2Module = Math.Sqrt(Math.Pow(v2.X, 2) + Math.Pow(v2.Y, 2));

            double angle = Math.Acos(crossProduct / (v1Module * v2Module));

            return angle;
        }

        /// <summary>
        /// Return list of annotation categories in document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> AnnoCatIds(Document doc)
        {
            // Get settings of current document
            Settings documentSettings = doc.Settings;

            // Retrieve annotation categories
            Categories cats = documentSettings.Categories;

            var annoCategories = new List<ElementId>();

            foreach (Category cat in cats)
            {
                if (cat.CategoryType == CategoryType.Annotation)
                {
                    annoCategories.Add(cat.Id);
                }
            }

            return annoCategories;
        }
    }
}
