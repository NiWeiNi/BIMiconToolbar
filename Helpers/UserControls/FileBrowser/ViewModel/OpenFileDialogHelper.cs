using BIMiconToolbar.Helpers.UserControls.FileBrowser.Model;
using System.Windows.Forms;

namespace BIMiconToolbar.Helpers.UserControls.FileBrowser.ViewModel
{
    class OpenFileDialogHelper
    {
        public static void SelectFile(FileBrowserViewModel fileBrowserViewModel)
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
                    fileBrowserViewModel.SelectedFilePath = fileBrowserModel.FilePath;

                }
            }
        }
    }
}
