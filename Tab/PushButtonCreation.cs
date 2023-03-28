using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Tab.Models;
using System;
using System.Windows.Media.Imaging;

namespace BIMicon.BIMiconToolbar.Tab
{
    internal class PushButtonCreation
    {
        readonly PushButtonIdentityData _identityData;

        public PushButtonCreation(PushButtonIdentityData pushButtonIdentityData) 
        {
            _identityData = pushButtonIdentityData;
            CreatePushButton(CreatePushButtonData());
        }

        private PushButtonData CreatePushButtonData()
        {
            return new PushButtonData(
               _identityData.ButtonName,
               _identityData.ButtonDisplayName,
               _identityData.AssemblyName,
               _identityData.ClassName);
        }

        private void CreatePushButton(PushButtonData pushButtonData)
        {
            PushButton pushButton;

            if (_identityData.RibbonPanelContainer != null)
                pushButton = _identityData.RibbonPanelContainer.AddItem(pushButtonData) as PushButton;
            else if ((_identityData.SplitButtonContainer != null))
                pushButton = _identityData.SplitButtonContainer.AddPushButton(pushButtonData);
            else
                pushButton = _identityData.RibbonPanelContainer.AddItem(pushButtonData) as PushButton;

            // Assign image
            pushButton.LargeImage = new BitmapImage(new Uri(_identityData.LargeImagePath));
            pushButton.Image = new BitmapImage(new Uri(_identityData.SmallImagePath));
            pushButton.ToolTip = _identityData.ToolTip;
            pushButton.LongDescription = _identityData.LongDescription;
            // Set the context help when F1 pressed
            pushButton.SetContextualHelp(_identityData.ContextualHelpButton);
            // Set if the button is available when no project is open
            if (_identityData.AvailabilityClass != null)
                pushButton.AvailabilityClassName = _identityData.AvailabilityClass;
        }
    }
}
