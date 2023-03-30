using BIMicon.BIMiconToolbar.Helpers.Browser.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace BIMicon.BIMiconToolbar.Helpers.Browser.ViewModels
{
    /// <summary>
    /// A view model for each browser item
    /// </summary>
    public class BrowserItemViewModel : BaseViewModel
    {
        #region Properties
        /// <summary>
        /// The type of this item
        /// </summary>
        public BrowserItemType Type { get; set; }

        /// <summary>
        /// The full path to the item
        /// </summary>
        public string FullPath { get; set; }

        public string Name { get { return this.Type == BrowserItemType.Drive ? this.FullPath : BrowserStructure.GetFileFolderName(this.FullPath); } }
    
        /// <summary>
        /// A list of all children contained inside this item
        /// </summary>
        public ObservableCollection<BrowserItemViewModel> Children { get; set; }

        /// <summary>
        /// Indicates if item can be expanded
        /// </summary>
        public bool CanExpand { get { return (this.Type == BrowserItemType.Folder || this.Type == BrowserItemType.Drive); } }

        /// <summary>
        /// Indicates if current item is expanded
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return this.Children?.Count(f => f != null) > 0;
            }
            set
            {
                if (value == true)
                {
                    Expand();
                }
                else
                {
                    this.ClearChildren();
                }
            }
        }

        #endregion

        #region Public Commands

        /// <summary>
        /// The command to expand this item
        /// </summary>
        public ICommand ExpandCommand { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="fullPath">The full path of this item</param>
        /// <param name="type">The type of item</param>
        public BrowserItemViewModel(string fullPath, BrowserItemType type)
        {
            this.ExpandCommand = new RelayCommand(Expand);

            this.FullPath = fullPath;
            this.Type = type;

            // Setup the children as needed
            this.ClearChildren();
        }
        #endregion

        #region Helper Methods

        /// <summary>
        /// Removes all children on the list, adding a dummy item to show the expand icon
        /// </summary>
        private void ClearChildren()
        {
            this.Children = new ObservableCollection<BrowserItemViewModel>();

            // Show expand arrow to not files
            if (this.Type == BrowserItemType.Drive || this.Type == BrowserItemType.Folder)
            {
                this.Children.Add(null);
            }
        }

        #endregion

        /// <summary>
        /// Expands this directory and finds all children
        /// </summary>
        private void Expand()
        {
            if (this.Type == BrowserItemType.File)
            {
                return;
            }

            var children = BrowserStructure.GetDirectoryContents(this.FullPath);
            this.Children = new ObservableCollection<BrowserItemViewModel>(
                                children.Select(content => new BrowserItemViewModel(content.FullPath, content.Type)));
        }
    }
}
