using Autodesk.Revit.UI;
using System;
using System.Windows.Media.Imaging;

namespace BIMiconToolbar.Models
{
    internal class PushButtonIdentityData
    {
        readonly string _buttonName;
        readonly string _buttonDisplayName;
        readonly string _assemblyName;
        readonly string _className;
        readonly string _toolTip;
        readonly string _longDescription;
        readonly string _availabilityClass;
        readonly RibbonPanel _ribbonPanel;
        readonly ContextualHelp _contextualHelp;
        readonly Uri _largeImageUri;
        readonly Uri _smallImageUri;

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

            _ribbonPanel = ribbonPanel;
            _contextualHelp = contextualHelp;

            _largeImageUri = new Uri(largeImagePath);
            _smallImageUri = new Uri(smallImagePath);
        }

        private PushButtonData CreatePushButtonData()
        {
            return new PushButtonData(
               _buttonName,
               _buttonDisplayName,
               _assemblyName,
               _className);
        }

        private PushButton CreatePushButton(PushButtonData pushButtonData)
        {
            PushButton pushButton = _ribbonPanel.AddItem(pushButtonData) as PushButton;

            // Assign image
            pushButton.LargeImage = new BitmapImage(_largeImageUri);
            pushButton.Image = new BitmapImage(_smallImageUri);
            pushButton.ToolTip = _toolTip;
            pushButton.LongDescription = _longDescription;
            // Set the context help when F1 pressed
            pushButton.SetContextualHelp(_contextualHelp);
            // Set if the button is available when no project is open
            if (_availabilityClass != null)
                pushButton.AvailabilityClassName = _availabilityClass;

            return pushButton;
        }
    }
}
