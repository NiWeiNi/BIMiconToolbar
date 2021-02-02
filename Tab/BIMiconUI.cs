using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMiconToolbar.Tab
{
    class BIMiconUI
    {
        public static void Toolbar(UIControlledApplication application)
        {
            // Create tab 
            string tabName = "BIMicon";
            application.CreateRibbonTab(tabName);

            // Create ribbon
            Autodesk.Revit.UI.RibbonPanel panelLibrary = application.CreateRibbonPanel(tabName, "Library");
            Autodesk.Revit.UI.RibbonPanel panelProject = application.CreateRibbonPanel(tabName, "Project");
            Autodesk.Revit.UI.RibbonPanel panelSchedules = application.CreateRibbonPanel(tabName, "Schedules");
            Autodesk.Revit.UI.RibbonPanel panelSheets = application.CreateRibbonPanel(tabName, "Sheets");
            Autodesk.Revit.UI.RibbonPanel panelSupport = application.CreateRibbonPanel(tabName, "Support");

            // Retrieve assembly path
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            /*---Ribbon Panel Library---*/
            #region Ribbon Panel Library
            // Duplicate sheets
            PushButtonData buttonRemoveBackups = new PushButtonData(
               "RemoveBackups",
               "Remove\nBackups",
               assemblyPath,
               "BIMiconToolbar.RemoveBackups.RemoveBackups"
            );

            PushButton pbRemoveBackups = panelLibrary.AddItem(buttonRemoveBackups) as PushButton;
            pbRemoveBackups.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/RemoveBackups/Images/iconRemoveBackup.png"));
            pbRemoveBackups.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/RemoveBackups/Images/iconRemoveBackupSmall.png"));
            pbRemoveBackups.ToolTip = "Remove Revit backup files.";
            pbRemoveBackups.LongDescription = "Remove Revit backup files from selected folder including subfolders.";

            // File Rename
            PushButtonData buttonFilesRename = new PushButtonData(
               "FilesRename",
               "Rename\nFiles",
               assemblyPath,
               "BIMiconToolbar.FilesRename.FilesRename"
            );

            PushButton pbFilesRename = panelLibrary.AddItem(buttonFilesRename) as PushButton;
            pbFilesRename.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesRename/Images/iconFilesRename.png"));
            pbFilesRename.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesRename/Images/iconFilesRenameSmall.png"));
            pbFilesRename.ToolTip = "Rename file inside a folder.";
            pbFilesRename.LongDescription = "Rename all all files of certain type inside a folder.";

            #endregion

            /*---Ribbon Panel Schedules---*/
            #region Ribbon Panel Schedules

            // Export Schedules
            PushButtonData buttonExportSchedules = new PushButtonData(
               "ExportSchedules",
               "Export\nSchedules",
               assemblyPath,
               "BIMiconToolbar.ExportSchedules.ExportSchedules"
            );

            PushButton pbExportSchedules = panelSchedules.AddItem(buttonExportSchedules) as PushButton;
            pbExportSchedules.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/ExportSchedules/Images/iconSchedulesExcel.png"));
            pbExportSchedules.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/ExportSchedules/Images/iconSchedulesExcelSmall.png"));
            pbExportSchedules.ToolTip = "Export selected schedules.";
            pbExportSchedules.LongDescription = "Export selected schedules to the selected destination.";

            // Warnings Review
            PushButtonData buttonWarningsReport = new PushButtonData(
               "WarningsReport",
               "Warnings\nReport",
               assemblyPath,
               "BIMiconToolbar.WarningsReport.WarningsReport"
            );

            PushButton pbWarningsReport = panelSchedules.AddItem(buttonWarningsReport) as PushButton;
            pbWarningsReport.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/WarningsReport/Images/iconWarningsReview.png"));
            pbWarningsReport.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/WarningsReport/Images/iconWarningsReviewSmall.png"));
            pbWarningsReport.ToolTip = "Generate Warnings report.";
            pbWarningsReport.LongDescription = "Exports a Warnings report classified by priority.";

            #endregion

            /*---Ribbon Panel Sheets---*/
            #region Ribbon Panel Sheets

            // Duplicate sheets
            PushButtonData buttonDuplicateSheets = new PushButtonData(
               "DuplicateSheets",
               "Duplicate\nSheets",
               assemblyPath,
               "BIMiconToolbar.DuplicateSheets.DuplicateSheets"
            );

            PushButton pbDuplicateSheets = panelSheets.AddItem(buttonDuplicateSheets) as PushButton;
            pbDuplicateSheets.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/DuplicateSheets/Images/iconDupSheets.png"));
            pbDuplicateSheets.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/DuplicateSheets/Images/iconDupSheetsSmall.png"));
            pbDuplicateSheets.ToolTip = "Duplicate active sheet.";
            pbDuplicateSheets.LongDescription = "Duplicate current active sheet with detailing and annotation elements.";

            // View on Sheet
            PushButtonData buttonViewOnSheet = new PushButtonData(
               "ViewOnSheet",
               "View on\nSheet",
               assemblyPath,
               "BIMiconToolbar.ViewOnSheet.ViewOnSheet"
            );

            PushButton pbViewOnSheet = panelSheets.AddItem(buttonViewOnSheet) as PushButton;
            pbViewOnSheet.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/ViewOnSheet/Images/iconViewOnSheet.png"));
            pbViewOnSheet.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/ViewOnSheet/Images/iconViewOnSheetSmall.png"));
            pbViewOnSheet.ToolTip = "Place active view on selected sheet.";
            pbViewOnSheet.LongDescription = "Place active view on selected sheet.";

            #endregion

            /*---Ribbon Panel Project---*/
            #region Ribbon Panel Project
            //Create push buttons for panelProject

            PushButtonData buttonNumberDoors = new PushButtonData(
                "NumberDoors",
                "Number\nDoors",
                assemblyPath,
                "BIMiconToolbar.NumberDoors.NumberDoors2020"
            );

            PushButton pbNumberDoors = panelProject.AddItem(buttonNumberDoors) as PushButton;
            pbNumberDoors.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberDoors/Images/iconNumberDoors.png"));
            pbNumberDoors.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberDoors/Images/iconNumberDoorsSmall.png"));

            RibbonToolTip numberDoorsToolTip = Auxiliar.ButtonToolTip("NumberDoorsHelp.mp4",
                                                "BIMiconToolbar.NumberDoors.Images.NumberDoorsHelp.mp4",
                                                "Number doors according to room number.",
                                                "Assigns a door number according to room. The primary parameter to use for door number" +
                                                "is picked from the ToRoom paramter from the door. If there is no room on either side" +
                                                "of the door, no number will be assigned.");

            Auxiliar.SetRibbonItemToolTip(pbNumberDoors, numberDoorsToolTip);

            // Number Windows
            PushButtonData buttonNumberWindows= new PushButtonData(
               "NumberWindows",
               "Number\nWindows",
               assemblyPath,
               "BIMiconToolbar.NumberWindows.NumberWindows"
            );

            PushButton pbNumberWindows = panelProject.AddItem(buttonNumberWindows) as PushButton;
            pbNumberWindows.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberWindows/Images/iconNumberWindows.png"));
            pbNumberWindows.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberWindows/Images/iconNumberWindowsSmall.png"));
            pbNumberWindows.ToolTip = "Number windows according to room number.";
            pbNumberWindows.LongDescription = "Assigns a window number according to room. The primary parameter to use for window number" +
                                                "is picked from the ToRoom paramter from the window. If there is no room on either side" +
                                                "of the window, no number will be assigned.";

            // Number by Spline
            PushButtonData buttonNumberBySpline = new PushButtonData(
               "NumberBySpline",
               "Number\nby Spline",
               assemblyPath,
               "BIMiconToolbar.NumberBySpline.NumberBySpline"
            );

            PushButton pbNumberBySpline = panelProject.AddItem(buttonNumberBySpline) as PushButton;
            pbNumberBySpline.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberBySpline/Images/iconNumberBySpline.png"));
            pbNumberBySpline.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberBySpline/Images/iconNumberBySpline.png"));
            pbNumberBySpline.ToolTip = "Number elements by intersecting with selected spline.";
            pbNumberBySpline.LongDescription = "Number elements of a selected category by interseting the bounding box with selected spline.";

            // Match grids
            PushButtonData buttonMatchGrids = new PushButtonData(
               "MatchGrids",
               "Match\nGrids",
               assemblyPath,
               "BIMiconToolbar.MatchGrids.MatchGrids"
            );

            PushButton pbMatchGrids = panelProject.AddItem(buttonMatchGrids) as PushButton;
            pbMatchGrids.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/MatchGrids/Images/iconMatchGrids.png"));
            pbMatchGrids.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/MatchGrids/Images/iconMatchGridsSmall.png"));
            pbMatchGrids.ToolTip = "Match grids display from one view to all selected views";
            pbMatchGrids.LongDescription = "Match grids display from one view to all other selected views.";

            // Mark Origin
            PushButtonData buttonMarkOrigin = new PushButtonData(
               "MarkerOrigin",
               "Mark\nOrigin",
               assemblyPath,
               "BIMiconToolbar.MarkOrigin.MarkOrigin"
            );

            PushButton pbMarkOrigin = panelProject.AddItem(buttonMarkOrigin) as PushButton;
            pbMarkOrigin.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/MarkOrigin/Images/iconMarkerOrigin.png"));
            pbMarkOrigin.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/MarkOrigin/Images/iconMarkerOrigin.png"));
            pbMarkOrigin.ToolTip = "Marks in the current view Revit's internal origin";
            pbMarkOrigin.LongDescription = "Marks in the current view Revit's internal origin";

            // Interior Elevations
            PushButtonData buttonInteriorElevations = new PushButtonData(
               "InteriorElevations",
               "Interior\nElevations",
               assemblyPath,
               "BIMiconToolbar.InteriorElevations.InteriorElevations"
            );

            PushButton pbInteriorElevations = panelProject.AddItem(buttonInteriorElevations) as PushButton;
            pbInteriorElevations.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/InteriorElevations/Images/iconInteriorElev.png"));
            pbInteriorElevations.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/InteriorElevations/Images/iconInteriorElevSmall.png"));
            pbInteriorElevations.ToolTip = "Creates interior elevations from selected rooms";
            pbInteriorElevations.LongDescription = "Creates interior elevations from selected rooms and place them onto sheets";

            #endregion

            /*--- Ribbon Panel Support ---*/
            #region Panel Support

            //Create buttons for panelSupport
            PushButtonData buttonVersion = new PushButtonData(
                "Version",
                "Version",
                assemblyPath,
                "BIMiconToolbar.Support.Version.Version");

            PushButtonData buttonDocumentation = new PushButtonData(
                "Documentation",
                "Docs",
                assemblyPath,
                "BIMiconToolbar.Support.Docs.Docs");

            PushButtonData buttonHelp = new PushButtonData(
                "Help",
                "Help",
                assemblyPath,
                "BIMiconToolbar.Support.Help.Help");

            // Stacked items for stacked buttons
            IList<Autodesk.Revit.UI.RibbonItem> stackedSupport = panelSupport.AddStackedItems(buttonHelp, buttonDocumentation, buttonVersion);

            // Defining buttons
            PushButton pbHelp = stackedSupport[0] as PushButton;
            pbHelp.ToolTip = "Get Help";
            pbHelp.LongDescription = "Contact us for any query or help";
            BitmapImage pbHelpImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Support/Help/Images/iconHelpSmall.png"));
            pbHelp.Image = pbHelpImage;

            PushButton pbDocumentation = stackedSupport[1] as PushButton;
            pbDocumentation.ToolTip = "Documentation";
            pbDocumentation.LongDescription = "Check our online documentation";
            BitmapImage pbDocumentationImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Support/Docs/Images/iconDocSmall.png"));
            pbDocumentation.Image = pbDocumentationImage;

            PushButton pbVersion = stackedSupport[2] as PushButton;
            pbVersion.ToolTip = "Display current version";
            pbVersion.LongDescription = "Retrieves current version";
            BitmapImage pbVersionImageSmall = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Support/Version/Images/iconVersionSmall.png"));
            pbVersion.Image = pbVersionImageSmall;

            #endregion
        }
    }
}