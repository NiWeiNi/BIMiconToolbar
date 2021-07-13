using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMiconToolbar.Helpers.UserControls.SelectFileReferences.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BIMiconToolbar.OpenLinksUnloaded
{
    /// <summary>
    /// Interaction logic for OpenLinksUnloadedWPF.xaml
    /// </summary>
    public partial class OpenLinksUnloadedWPF : Window, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public UIApplication UIApp { get; set; }
        public SelectFileReferencesViewModel selectFileReferencesViewModel { get; set; }
        public List<ExternalFileReferenceType> SelectedFileReferences { get; set; }
        public string SelectedFilePath { get; set; }

        private string _warningMessage;
        public string WarningMessage
        {
            get { return _warningMessage; }
            set
            {
                _warningMessage = value;
                OnPropertyChanged();
            }
        }

        public OpenLinksUnloadedWPF(UIApplication uiApp)
        {
            InitializeComponent();
            UIApp = uiApp;
        }

        public void Dispose()
        {
            this.Close();
        }

        public void SelectFileReferences_Loaded(object sender, RoutedEventArgs e)
        {
            ExternalFileReferenceType[] extFileRefT = { ExternalFileReferenceType.RevitLink, ExternalFileReferenceType.CADLink };

            selectFileReferencesViewModel = new SelectFileReferencesViewModel();
            selectFileReferencesViewModel.LoadFileReferences(extFileRefT);
            SelectFileReferences.DataContext = selectFileReferencesViewModel;
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            // Assign selected input to properties
            SelectedFilePath = FileBrowser.FilePath;
            SelectedFileReferences = new List<ExternalFileReferenceType>();

            foreach (var item in selectFileReferencesViewModel.FileReferences)
            {
                if (item.IsSelected)
                {
                    SelectedFileReferences.Add(item.ReferenceType);
                }
            }

            // Check for user input
            if (SelectedFileReferences.Count == 0 && (SelectedFilePath == null || SelectedFilePath == ""))
            {
                WarningMessage = "Please select Revit project and file links to unload.";
            }
            else if (SelectedFileReferences.Count == 0)
            {
                WarningMessage = "Please select file links to unload.";
            }
            else if (SelectedFilePath == null || SelectedFilePath == "")
            {
                WarningMessage = "Please select Revit project.";
            }
            // Execute the program
            else
            {
                // Reset warning message
                WarningMessage = "";

                // Variable for final modelpath
                FilePath finalPath = null;

                // Create ModelPath to project file
                FilePath modelPath = new FilePath(SelectedFilePath);

                // Check if model is workshared
                bool isWorkshared = BasicFileInfo.Extract(SelectedFilePath).IsWorkshared;

                if (isWorkshared)
                {
                    FileInfo modelInfo = new FileInfo(SelectedFilePath);
                    string modelName = modelInfo.Name;
                    string localName = modelName.Replace(".rvt", "") + " - LinksUnloaded.rvt";

                    // Get user documents folder path and copy a local file
                    string docPath = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    string localPathString = docPath + localName;
                    FilePath localPath = new FilePath(localPathString);

                    TransmissionData transData = TransmissionData.ReadTransmissionData(modelPath);
                    if (transData.IsTransmitted)
                    {
                        transData.IsTransmitted = false;
                        TransmissionData.WriteTransmissionData(modelPath, transData);
                    }

                    WorksharingUtils.CreateNewLocal(modelPath, localPath);

                    finalPath = new FilePath(localPathString);
                }
                else
                {
                    finalPath = modelPath;

                    // Unload links
                    Helpers.RevitDirectories.UnloadLinks(finalPath, SelectedFileReferences);
                }

                // Open document
                OpenOptions openOptions = new OpenOptions();
                UIApp.OpenAndActivateDocument(finalPath, openOptions, false);
            }

            // Close window
            this.Dispose();
        }
    }
}
