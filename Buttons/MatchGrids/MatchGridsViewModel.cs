using BIMicon.BIMiconToolbar.Models;
using BIMicon.BIMiconToolbar.Models.MVVM;
using BIMicon.BIMiconToolbar.Models.MVVM.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BIMicon.BIMiconToolbar.Buttons.MatchGrids
{
    internal class MatchGridsViewModel : ViewModelBase
    {
        public ObservableCollection<BaseElement> Views { get; set; }
        public ObservableCollection<BaseElement> ViewToCopy { get; set; }

        public MatchGridsViewModel() { }
    }
}
