using System;
using System.Windows.Input;

namespace BIMicon.BIMiconToolbar.Models.MVVM.ViewModel
{
    /// <summary>
    /// A basic command that runs an Action
    /// </summary>
    public class RelayCommand : ICommand
    {
        #region Private Members
        
        /// <summary>
        /// The action to run
        /// </summary>
        private Action<object> execute;
        private Func<object, bool> canExecute;

        #endregion

        #region Public Events
        /// <summary>
        /// The event that is fired when the value has changed
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        #endregion

        #region Command Methods
        /// <summary>
        /// A relay command can always execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            if (parameter == null)
                return canExecute == null;
            else
                return canExecute(parameter);
        }

        /// <summary>
        /// Executes the commands Action
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion
    }
}
