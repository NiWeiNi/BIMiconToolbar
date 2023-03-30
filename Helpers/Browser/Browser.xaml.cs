using BIMicon.BIMiconToolbar.Helpers.Browser.ViewModels;
using System;
using System.Windows;

namespace BIMicon.BIMiconToolbar.Helpers.Browser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class BrowserWindow : Window, IDisposable 
    {
        /// <summary>
        /// Method to call window
        /// </summary>
        public BrowserWindow()
        {
            InitializeComponent();

            this.DataContext = new BrowserStructureViewModel();
        }

        /// <summary>
        /// Implement Idisposable interface
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Method for cancel button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// String to store user's selected path
        /// </summary>
        public string selectedPath = null;

        /// <summary>
        /// Method for ok click to return selected path
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            BrowserItemViewModel selectedFolder = FolderView.SelectedItem as BrowserItemViewModel;

            if (selectedFolder != null)
            {
                // Assign selected path to variable for use in main program
                selectedPath = selectedFolder.FullPath;
            }

            this.Dispose();
        }
    }
}
