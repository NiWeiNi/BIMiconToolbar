using System.Windows.Data;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.WPF.Models
{
    public class EditTextBox : TextBox
    {
        #region Private fields

        private string startText;

        #endregion Private fields

        #region Constructor

        static EditTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(EditTextBox), new FrameworkPropertyMetadata(typeof(EditTextBox)));
        }

        public EditTextBox()
        {
            Loaded += OnTreeViewEditTextBoxLoaded;
        }

        #endregion Constructor

        #region Methods

        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            startText = Text;
            SelectAll();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            if (!e.Handled)
            {
                Key key = e.Key;
                switch (key)
                {
                    case Key.Escape:
                        Text = startText;
                        break;
                }
            }
        }

        private void OnTreeViewEditTextBoxLoaded(object sender, RoutedEventArgs e)
        {
            BindingExpression be = GetBindingExpression(TextProperty);
            if (be != null) be.UpdateTarget();
            FocusHelper.Focus(this);
        }

        #endregion
    }
}
