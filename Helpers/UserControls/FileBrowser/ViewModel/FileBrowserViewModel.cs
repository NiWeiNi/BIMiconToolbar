using BIMiconToolbar.Helpers.MVVM.ViewModel;
using BIMiconToolbar.Helpers.UserControls.FileBrowser.Model;
using System.Windows.Forms;
using System.Windows.Input;

namespace BIMiconToolbar.Helpers.UserControls.FileBrowser.ViewModel
{
    public class FileBrowserViewModel : ViewModelBase
    {
        private readonly ICommand _selectFile;
        private FileBrowserModel SelectedFileModel;
        private string _selectedFilePath;
        public string SelectedFilePath
        {
            get { return _selectedFilePath; }
            set { SetProperty(ref _selectedFilePath, value); }
        }

        public void SelectFileDialog()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "";
                openFileDialog.Filter = "Revit files (*.rvt)|*.rvt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Assign the path to object
                    FileBrowserModel fileBrowserModel = new FileBrowserModel();

                    fileBrowserModel.FilePath = openFileDialog.FileName;
                    SelectedFilePath = openFileDialog.FileName;
                }
            }
        }

        public ICommand SelectFile
        {
            get { return _selectFile; }
        }

        public FileBrowserViewModel()
        {
            _selectFile = new RelayCommand(() => SelectFileDialog());
        }
    }
}
