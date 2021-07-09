using Autodesk.Revit.DB;
using BIMiconToolbar.Helpers.UserControls.FileBrowser.ViewModel;
using BIMiconToolbar.Helpers.UserControls.SelectFileReferences.ViewModel;
using System;
using System.Windows;

namespace BIMiconToolbar.OpenLinksUnloaded
{
    /// <summary>
    /// Interaction logic for OpenLinksUnloadedWPF.xaml
    /// </summary>
    public partial class OpenLinksUnloadedWPF : Window, IDisposable
    {
        public OpenLinksUnloadedWPF()
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            this.Close();
        }

        private void SelectFileReferences_Loaded(object sender, RoutedEventArgs e)
        {
            ExternalFileReferenceType[] extFileRefT = { ExternalFileReferenceType.RevitLink, ExternalFileReferenceType.CADLink };

            SelectFileReferencesViewModel selectFileReferencesViewModel = new SelectFileReferencesViewModel();
            selectFileReferencesViewModel.LoadFileReferences(extFileRefT);
            SelectFileReferences.DataContext = selectFileReferencesViewModel;
        }

        private void OKCancelButtons_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void FileBrowser_Loaded(object sender, RoutedEventArgs e)
        {
            FileBrowserViewModel fileBrowserViewModelObject = new FileBrowserViewModel();
            FileBrowser.DataContext = fileBrowserViewModelObject;
        }
    }
}
