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
            Autodesk.Revit.UI.RibbonPanel panelProject = application.CreateRibbonPanel(tabName, "Project");
            Autodesk.Revit.UI.RibbonPanel panelSheets = application.CreateRibbonPanel(tabName, "Sheets");

            // Retrieve assembly path
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

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
            #endregion

            /*---Ribbon Panel Sheets---*/
            #region Ribbon Panel Sheets

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

            #endregion
        }
    }
}