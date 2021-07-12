using BIMiconToolbar.Helpers.MVVM.ViewModel;
using System.Windows.Input;

namespace BIMiconToolbar.Helpers.UserControls.OKCancel.ViewModel
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
            _okExecute = new RelayCommand(() => OKButtonPress("message"));
        }
    }
}
