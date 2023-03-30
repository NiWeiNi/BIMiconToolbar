using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using Newtonsoft.Json.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMicon.BIMiconToolbar.WarningsReport
{
    [TransactionAttribute(TransactionMode.Manual)]
    class WarningsReport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Check if the open document is a Family document
            try
            {
                var check = doc.FamilyManager;
                TaskDialog.Show("Warning", "Family document opened, please open a project");
                return Result.Cancelled;
            }
            catch
            {
                // TODO: refactor Family document check
            }

            // Retrieve current date
            string currentDate = DateTime.Today.ToString("dd/MM/yyyy");

            string[] columnNames = { "Priority", "Warning", "Element Ids", "Date Detected", "Date Solved", "Fixed by" };

            string warningJSONPath = @"C:\ProgramData\Autodesk\Revit\Addins\BIMicon\RevitWarningsClassified.json";
            string warningsJsonString = GeneralHelpers.WriteSafeReadAllLines(warningJSONPath);
            var warningsJObject = JObject.Parse(warningsJsonString);

            string critical = string.Join("", warningsJObject.Value<JArray>("Critical").ToObject<string[]>());
            string high = string.Join("", warningsJObject.Value<JArray>("High").ToObject<string[]>());
            string medium = string.Join("", warningsJObject.Value<JArray>("Medium").ToObject<string[]>());
            string low = string.Join("", warningsJObject.Value<JArray>("Low").ToObject<string[]>());

            IList<FailureMessage> docWarnings = doc.GetWarnings();

            // Check if there is any warning in the document
            if (docWarnings.Count == 0)
            {
                TaskDialog.Show("Warning", "This project doesn't contain any warnings. Congratulations!");
                return Result.Succeeded;
            }

            // Store data to transfer to database
            List<string[]> dataTransfer = new List<string[]>();

            foreach (FailureMessage failMessage in docWarnings)
            {
                string failDescription = failMessage.GetDescriptionText();
                ICollection<ElementId> failWarningElementIds = failMessage.GetFailingElements();
                string failElementIds = string.Join(", ", failWarningElementIds);
                string priorityCat = "";

                if (critical.Contains(failDescription))
                {
                    priorityCat = "Critical";
                }
                else if (high.Contains(failDescription))
                {
                    priorityCat = "High";
                }
                else if (low.Contains(failDescription))
                {
                    priorityCat = "Low";
                }
                else
                {
                    priorityCat = "Medium";
                }
                dataTransfer.Add(new string[]{
                                                priorityCat,
                                                failDescription,
                                                failElementIds,
                                                currentDate
                    });
            }

            // Path to output data
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string excelPath = desktopPath + @"\Warnings Report.xlsx";

            // Create excel file
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            SXSSFSheet excelSheet = (SXSSFSheet)workbook.CreateSheet("Sheet1");
            excelSheet.SetRandomAccessWindowSize(100);

            //Create a header row
            IRow row = excelSheet.CreateRow(0);

            // Style for header
            var titleHeader = workbook.CreateFont();
            titleHeader.FontHeightInPoints = 12;
            titleHeader.IsBold = true;
            ICellStyle boldStyle = workbook.CreateCellStyle();
            boldStyle.SetFont(titleHeader);

            // Write to excel
            using (var fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                // Write header
                for (int i = 0; i < columnNames.Count(); i++)
                {
                    var cell = row.CreateCell(i);
                    cell.SetCellValue(columnNames[i]);
                    cell.CellStyle = boldStyle;
                }

                // Write content
                for (int i = 0; i < dataTransfer.Count; i++)
                {
                    int numberElements = dataTransfer[i].Count();
                    row = excelSheet.CreateRow(i + 1);

                    for (int j = 0; j < numberElements; j++)
                    {
                        row.CreateCell(j).SetCellValue(dataTransfer[i][j]);
                    }
                }

                // Size columns
                excelSheet.TrackAllColumnsForAutoSizing();

                for (int i = 0; i < columnNames.Count(); i++)
                {
                    if (i == 1)
                    {
                        excelSheet.SetColumnWidth(i, 3800);
                    }
                    // Autosize needs to be after column has some data
                    excelSheet.AutoSizeColumn(i);
                }

                excelSheet.UntrackAllColumnsForAutoSizing();

                // Write to file
                workbook.Write(fs);

                TaskDialog.Show("Success", "Warnings report created in: " + excelPath);
            }
            return Result.Succeeded;
        }
    }
}