using Autodesk.Revit.UI;
using System;

namespace BIMicon.BIMiconToolbar.Tab.Models
{
    internal class PushButtonIdentityData
    {
        #region Private Properties
        readonly string _buttonName;
        readonly string _buttonDisplayName;
        readonly string _assemblyName;
        readonly string _className;
        readonly string _toolTip;
        readonly string _longDescription;
        readonly string _availabilityClass;
        readonly RibbonPanel _ribbonPanelContainer;
        readonly ContextualHelp _contextualHelpButton;
        readonly Uri _largeImageUri;
        readonly Uri _smallImageUri;
        #endregion
        #region Public Properties
        public string ButtonName { get { return _buttonName; } }
        public string ButtonDisplayName { get { return _buttonDisplayName; } }
        public string AssemblyName { get { return _assemblyName; } }
        public string ClassName { get { return _className; } }
        public string ToolTip { get { return _toolTip; } }
        public string LongDescription { get { return _longDescription; } }
        public string AvailabilityClass { get { return _availabilityClass; } }
        public RibbonPanel RibbonPanelContainer { get { return _ribbonPanelContainer; } }
        public ContextualHelp ContextualHelpButton { get { return _contextualHelpButton; } }
        public Uri LargeImageUri { get { return _largeImageUri; } }
        public Uri SmallImageUri { get  => _smallImageUri; }
        #endregion

        public PushButtonIdentityData(string buttonName, string buttonDisplayName, string assemblyName, string className,
            string toolTip, string longDescription, string availabilityClass, RibbonPanel ribbonPanel,
            ContextualHelp contextualHelp, string largeImagePath, string smallImagePath)
        {
            _buttonName = buttonName;
            _buttonDisplayName = buttonDisplayName;
            _assemblyName = assemblyName;
            _className = className;
            _toolTip = toolTip;
            _longDescription = longDescription;
            _availabilityClass = availabilityClass;

            _ribbonPanelContainer = ribbonPanel;
            _contextualHelpButton = contextualHelp;

            _largeImageUri = new Uri(largeImagePath);
            _smallImageUri = new Uri(smallImagePath);
        }
    }
}
