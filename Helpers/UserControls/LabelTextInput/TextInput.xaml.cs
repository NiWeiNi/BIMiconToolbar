using System;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.LabelTextInput
{
    #region DPs
    /// <summary>
    /// Interaction logic for TextInput.xaml
    /// </summary>
    public partial class TextInput : UserControl
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
              typeof(TextInput), new PropertyMetadata(""));

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
              typeof(TextInput), new PropertyMetadata(null));

        /// <summary>
        /// Get or Set the orientation of the container of the TextBlock and TextBox
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                Orientation? o = GetValue(OrientationProperty) as Orientation?;
                return o.HasValue ? o.Value : Orientation.Horizontal;
            }
            set => SetValue(OrientationProperty, value);
        }

        /// <summary>
        /// Identify the Orientation dependency property
        /// </summary>
        public static readonly DependencyProperty OrientationProperty = 
            DependencyProperty.Register("Orientation", typeof(Orientation),
                typeof(TextInput), new PropertyMetadata(Orientation.Horizontal));

        #endregion

        /// <summary>
        /// Initialize component
        /// </summary>
        public TextInput()
        {
            InitializeComponent();
        }
    }
}
