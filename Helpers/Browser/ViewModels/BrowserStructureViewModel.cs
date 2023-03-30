using System.Collections.ObjectModel;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Helpers.Browser.ViewModels
{
    /// <summary>
    /// The view model for the applications main Directory view
    /// </summary>
    public class BrowserStructureViewModel : BaseViewModel
    {
        #region Public Properties
        /// <summary>
        /// A list of all directories in the machine
        /// </summary>
        public ObservableCollection<BrowserItemViewModel> Items { get; set; }

        #endregion 

        /// <summary>
        /// Default constructor
        /// </summary>
        public BrowserStructureViewModel()
        {
            var children = BrowserStructure.GetLogicalDrives();

            this.Items = new ObservableCollection<BrowserItemViewModel>(
                            children.Select(drive => new BrowserItemViewModel(drive.FullPath, Data.BrowserItemType.Drive)));
        }
    }
}
