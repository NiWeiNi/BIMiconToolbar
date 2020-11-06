using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMiconToolbar.ExportSchedules
{
    [TransactionAttribute(TransactionMode.Manual)]
    class ExportSchedules : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Export destination folder
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string excelPath = desktopPath + @"\Schedule.xlsx";

            // Collect schedules
            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc)
                                                    .OfCategory(BuiltInCategory.OST_Schedules);

            List<ViewSchedule> schedules = viewsCollector.Cast<ViewSchedule>().Where(sh =>
                                   sh.Name != "<Revision Schedule>").ToList();

            // Extract data from schedule
            ViewSchedule sched = schedules[0] as ViewSchedule;
            TableData tD = sched.GetTableData();
            TableSectionData sectionData = tD.GetSectionData(SectionType.Body);
            int numbRows = sectionData.NumberOfRows;
            int numbCols = sectionData.NumberOfColumns;

            // Create excel file
            SXSSFWorkbook workbook = new SXSSFWorkbook();
            SXSSFSheet excelSheet = (SXSSFSheet)workbook.CreateSheet("Sheet1");
            excelSheet.SetRandomAccessWindowSize(100);

            //Create a header row
            IRow row = excelSheet.CreateRow(0);

            // Write to excel
            using (var fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write))
            {
                // Write content
                for (int i = 0; i < numbRows; i++)
                {
                    row = excelSheet.CreateRow(i + 1);

                    for (int j = 0; j < numbCols; j++)
                    {
                        string content = sched.GetCellText(SectionType.Body, i, j);
                        row.CreateCell(j).SetCellValue(content);
                    }
                }

                // Write to file
                workbook.Write(fs);

                TaskDialog.Show("Success", "Warnings report created in: " + excelPath);
            }

            return Result.Succeeded;
        }
    }
}
