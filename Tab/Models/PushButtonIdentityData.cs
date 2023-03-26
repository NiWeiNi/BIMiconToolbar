using Autodesk.Revit.UI;

namespace BIMicon.BIMiconToolbar.Tab.Models
{
    internal class PushButtonIdentityData
    {
        public string ButtonName { get; set; }
        public string ButtonDisplayName { get; set; }
        public string AssemblyName { get; set; }
        public string ClassName { get; set; }
        public string ToolTip { get; set; }
        public string LongDescription { get; set; }
        public string AvailabilityClass { get; set; }
        public RibbonPanel RibbonPanelContainer { get; set; }
        public SplitButton SplitButtonContainer { get; set; }
        public ContextualHelp ContextualHelpButton { get; set; }
        public string LargeImagePath { get; set; }
        public string SmallImagePath { get; set; }

        public PushButtonIdentityData() { }
    }
}
