using BIMicon.BIMiconToolbar.Helpers.UserControls.SelectFileReferences.Model;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.SelectFileReferences
{
    /// <summary>
    /// Interaction logic for SelectFileLinkTypes.xaml
    /// </summary>
    public partial class SelectFileLinkTypes : UserControl
    {
        public ObservableCollection<SelectFileReferencesModel> FileReferences { get; set; }

        public SelectFileLinkTypes()
        {
            InitializeComponent();
        }

    }
}
