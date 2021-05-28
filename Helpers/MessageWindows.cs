using BIMiconToolbar.Helpers.MessageWindow;

namespace BIMiconToolbar.Helpers
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
                customWindow.ShowDialog();
            }
        }
    }
}
