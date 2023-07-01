using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers.Browser;
using NPOI.SS.UserModel;
using NPOI.XSSF.Streaming;
using System;
using System.Collections.Generic;
using System.IO;

namespace BIMicon.BIMiconToolbar.ExportSchedules
{
    [TransactionAttribute(TransactionMode.Manual)]
    class ExportSchedules : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            // Variables to store user input
            List<int> listIds;
            bool combineSchedules;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Variables to store log
            var schedSuccess = new List<string>();
            var schedFail = new List<string>();

            // Prompt window to collect user input
            using (BrowserCheckboxes customWindow = new BrowserCheckboxes(commandData))
            {
                customWindow.ShowDialog();
                listIds = customWindow.listIds;
                combineSchedules = customWindow.combineCheckbox.IsChecked.Value;
            }

            // Check there are schedules selected
            if (listIds != null)
            {
                BrowserWindow browserWindow = new BrowserWindow();
                browserWindow.ShowDialog();

                // Variables
                string fullPath = browserWindow.selectedPath;

                // Check that path is not empty and path is a folder
                if (fullPath == null || !Directory.Exists(fullPath))
                {
                    TaskDialog.Show("Warning", "No folder has been selected");
                    return Result.Cancelled;
                }
                else
                {
                    // Create excel file
                    SXSSFWorkbook workbook = new SXSSFWorkbook();

                    // Loop through each selected schedule
                    foreach (int id in listIds)
                    {
                        // Extract data from schedule
                        ViewSchedule sched = doc.GetElement(new ElementId(id)) as ViewSchedule;
                        TableData tD = sched.GetTableData();
                        TableSectionData sectionData = tD.GetSectionData(SectionType.Body);
                        int numbRows = sectionData.NumberOfRows;
                        int numbCols = sectionData.NumberOfColumns;

                        string excelPath;
                        SXSSFSheet excelSheet = (SXSSFSheet)workbook.CreateSheet(sched.Name);
                        excelSheet.SetRandomAccessWindowSize(100);

                        //Create a header row
                        IRow row = excelSheet.CreateRow(0);

                        // Define format for cells
                        var fontStyle = workbook.CreateFont();
                        fontStyle.IsBold = true;
                        fontStyle.FontHeightInPoints = 12;
                        var titleStyle = workbook.CreateCellStyle();
                        titleStyle.SetFont(fontStyle);

                        FileStream fs;

                        // Combine schedules into a single file or separate them
                        if (combineSchedules)
                        {
                            // Name of the file
                            excelPath = fullPath + @"\" + doc.Title + ".xlsx";
                            fs = new FileStream(excelPath, FileMode.OpenOrCreate, FileAccess.Write);
                        }
                        else
                        {
                            // Name of the file
                            excelPath = fullPath + @"\" + sched.Name + ".xlsx";
                            fs = new FileStream(excelPath, FileMode.Create, FileAccess.Write);
                        }

                        // Write to excel
                        using (fs)
                        {
                            // Write content
                            for (int i = 0; i < numbRows; i++)
                            {
                                row = excelSheet.CreateRow(i);

                                for (int j = 0; j < numbCols; j++)
                                {
                                    string content = sched.GetCellText(SectionType.Body, i, j);
                                    var cell = row.CreateCell(j);
                                    cell.SetCellValue(content);

                                    if (i == 0)
                                    {
                                        cell.CellStyle = titleStyle;
                                    }
                                }
                            }

                            // Size columns
                            excelSheet.TrackAllColumnsForAutoSizing();
                            for (int i = 0; i < numbCols; i++)
                            {
                                excelSheet.AutoSizeColumn(i);
                            }
                            excelSheet.UntrackAllColumnsForAutoSizing();

                            // Write to file
                            try
                            {
                                workbook.Write(fs);
                                // Log success export schedule name
                                schedSuccess.Add(sched.Name);
                            }
                            catch
                            {
                                schedFail.Add(sched.Name);
                            }
                        }
                    }

                    TaskDialog.Show("Success", "The following schedules have been exported: " +
                                    string.Join("\n", schedSuccess.ToArray()));

                    return Result.Succeeded;
                }
            }

            return Result.Cancelled;
        }
    }
}
