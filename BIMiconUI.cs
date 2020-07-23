using Autodesk.Revit.UI;
using System;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace BIMiconToolbar
{
    class BIMiconUI
    {
        public static void Toolbar(UIControlledApplication application)
        {
            // Create tab 
            string tabName = "BIMicon";
            application.CreateRibbonTab(tabName);

            // Create ribbon
            RibbonPanel panelProject = application.CreateRibbonPanel(tabName, "Project");

            // Retrieve assembly path
            string assemblyPath = Assembly.GetExecutingAssembly().Location;

            /*---Ribbon Panel Project---*/
            //Create push buttons for panelProject

            PushButtonData buttonNumberDoors = new PushButtonData(
                "NumberDoors",
                "Number\nDoors",
                assemblyPath,
                "BIMiconToolbar.NumberDoors.NumberDoors2020"
            );

            PushButton pbNumberDoors = panelProject.AddItem(buttonNumberDoors) as PushButton;
            pbNumberDoors.ToolTip = "Number doors according to room number.";
            pbNumberDoors.LongDescription = "Assigns a door number according to room. The primary parameter to use for door number is picked from " +
                "the ToRoom paramter from the door. If there is no room on either side of the door, no number will be assigned.";

            pbNumberDoors.LargeImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberDoors/Images/iconNumberDoors.png"));
            pbNumberDoors.Image = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberDoors/Images/iconNumberDoorsSmall.png"));
            pbNumberDoors.ToolTipImage = new BitmapImage(new Uri("pack://application:,,,/BIMiconToolbar;component/NumberDoors/Images/NumberDoorsHelp.gif"));
        }
    }
}
