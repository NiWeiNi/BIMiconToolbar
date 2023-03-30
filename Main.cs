using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Tab;
using BIMicon.BIMiconToolbar.FamilyBrowser;
using System;

namespace BIMicon.BIMiconToolbar
{
    public class Main : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            new BIMiconUI(application);
            //RegisterDockPanel(application);
            return Result.Succeeded;
        }

        /// <summary>
        /// Method to register dock panel at zero doc state
        /// </summary>
        /// <param name="app"></param>
        private void RegisterDockPanel(UIControlledApplication app)
        {
            FamilyBrowserWPF dockPage = new FamilyBrowserWPF();
            DockablePaneId dpId = new DockablePaneId(new Guid("{22827024-7B1A-4D88-80A5-1A8E894F1057}"));
            app.RegisterDockablePane(dpId, "Family Browser", dockPage as IDockablePaneProvider);
        }
    }
}
