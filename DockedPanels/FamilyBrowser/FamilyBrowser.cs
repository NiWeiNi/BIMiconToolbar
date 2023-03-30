using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System;

namespace BIMicon.BIMiconToolbar.FamilyBrowser
{
    [TransactionAttribute(TransactionMode.Manual)]
    class FamilyBrowser : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Application app = uiApp.Application;
            Document doc = uiApp.ActiveUIDocument.Document;

            // Create the Family Browser dock panel registered on startup
            DockablePaneId dpId = new DockablePaneId(new Guid("{22827024-7B1A-4D88-80A5-1A8E894F1057}"));
            DockablePane dockFamilyBrowser = commandData.Application.GetDockablePane(dpId);

            // Toggle the dock panel visibility
            if (dockFamilyBrowser.IsShown())
            {
                dockFamilyBrowser.Hide();
            }
            else
            {
                dockFamilyBrowser.Show();
            }

            string path = RevitDirectories.RevitContentPath(app);

            return Result.Succeeded;
        }
    }
}
