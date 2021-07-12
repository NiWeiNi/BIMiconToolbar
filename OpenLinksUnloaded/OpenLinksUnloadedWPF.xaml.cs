using Autodesk.Revit.DB;
using BIMiconToolbar.Helpers.UserControls.SelectFileReferences.ViewModel;
using System;
using System.ComponentModel;
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
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public SelectFileReferencesViewModel selectFileReferencesViewModel { get; set; }

        private string _text;
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                _text = value;
                OnPropertyChanged();
            }
        }

        public OpenLinksUnloadedWPF()
        {
            InitializeComponent();
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

        private void OKCancelButtons_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Text = FileBrowser.FilePath;
        }
    }
}
