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
            Autodesk.Revit.UI.RibbonPanel panelModel = application.CreateRibbonPanel(tabName, "Model");
            Autodesk.Revit.UI.RibbonPanel panelModelling = application.CreateRibbonPanel(tabName, "Modelling");
            Autodesk.Revit.UI.RibbonPanel panelProject = application.CreateRibbonPanel(tabName, "Project");
            Autodesk.Revit.UI.RibbonPanel panelSchedules = application.CreateRibbonPanel(tabName, "Schedules");
            Autodesk.Revit.UI.RibbonPanel panelSheets = application.CreateRibbonPanel(tabName, "Sheets");
            Autodesk.Revit.UI.RibbonPanel panelSupport = application.CreateRibbonPanel(tabName, "Support");

            // Retrieve assembly path
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // Help url for BIMicon items in the ribbon
            string urlHelp = @"https://www.bimicon.com/bimicon-plugin/";
            ContextualHelp contextHelpUrl = new ContextualHelp(ContextualHelpType.Url, urlHelp);

            /*---Ribbon Panel Library---*/
            #region Ribbon Panel Library
            // Remove backups
            #region Remove Backups tool
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
            pbRemoveBackups.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbRemoveBackups.SetContextualHelp(contextHelpUrl);
            #endregion
            // File Rename
            #region File Renames tool
            PushButtonData buttonFilesRename = new PushButtonData(
               "FilesRename",
               "Rename\nFiles",
               assemblyPath,
               "BIMiconToolbar.FilesRename.FilesRename"
            );

            PushButton pbFilesRename = panelLibrary.AddItem(buttonFilesRename) as PushButton;
            pbFilesRename.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesRename/Images/iconFilesRename.png"));
            pbFilesRename.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesRename/Images/iconFilesRenameSmall.png"));
            pbFilesRename.ToolTip = "Rename the files inside a folder.";
            pbFilesRename.LongDescription = "Rename all files of a certain type inside a folder.";
            pbFilesRename.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbFilesRename.SetContextualHelp(contextHelpUrl);
            #endregion
            // Upgrade Revit Files
            #region Upgrade Revit Files
            PushButtonData buttonFilesUpgrade = new PushButtonData(
               "FilesUpgrade",
               "Upgrade\nFiles",
               assemblyPath,
               "BIMiconToolbar.FilesUpgrade.FilesUpgrade"
            );

            PushButton pbFilesUpgrade = panelLibrary.AddItem(buttonFilesUpgrade) as PushButton;
            pbFilesUpgrade.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesUpgrade/Images/iconFilesUpgrade.png"));
            pbFilesUpgrade.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FilesUpgrade/Images/iconFilesUpgradeSmall.png"));
            pbFilesUpgrade.ToolTip = "Upgrade the files inside a folder.";
            pbFilesUpgrade.LongDescription = "Upgrade all Revit files inside a folder.";
            pbFilesUpgrade.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbFilesUpgrade.SetContextualHelp(contextHelpUrl);
            #endregion
            // Create Types Catalogue
            #region Generate Types Catalogue
            PushButtonData buttonTypesCatalogue = new PushButtonData(
               "TypesCatalogue",
               "Types\nCatalogue",
               assemblyPath,
               "BIMiconToolbar.TypesCatalogue.TypesCatalogue"
            );

            PushButton pbTypesCatalogue = panelLibrary.AddItem(buttonTypesCatalogue) as PushButton;
            pbTypesCatalogue.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/TypesCatalogue/Images/iconTypesCatalogue.png"));
            pbTypesCatalogue.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/TypesCatalogue/Images/iconTypesCatalogueSmall.png"));
            pbTypesCatalogue.ToolTip = "Generates a types catalago from the currently open family or selected family files.";
            pbTypesCatalogue.LongDescription = "Generates a types catalago from the currently open family or selected family files.";
            // Set the context help when F1 pressed
            pbTypesCatalogue.SetContextualHelp(contextHelpUrl);
            #endregion
            // Family Browser
            #region Family Browser
            //PushButtonData buttonFamilyBrowser = new PushButtonData(
            //   "FamilyBrowser",
            //   "Family\nBrowser",
            //   assemblyPath,
            //   "BIMiconToolbar.FamilyBrowser.FamilyBrowser"
            //);

            //PushButton pbFamilyBrowser = panelLibrary.AddItem(buttonFamilyBrowser) as PushButton;
            //pbFamilyBrowser.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FamilyBrowser/Images/iconFamilyBrowser.png"));
            //pbFamilyBrowser.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FamilyBrowser/Images/iconFamilyBrowserSmall.png"));
            //pbFamilyBrowser.ToolTip = "Open a dockable panel to search and load families.";
            //pbFamilyBrowser.LongDescription = "Open a dockable panel that allows searching and browsing families.";

            //// Set the context help when F1 pressed
            //pbFamilyBrowser.SetContextualHelp(contextHelpUrl);
            #endregion
            #endregion

            /*---Ribbon Panel Model---*/
            #region Ribbon Panel Model
            // Duplicate sheets
            PushButtonData buttonOpenLinksUnloaded = new PushButtonData(
               "Open with Links Unloaded",
               "Open with\nLinks Unloaded",
               assemblyPath,
               "BIMiconToolbar.OpenLinksUnloaded.OpenLinksUnloaded"
            );

            PushButton pbOpenLinksUnloaded = panelModel.AddItem(buttonOpenLinksUnloaded) as PushButton;
            pbOpenLinksUnloaded.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/OpenLinksUnloaded/Images/iconOpenLinksUnloaded.png"));
            pbOpenLinksUnloaded.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/OpenLinksUnloaded/Images/iconOpenLinksUnloadedSmall.png"));
            pbOpenLinksUnloaded.ToolTip = "Open Revit model with links unloaded.";
            pbOpenLinksUnloaded.LongDescription = "Open selected Revit model with the option to unload selected link types; Revit, IFC, CAD, and so on.";
            pbOpenLinksUnloaded.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbOpenLinksUnloaded.SetContextualHelp(contextHelpUrl);

            #endregion

            /*---Ribbon Panel Modelling---*/
            #region Ribbon Panel Modelling
            // Create floor finish according to room
            PushButtonData buttonFloorFinish = new PushButtonData(
               "Floor Finish",
               "Floor\nFinish",
               assemblyPath,
               "BIMiconToolbar.FloorFinish.FloorFinish"
            );

            PushButton pbFloorFinish = panelModelling.AddItem(buttonFloorFinish) as PushButton;
            pbFloorFinish.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FloorFinish/Images/iconFloorFinish.png"));
            pbFloorFinish.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/FloorFinish/Images/iconFloorFinishSmall.png"));
            pbFloorFinish.ToolTip = "Create floor finishes by selecting rooms.";
            pbFloorFinish.LongDescription = "Create floor finishes by selecting rooms and floor type.";
            // Set the context help when F1 pressed
            pbFloorFinish.SetContextualHelp(contextHelpUrl);
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

            // Set the context help when F1 pressed
            pbExportSchedules.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbWarningsReport.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbDuplicateSheets.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbViewOnSheet.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbNumberDoors.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbNumberWindows.SetContextualHelp(contextHelpUrl);

            // Create container for numbering tools
            SplitButtonData numberData = new SplitButtonData("numberGroupContainer", "Number Tools");
            SplitButton numberGroup = panelProject.AddItem(numberData) as SplitButton;

            // Number by Spline
            PushButtonData buttonNumberBySpline = new PushButtonData(
               "NumberBySpline",
               "Number\nby Spline",
               assemblyPath,
               "BIMiconToolbar.NumberBySpline.NumberBySpline"
            );

            PushButton pbNumberBySpline = numberGroup.AddPushButton(buttonNumberBySpline);
            pbNumberBySpline.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberBySpline/Images/iconNumberBySpline.png"));
            pbNumberBySpline.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberBySpline/Images/iconNumberBySpline.png"));
            pbNumberBySpline.ToolTip = "Number elements by intersecting with selected spline.";
            pbNumberBySpline.LongDescription = "Number elements of a selected category by interseting the bounding box with selected spline.";

            // Set the context help when F1 pressed
            pbNumberBySpline.SetContextualHelp(contextHelpUrl);

            // Number by Pick
            PushButtonData buttonNumberByPick = new PushButtonData(
            "NumberByPick",
            "Number\nby Pick",
            assemblyPath,
            "BIMiconToolbar.NumberByPick.NumberByPick"
            );

            PushButton pbNumberByPick = numberGroup.AddPushButton(buttonNumberByPick);
            pbNumberByPick.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberByPick/Images/iconNumberByPick.png"));
            pbNumberByPick.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberByPick/Images/iconNumberByPick.png"));
            pbNumberByPick.ToolTip = "Number picked elements in order of selection.";
            pbNumberByPick.LongDescription = "Number elements of a selected category by picking order.";

            // Set the context help when F1 pressed
            pbNumberByPick.SetContextualHelp(contextHelpUrl);


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

            // Set the context help when F1 pressed
            pbMatchGrids.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbMarkOrigin.SetContextualHelp(contextHelpUrl);

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

            // Set the context help when F1 pressed
            pbInteriorElevations.SetContextualHelp(contextHelpUrl);

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
            pbHelp.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbHelp.SetContextualHelp(contextHelpUrl);

            PushButton pbDocumentation = stackedSupport[1] as PushButton;
            pbDocumentation.ToolTip = "Documentation";
            pbDocumentation.LongDescription = "Check our online documentation";
            BitmapImage pbDocumentationImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Support/Docs/Images/iconDocSmall.png"));
            pbDocumentation.Image = pbDocumentationImage;
            pbDocumentation.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbDocumentation.SetContextualHelp(contextHelpUrl);

            PushButton pbVersion = stackedSupport[2] as PushButton;
            pbVersion.ToolTip = "Display current version";
            pbVersion.LongDescription = "Retrieves current version";
            BitmapImage pbVersionImageSmall = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Support/Version/Images/iconVersionSmall.png"));
            pbVersion.Image = pbVersionImageSmall;
            pbVersion.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbVersion.SetContextualHelp(contextHelpUrl);

            #endregion
        }
    }
}