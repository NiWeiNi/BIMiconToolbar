using Autodesk.Revit.UI;
using Autodesk.Windows;

namespace BIMicon.BIMiconToolbar.Helpers
{
    internal static class RevitRibbon
    {
        /// <summary>
        /// Method to check to create a tab with given name if it does not exist in the Revit UI
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="application"></param>
        public static void CheckRibbonTabExist(string tabName, UIControlledApplication application)
        {
            bool hasTab = false;
            foreach (RibbonTab ribbonTab in ComponentManager.Ribbon.Tabs)
            {
                if (ribbonTab.Name == tabName)
                {
                    hasTab = true;
                    break;
                }
            }
            if (!hasTab)
                application.CreateRibbonTab(tabName);
        }
    }
}
