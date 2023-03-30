using System;
using System.Windows;

namespace BIMicon.BIMiconToolbar.Helpers.MessageWindow
{
    /// <summary>
    /// Interaction logic for MessageBoxWPF.xaml
    /// </summary>
    public partial class MessageWindowWPF : Window, IDisposable
    {
        private string _windowTitle;
        public string WindowTitle { get { return _windowTitle; } set { _windowTitle = value; } }

        private string _errorMessage;
        public string ErrorMessage { get{return _errorMessage;} set { _errorMessage = value; } }

        public MessageWindowWPF(string windowTitle, string errorMessage)
        {
            InitializeComponent();
            this.Title = windowTitle;
            TextMessage.Text = errorMessage;
        }

        /// <summary>
        /// Method to dispose window
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Method executed when OK button is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }
    }
}
