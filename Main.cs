using Autodesk.Revit.UI;

namespace BIMiconToolbar
{
    public class Main : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            BIMiconUI.Toolbar(application);
            return Result.Succeeded;
        }
    }
}
