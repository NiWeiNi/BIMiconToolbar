using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.LabelComboBox
{
    /// <summary>
    /// Interaction logic for UCComboBox.xaml
    /// </summary>
    public partial class UCComboBox : UserControl
    {
        /// <summary>
        /// Get or Set text in TextBlock
        /// </summary>
        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }

        /// <summary>
        /// Identify the Label dependency property
        /// </summary>
        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string),
              typeof(UCComboBox), new PropertyMetadata(""));

        /// <summary>
        /// Get or Set value displayed in TextBox
        /// </summary>
        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }

        /// <summary>
        /// Identify the Value dependency property
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object),
              typeof(UCComboBox), new PropertyMetadata(null));

        public UCComboBox()
        {
            InitializeComponent();
        }
    }
}
