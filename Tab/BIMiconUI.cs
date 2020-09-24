using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.IO;
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
            Autodesk.Revit.UI.RibbonPanel panelSheets = application.CreateRibbonPanel(tabName, "Sheets");

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
            
            #endregion

            /*---Ribbon Panel Schedules---*/

            #region Ribbon Panel Schedules

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

            // Warnings Review
            PushButtonData buttonWarningsReport = new PushButtonData(
               "WarningsReport",
               "Warnings\nReport",
               assemblyPath,
               "BIMiconToolbar.WarningsReport.WarningsReport"
            );

            PushButton pbWarningsReport = panelProject.AddItem(buttonWarningsReport) as PushButton;
            pbWarningsReport.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/WarningsReport/Images/iconWarningsReview.png"));
            pbWarningsReport.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/WarningsReport/Images/iconWarningsReviewSmall.png"));
            pbWarningsReport.ToolTip = "Generate Warnings report.";
            pbWarningsReport.LongDescription = "Exports a Warnings report classified by priority.";

            #endregion
        }
    }
}