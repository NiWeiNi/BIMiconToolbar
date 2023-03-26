using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Tab.Models;
using BIMicon.BIMiconToolbar.Tab;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMiconToolbar.Tab
{
    internal class BIMiconUI
    {
        public static void Toolbar(UIControlledApplication application)
        {
            // Create tab 
            string tabName = "BIMicon";
            application.CreateRibbonTab(tabName);

            // Create ribbon
            RibbonPanel panelLibrary = application.CreateRibbonPanel(tabName, "Library");
            RibbonPanel panelModel = application.CreateRibbonPanel(tabName, "Model");
            RibbonPanel panelModelling = application.CreateRibbonPanel(tabName, "Modelling");
            RibbonPanel panelProject = application.CreateRibbonPanel(tabName, "Project");
            RibbonPanel panelSchedules = application.CreateRibbonPanel(tabName, "Schedules");
            RibbonPanel panelSheets = application.CreateRibbonPanel(tabName, "Sheets");
            RibbonPanel panelSupport = application.CreateRibbonPanel(tabName, "Support");

            // Retrieve assembly path
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            // Help url for BIMicon items in the ribbon
            string urlHelp = @"https://www.bimicon.com/bimicon-plugin/";
            ContextualHelp contextHelpUrl = new ContextualHelp(ContextualHelpType.Url, urlHelp);

            /*---Ribbon Panel Library---*/
            #region Ribbon Panel Library
            // Remove Backups
            #region Remove Backups tool
            PushButtonIdentityData removeBackupIdData = new PushButtonIdentityData
            {
                ButtonName = "RemoveBackups",
                ButtonDisplayName = "Remove\nBackups",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.RemoveBackups.RemoveBackups",
                ToolTip = "Remove Revit backup files.",
                LongDescription = "Remove Revit backup files from selected folder including subfolders.",
                AvailabilityClass = "BIMiconToolbar.Tab.CommandAvailability",
                RibbonPanelContainer = panelLibrary,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/RemoveBackups/Images/iconRemoveBackup.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/RemoveBackups/Images/iconRemoveBackupSmall.png"
            };
            _ = new PushButtonCreation(removeBackupIdData);
            #endregion
            // File Rename
            #region File Renames tool
            PushButtonIdentityData filesRenameIdData = new PushButtonIdentityData
            {
                ButtonName = "FilesRename",
                ButtonDisplayName = "Rename\nFiles",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.FilesRename.FilesRename",
                ToolTip = "Rename the files inside a folder.",
                LongDescription = "Rename all files of a certain type inside a folder.",
                AvailabilityClass = "BIMiconToolbar.Tab.CommandAvailability",
                RibbonPanelContainer = panelLibrary,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FilesRename/Images/iconFilesRename.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FilesRename/Images/iconFilesRenameSmall.png"
            };
            _ = new PushButtonCreation(filesRenameIdData);
            #endregion
            // Upgrade Revit Files
            #region Upgrade Revit Files
            PushButtonIdentityData filesUpgradeIdData = new PushButtonIdentityData
            {
                ButtonName = "FilesUpgrade",
                ButtonDisplayName = "Upgrade\nFiles",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.FilesUpgrade.FilesUpgrade",
                ToolTip = "Upgrade the files inside a folder.",
                LongDescription = "Upgrade all Revit files inside a folder.",
                AvailabilityClass = "BIMiconToolbar.Tab.CommandAvailability",
                RibbonPanelContainer = panelLibrary,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FilesUpgrade/Images/iconFilesUpgrade.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FilesUpgrade/Images/iconFilesUpgradeSmall.png"
            };
            _ = new PushButtonCreation(filesUpgradeIdData);
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
            //pbFamilyBrowser.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/FamilyBrowser/Images/iconFamilyBrowser.png"));
            //pbFamilyBrowser.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/FamilyBrowser/Images/iconFamilyBrowserSmall.png"));
            //pbFamilyBrowser.ToolTip = "Open a dockable panel to search and load families.";
            //pbFamilyBrowser.LongDescription = "Open a dockable panel that allows searching and browsing families.";

            //// Set the context help when F1 pressed
            //pbFamilyBrowser.SetContextualHelp(contextHelpUrl);
            #endregion
            #endregion

            /*---Ribbon Panel Model---*/
            #region Ribbon Panel Model
            // Open files with links unloaded
            PushButtonIdentityData openLinksUnloadedIdData = new PushButtonIdentityData
            {
                ButtonName = "Open with Links Unloaded",
                ButtonDisplayName = "Open with\nLinks Unloaded",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.OpenLinksUnloaded.OpenLinksUnloaded",
                ToolTip = "Open Revit model with links unloaded.",
                LongDescription = "Open selected Revit model with the option to unload selected link types; Revit, IFC, CAD, and so on.",
                AvailabilityClass = "BIMiconToolbar.Tab.CommandAvailability",
                RibbonPanelContainer = panelModel,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/OpenLinksUnloaded/Images/iconOpenLinksUnloaded.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/OpenLinksUnloaded/Images/iconOpenLinksUnloadedSmall.png"
            };
            _ = new PushButtonCreation(openLinksUnloadedIdData);
            #endregion

            /*---Ribbon Panel Modelling---*/
            #region Ribbon Panel Modelling
            // Create floor finish according to room
            PushButtonIdentityData floorFinishIdData = new PushButtonIdentityData
            {
                ButtonName = "Floor Finish",
                ButtonDisplayName = "Floor\nFinish",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.FloorFinish.FloorFinish",
                ToolTip = "Create floor finishes by selecting rooms.",
                LongDescription = "Create floor finishes by selecting rooms and floor type.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelModelling,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FloorFinish/Images/iconFloorFinish.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/FloorFinish/Images/iconFloorFinishSmall.png"
            };
            _ = new PushButtonCreation(floorFinishIdData);
            #endregion

            /*---Ribbon Panel Schedules---*/
            #region Ribbon Panel Schedules

            // Export Schedules
            PushButtonIdentityData exportSchedulesIdData = new PushButtonIdentityData
            {
                ButtonName = "ExportSchedules",
                ButtonDisplayName = "Export\nSchedules",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.ExportSchedules.ExportSchedules",
                ToolTip = "Export selected schedules.",
                LongDescription = "Export selected schedules to the selected destination.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelSchedules,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/ExportSchedules/Images/iconSchedulesExcel.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/ExportSchedules/Images/iconSchedulesExcelSmall.png"
            };
            _ = new PushButtonCreation(exportSchedulesIdData);

            // Warnings Review
            PushButtonIdentityData warningsReviewIdData = new PushButtonIdentityData
            {
                ButtonName = "WarningsReport",
                ButtonDisplayName = "Warnings\nReport",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.WarningsReport.WarningsReport",
                ToolTip = "Generate Warnings report.",
                LongDescription = "Exports a Warnings report classified by priority.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelSchedules,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/WarningsReport/Images/iconWarningsReview.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/WarningsReport/Images/iconWarningsReviewSmall.png"
            };
            _ = new PushButtonCreation(warningsReviewIdData);
            #endregion

            /*---Ribbon Panel Sheets---*/
            #region Ribbon Panel Sheets

            // Duplicate sheets
            PushButtonIdentityData duplicateSheetsIdData = new PushButtonIdentityData
            {
                ButtonName = "DuplicateSheets",
                ButtonDisplayName = "Duplicate\nSheets",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.DuplicateSheets.DuplicateSheets",
                ToolTip = "Duplicate active sheet.",
                LongDescription = "Duplicate current active sheet with detailing and annotation elements.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelSheets,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/DuplicateSheets/Images/iconDupSheets.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/DuplicateSheets/Images/iconDupSheetsSmall.png"
            };
            _ = new PushButtonCreation(duplicateSheetsIdData);

            // View on Sheet
            PushButtonIdentityData viewOnSheetsIdData = new PushButtonIdentityData
            {
                ButtonName = "ViewOnSheet",
                ButtonDisplayName = "View on\nSheet",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.ViewOnSheet.ViewOnSheet",
                ToolTip = "Place active view on selected sheet.",
                LongDescription = "Place active view on selected sheet.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelSheets,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/ViewOnSheet/Images/iconViewOnSheet.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/ViewOnSheet/Images/iconViewOnSheetSmall.png"
            };
            _ = new PushButtonCreation(viewOnSheetsIdData);
            #endregion

            /*---Ribbon Panel Project---*/
            #region Ribbon Panel Project
            // Create push buttons for panelProject
            // Number Doors
            PushButtonIdentityData numberDoorsIdData = new PushButtonIdentityData
            {
                ButtonName = "NumberDoors",
                ButtonDisplayName = "Number\nDoors",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.NumberDoors.NumberDoors2020",
                ToolTip = "Number doors according to room number.",
                LongDescription = "Assigns a door a number according to room. The primary parameter to use for door number" +
                                "is picked from the ToRoom paramter from the door. If there is no room on either side" +
                                "of the door, no number will be assigned.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelProject,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberDoors/Images/iconNumberDoors.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/ViewOnSheet/Images/iconViewOnSheetSmall.png"
            };
            _ = new PushButtonCreation(numberDoorsIdData);

            // Number Windows
            PushButtonIdentityData numberWindowsIdData = new PushButtonIdentityData
            {
                ButtonName = "NumberWindows",
                ButtonDisplayName = "Number\nWindows",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.NumberWindows.NumberWindows",
                ToolTip = "Number windows according to room number.",
                LongDescription = "Assigns a window number according to room. The primary parameter to use for window number" +
                    "is picked from the ToRoom paramter from the window. If there is no room on either side" +
                    "of the window, no number will be assigned.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelProject,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberWindows/Images/iconNumberWindows.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberWindows/Images/iconNumberWindowsSmall.png"
            };
            _ = new PushButtonCreation(numberWindowsIdData);

            // Create container for numbering tools
            SplitButtonData numberData = new SplitButtonData("numberGroupContainer", "Number Tools");
            SplitButton numberGroup = panelProject.AddItem(numberData) as SplitButton;

            // Number by Spline
            PushButtonIdentityData numberBySplineIdData = new PushButtonIdentityData
            {
                ButtonName = "NumberBySpline",
                ButtonDisplayName = "Number\nby Spline",
                AssemblyName = assemblyPath,
                ClassName = "BIMiconToolbar.NumberBySpline.NumberBySpline",
                ToolTip = "Number elements by intersecting with selected spline.",
                LongDescription = "Number elements of a selected category by interseting the bounding box with selected spline.",
                AvailabilityClass = null,
                SplitButtonContainer = numberGroup,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberBySpline/Images/iconNumberBySpline.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberBySpline/Images/iconNumberBySpline.png"
            };
            _ = new PushButtonCreation(numberBySplineIdData);


            // Number by Pick
            PushButtonData buttonNumberByPick = new PushButtonData(
            "NumberByPick",
            "Number\nby Pick",
            assemblyPath,
            "BIMiconToolbar.NumberByPick.NumberByPick"
            );

            PushButton pbNumberByPick = numberGroup.AddPushButton(buttonNumberByPick);
            pbNumberByPick.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/NumberByPick/Images/iconNumberByPick.png"));
            pbNumberByPick.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/NumberByPick/Images/iconNumberByPick.png"));
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
            pbMatchGrids.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/MatchGrids/Images/iconMatchGrids.png"));
            pbMatchGrids.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/MatchGrids/Images/iconMatchGridsSmall.png"));
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
            pbMarkOrigin.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/MarkOrigin/Images/iconMarkerOrigin.png"));
            pbMarkOrigin.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/MarkOrigin/Images/iconMarkerOrigin.png"));
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
            pbInteriorElevations.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/InteriorElevations/Images/iconInteriorElev.png"));
            pbInteriorElevations.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/InteriorElevations/Images/iconInteriorElevSmall.png"));
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
            BitmapImage pbHelpImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Help/Images/iconHelpSmall.png"));
            pbHelp.Image = pbHelpImage;
            pbHelp.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbHelp.SetContextualHelp(contextHelpUrl);

            PushButton pbDocumentation = stackedSupport[1] as PushButton;
            pbDocumentation.ToolTip = "Documentation";
            pbDocumentation.LongDescription = "Check our online documentation";
            BitmapImage pbDocumentationImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Docs/Images/iconDocSmall.png"));
            pbDocumentation.Image = pbDocumentationImage;
            pbDocumentation.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbDocumentation.SetContextualHelp(contextHelpUrl);

            PushButton pbVersion = stackedSupport[2] as PushButton;
            pbVersion.ToolTip = "Display current version";
            pbVersion.LongDescription = "Retrieves current version";
            BitmapImage pbVersionImageSmall = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Version/Images/iconVersionSmall.png"));
            pbVersion.Image = pbVersionImageSmall;
            pbVersion.AvailabilityClassName = "BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbVersion.SetContextualHelp(contextHelpUrl);

            #endregion
        }
    }
}