using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.FilesRename
{
    /// <summary>
    /// Interaction logic for FilesRenameWPF.xaml
    /// </summary>
    public partial class FilesRenameWPF : Window, IDisposable
    {
        // Variables to hold user input
        public bool filesRenameBool = false;
        public ObservableCollection<ComboBoxItem> CbFileType{ get; set; }
        public ComboBoxItem SelectedComboItemFileType { get; set; }

        /// <summary>
        /// Main function to call window
        /// </summary>
        public FilesRenameWPF(string selectedPath)
        {
            InitializeComponent();
            DataContext = this;

            // Set path to display as selectedPath
            this.currentPath.Text = selectedPath;

            // Populate comboBox
            FileTypes(selectedPath);
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Functon to toggle file or folder selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FoldersRename_Checked(object sender, RoutedEventArgs e)
        {
            this.filesRenameBool = !filesRenameBool;

            // Toggle enable/dissable dropdown list of file types
            this.comboDisplayFileType.IsEnabled = !this.comboDisplayFileType.IsEnabled;
        }

        /// <summary>
        /// Function to retrieve file types
        /// </summary>
        /// <param name="selectedPath"></param>
        public void FileTypes(string selectedPath)
        {
            CbFileType = new ObservableCollection<ComboBoxItem>();

            var files = Helpers.HelpersDirectory.RetrieveFiles(selectedPath);

            var fileTypes = Helpers.HelpersDirectory.GetFilesType(files);

            // Populate the comboBoxes
            bool populateFirst = true;

            foreach (var ext in fileTypes)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = ext;
                CbFileType.Add(comb);

                // Set the first item to display in comboBox
                if (populateFirst)
                {
                    SelectedComboItemFileType = comb;
                    populateFirst = false;
                }
            }
        }
    }
}
