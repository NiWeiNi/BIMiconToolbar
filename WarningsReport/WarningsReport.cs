using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BIMiconToolbar.WarningsReport
{
    [TransactionAttribute(TransactionMode.Manual)]
    class WarningsReport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            string warningJSONPath = @"C:\ProgramData\Autodesk\Revit\Addins\2019\WarningsReport\RevitWarningsClassified.json";
            string warningsJsonString = Helpers.Helpers.WriteSafeReadAllLines(warningJSONPath);
            var warningsJObject = JObject.Parse(warningsJsonString);

            string critical = string.Join("", warningsJObject.Value<JArray>("Critical").ToObject<string[]>());
            string high = string.Join("", warningsJObject.Value<JArray>("High").ToObject<string[]>());
            string medium = string.Join("", warningsJObject.Value<JArray>("Medium").ToObject<string[]>());
            string low = string.Join("", warningsJObject.Value<JArray>("Low").ToObject<string[]>());

            IList<FailureMessage> docWarnings = doc.GetWarnings();

            // Store data to transfer to database
            List<string[]> dataTransfer = new List<string[]>();

            foreach (FailureMessage failMessage in docWarnings)
            {
                string failDescription = failMessage.GetDescriptionText();
                ICollection<ElementId> failWarningElementIds = failMessage.GetFailingElements();
                string failElementIds = string.Join(", ", failWarningElementIds);

                if (critical.Contains(failDescription))
                {
                    dataTransfer.Add(new string[]{
                                                "Critical",
                                                failDescription,
                                                failElementIds
                    });
                }
                else if (high.Contains(failDescription))
                {
                    dataTransfer.Add(new string[]{
                                                "High",
                                                failDescription,
                                                failElementIds
                    });
                }
                else if (low.Contains(failDescription))
                {
                    dataTransfer.Add(new string[]{
                                                "Low",
                                                failDescription,
                                                failElementIds
                    });
                }
                else
                {
                    dataTransfer.Add(new string[]{
                                                "Medium",
                                                failDescription,
                                                failElementIds
                    });
                }
            }

            // Store results
            var csv = new StringBuilder();

            foreach (string[] dataArray in dataTransfer)
            {
                try
                {
                    csv.AppendLine(string.Join("---", dataArray));
                }
                catch
                {
                    // TODO: Log error
                }
            }

            // Path to output data
            string pathOut = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Warnings Report.csv";
            // Write output
            File.AppendAllText(pathOut, csv.ToString());

            return Result.Succeeded;
        }
    }
}