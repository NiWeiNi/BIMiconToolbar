using System.ComponentModel;

namespace BIMiconToolbar.Helpers.MVVM.ViewModel
{
    /// <summary>
    /// A base view model class that implements property changed
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Event fired when any child property changes its value
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };
    }
}
