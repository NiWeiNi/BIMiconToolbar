using BIMicon.BIMiconToolbar.Models.MVVM.ViewModel;
using System.Collections.Generic;
using System.IO;

namespace BIMicon.BIMiconToolbar.FamilyBrowser.ViewModel
{
    class FamilyBrowserStructure : ViewModelBase
    {
        private IList<FamilyItem> _familyCollection;

        public IList<FamilyItem> FamilyCollection
        {
            get { return _familyCollection; }

            set { SetProperty(ref _familyCollection, value); }
        }

        public void PopulateFamilies()
        {
            FamilyCollection = FileExplorer.GetFamiliesFromDirectory(@"C:\BIMicon\01-BIM Implementation\02-Content Library\01-Library\Doors");
        }
    }
}
