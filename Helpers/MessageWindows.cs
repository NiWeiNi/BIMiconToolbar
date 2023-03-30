using BIMicon.BIMiconToolbar.Helpers.MessageWindow;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class MessageWindows
    {
        /// <summary>
        /// Method to display custom alert messages to user
        /// </summary>
        /// <param name="windowTitle"></param>
        /// <param name="errorMessage"></param>
        public static void AlertMessage(string windowTitle, string errorMessage)
        {
            using (MessageWindowWPF customWindow = new MessageWindowWPF(windowTitle, errorMessage))
            {
                // Place message at the top of Revit 
                System.Windows.Interop.WindowInteropHelper helper = new System.Windows.Interop.WindowInteropHelper(customWindow);
                helper.Owner = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;

                customWindow.ShowDialog();
            }
        }
    }
}
