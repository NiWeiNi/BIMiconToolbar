using BIMicon.BIMiconToolbar.Models.MVVM.ViewModel;
using System.Windows.Input;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.OKCancel.ViewModel
{
    class OKCancelViewModel : ViewModelBase
    {
        private readonly ICommand _okExecute;
        public ICommand OKExecute
        {
            get { return _okExecute; }
        }

        public void OKButtonPress(string message)
        {
            
        }

        public OKCancelViewModel()
        {
            
        }
    }
}
