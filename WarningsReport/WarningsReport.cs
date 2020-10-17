using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Newtonsoft.Json.Linq;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace BIMiconToolbar.WarningsReport
{
    [TransactionAttribute(TransactionMode.Manual)]
    class WarningsReport : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Retrieve current date
            string currentDate = DateTime.Today.ToString("dd/MM/yyyy");

            string[] columnNames = { "Priority", "Warning", "Element Ids", "Date Detected", "Date Solved", "Fix by" };

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

            // Size columns
            for (int i = 0; i < columnNames.Count(); i++)
            {
                if (i == 1)
                {
                    excelSheet.SetColumnWidth(i, 5000);
                }
                excelSheet.AutoSizeColumn(i);
            }

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
                workbook.Write(fs);
            }
            return Result.Succeeded;
        }
    }
}