using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.FilesRename
{
    /// <summary>
    /// Interaction logic for FilesRenameWPF.xaml
    /// </summary>
    public partial class FilesRenameWPF : Window, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Variables to hold user input
        public bool filesRenameBool = false;

        public ObservableCollection<ComboBoxItem> CbFileType{ get; set; }
        public ComboBoxItem SelectedComboItemFileType { get; set; }

        public string namePrefix { get; set; }
        public string nameSuffix { get; set; }

        private string nameDestinationPath;
        public string NameDestinationPath
        {
            get { return nameDestinationPath; }
            set
            {
                if (value != nameDestinationPath)
                {
                    nameDestinationPath = value;
                    OnPropertyChanged("NameDestinationPath");
                }
            }
        }

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

            // Retrieve suffix and prefix for name
            namePrefix = prefixTextBox.Text;
            nameSuffix = suffixTextBox.Text;

            NameDestinationPath = namePrefix;
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
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

        /// <summary>
        /// Function to confirm input in window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Function to close the current window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        private void PrefixTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            namePrefix = prefixTextBox.Text;
            NameDestinationPath = namePrefix;
        }
    }
}
