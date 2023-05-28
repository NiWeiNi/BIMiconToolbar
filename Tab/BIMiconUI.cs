using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Tab.Models;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMicon.BIMiconToolbar.Tab
{
    internal class BIMiconUI
    {
        public BIMiconUI(UIControlledApplication application)
        {
            string tabName = "BIMicon";
            // Handle tab creation
            RevitRibbon.CheckRibbonTabExist(tabName, application);

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
                ClassName = "BIMicon.BIMiconToolbar.RemoveBackups.RemoveBackups",
                ToolTip = "Remove Revit backup files.",
                LongDescription = "Remove Revit backup files from selected folder including subfolders.",
                AvailabilityClass = "BIMicon.BIMiconToolbar.Tab.CommandAvailability",
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
                ClassName = "BIMicon.BIMiconToolbar.FilesRename.FilesRename",
                ToolTip = "Rename the files inside a folder.",
                LongDescription = "Rename all files of a certain type inside a folder.",
                AvailabilityClass = "BIMicon.BIMiconToolbar.Tab.CommandAvailability",
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
                ClassName = "BIMicon.BIMiconToolbar.FilesUpgrade.FilesUpgrade",
                ToolTip = "Upgrade the files inside a folder.",
                LongDescription = "Upgrade all Revit files inside a folder.",
                AvailabilityClass = "BIMicon.BIMiconToolbar.Tab.CommandAvailability",
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
                ClassName = "BIMicon.BIMiconToolbar.OpenLinksUnloaded.OpenLinksUnloaded",
                ToolTip = "Open Revit model with links unloaded.",
                LongDescription = "Open selected Revit model with the option to unload selected link types; Revit, IFC, CAD, and so on.",
                AvailabilityClass = "BIMicon.BIMiconToolbar.Tab.CommandAvailability",
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
                ClassName = "BIMicon.BIMiconToolbar.FloorFinish.FloorFinish",
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
                ClassName = "BIMicon.BIMiconToolbar.ExportSchedules.ExportSchedules",
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
                ClassName = "BIMicon.BIMiconToolbar.WarningsReport.WarningsReport",
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
                ClassName = "BIMicon.BIMiconToolbar.DuplicateSheets.DuplicateSheets",
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
                ClassName = "BIMicon.BIMiconToolbar.ViewOnSheet.ViewOnSheet",
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
                ClassName = "BIMicon.BIMiconToolbar.NumberDoors.NumberDoors2020",
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
                ClassName = "BIMicon.BIMiconToolbar.NumberWindows.NumberWindows",
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
                ClassName = "BIMicon.BIMiconToolbar.NumberBySpline.NumberBySpline",
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
            PushButtonIdentityData numberByPickIdData = new PushButtonIdentityData
            {
                ButtonName = "NumberByPick",
                ButtonDisplayName = "Number\nby Pick",
                AssemblyName = assemblyPath,
                ClassName = "BIMicon.BIMiconToolbar.NumberByPick.NumberByPick",
                ToolTip = "Number picked elements in order of selection.",
                LongDescription = "Number elements of a selected category by picking order.",
                AvailabilityClass = null,
                SplitButtonContainer = numberGroup,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberByPick/Images/iconNumberByPick.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/NumberByPick/Images/iconNumberByPick.png"
            };
            _ = new PushButtonCreation(numberByPickIdData);

            // Match grids
            PushButtonIdentityData matchGridsIdData = new PushButtonIdentityData
            {
                ButtonName = "MatchGrids",
                ButtonDisplayName = "Match\nGrids",
                AssemblyName = assemblyPath,
                ClassName = "BIMicon.BIMiconToolbar.MatchGrids.MatchGrids",
                ToolTip = "Match grids display from one view to all selected views",
                LongDescription = "Match grids display from one view to all other selected views.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelProject,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/MatchGrids/Images/iconMatchGrids.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/MatchGrids/Images/iconMatchGridsSmall.png"
            };
            _ = new PushButtonCreation(matchGridsIdData);

            // Mark Origin
            PushButtonIdentityData markOriginIdData = new PushButtonIdentityData
            {
                ButtonName = "MarkerOrigin",
                ButtonDisplayName = "Mark\nOrigin",
                AssemblyName = assemblyPath,
                ClassName = "BIMicon.BIMiconToolbar.MarkOrigin.MarkOrigin",
                ToolTip = "Marks in the current view Revit's internal origin",
                LongDescription = "Marks in the current view Revit's internal origin.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelProject,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/MarkOrigin/Images/iconMarkerOrigin.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/MarkOrigin/Images/iconMarkerOriginSmall.png"
            };
            _ = new PushButtonCreation(markOriginIdData);

            // Interior Elevations
            PushButtonIdentityData interiorElevationsIdData = new PushButtonIdentityData
            {
                ButtonName = "InteriorElevations",
                ButtonDisplayName = "Interior\nElevations",
                AssemblyName = assemblyPath,
                ClassName = "BIMicon.BIMiconToolbar.InteriorElevations.InteriorElevations",
                ToolTip = "Creates interior elevations from selected rooms",
                LongDescription = "Creates interior elevations from selected rooms and place them onto sheets.",
                AvailabilityClass = null,
                RibbonPanelContainer = panelProject,
                ContextualHelpButton = contextHelpUrl,
                LargeImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/InteriorElevations/Images/iconInteriorElev.png",
                SmallImagePath = "pack://application:,,,/BIMiconToolbar;component/Buttons/InteriorElevations/Images/iconInteriorElevSmall.png"
            };
            _ = new PushButtonCreation(interiorElevationsIdData);
            #endregion

            /*--- Ribbon Panel Support ---*/
            #region Panel Support

            // Create buttons for panelSupport
            PushButtonData buttonVersion = new PushButtonData(
                "Version",
                "Version",
                assemblyPath,
                "BIMicon.BIMiconToolbar.Support.Version.Version");

            PushButtonData buttonDocumentation = new PushButtonData(
                "Documentation",
                "Docs",
                assemblyPath,
                "BIMicon.BIMiconToolbar.Support.Docs.Docs");

            PushButtonData buttonHelp = new PushButtonData(
                "Help",
                "Help",
                assemblyPath,
                "BIMicon.BIMiconToolbar.Support.Help.Help");

            // Stacked items for stacked buttons
            IList<RibbonItem> stackedSupport = panelSupport.AddStackedItems(buttonHelp, buttonDocumentation, buttonVersion);

            // Defining buttons
            PushButton pbHelp = stackedSupport[0] as PushButton;
            pbHelp.ToolTip = "Get Help";
            pbHelp.LongDescription = "Contact us for any query or help";
            BitmapImage pbHelpImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Help/Images/iconHelpSmall.png"));
            pbHelp.Image = pbHelpImage;
            pbHelp.AvailabilityClassName = "BIMicon.BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbHelp.SetContextualHelp(contextHelpUrl);

            PushButton pbDocumentation = stackedSupport[1] as PushButton;
            pbDocumentation.ToolTip = "Documentation";
            pbDocumentation.LongDescription = "Check our online documentation";
            BitmapImage pbDocumentationImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Docs/Images/iconDocSmall.png"));
            pbDocumentation.Image = pbDocumentationImage;
            pbDocumentation.AvailabilityClassName = "BIMicon.BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbDocumentation.SetContextualHelp(contextHelpUrl);

            PushButton pbVersion = stackedSupport[2] as PushButton;
            pbVersion.ToolTip = "Display current version";
            pbVersion.LongDescription = "Retrieves current version";
            BitmapImage pbVersionImageSmall = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/Buttons/Support/Version/Images/iconVersionSmall.png"));
            pbVersion.Image = pbVersionImageSmall;
            pbVersion.AvailabilityClassName = "BIMicon.BIMiconToolbar.Tab.CommandAvailability";
            // Set the context help when F1 pressed
            pbVersion.SetContextualHelp(contextHelpUrl);

            #endregion
        }
    }
}