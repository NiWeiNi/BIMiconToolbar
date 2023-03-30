using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.NumberByPick
{
    /// <summary>
    /// Interaction logic for NumberByPickWPF.xaml
    /// </summary>
    public partial class NumberByPickWPF : Window, IDisposable
    {
        private Document Doc { get; set; }
        public UIDocument UIdoc { get; set; }
        public IList<ElementId> ElementIds { get; set; }
        public string StartNumber { get; set; }
        public string Prefix { get; set; }
        public ObservableCollection<ComboBoxItem> CbParameters { get; set; }
        public ComboBoxItem SelectedComboItemParameters { get; set; }

        public bool Cancel = false;
        public NumberByPickWPF(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            UIdoc = commandData.Application.ActiveUIDocument;

            InitializeComponent();

            DataContext = this;

            // Associate the event-handling method
            this.comboDisplayParameters.SelectionChanged += new SelectionChangedEventHandler(ComboDisplayParameters_SelectionChanged);
        }

        /// <summary>
        /// Function that allows user to select curve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectElements_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ElementIds = HelpersSelection.PickOrderedElements(UIdoc);

            if (ElementIds.Count > 0)
            {
                PopulateParameters(ElementIds[0]);
                this.comboDisplayParameters.ItemsSource = CbParameters;
                this.comboDisplayParameters.SelectedItem = SelectedComboItemParameters;

                this.selectElements.Content = "IDs: " + ElementIds[0].ToString();
                if (ElementIds.Count > 1)
                {
                    this.selectElements.Content = this.selectElements.Content + ", " + ElementIds[1].ToString() + ", ...";
                }
                
                this.ShowDialog();
            }
        }

        /// <summary>
        /// Method to populate parameters selection
        /// </summary>
        private void PopulateParameters(ElementId elementId)
        {
            if (CbParameters == null)
            {
                CbParameters = new ObservableCollection<ComboBoxItem>();
            }
            else
            {
                CbParameters.Clear();
            }

            Element element = Doc.GetElement(elementId);

            // Retrieve parameters
            Parameter[] parameters = Parameters.GetParametersOfInstance(element);

            IOrderedEnumerable<Parameter> orderParams = parameters.OrderBy(ph => ph.Definition.Name);

            int count = 0;

            foreach (var param in orderParams)
            {
                ComboBoxItem comb = new ComboBoxItem
                {
                    Content = param.Definition.Name,
                    Tag = param
                };
                CbParameters.Add(comb);

                if (count == 0)
                {
                    this.SelectedComboItemParameters = comb;
                    this.comboDisplayParameters.SelectedItem = SelectedComboItemParameters;
                }

                count++;
            }
        }

        /// <summary>
        /// Function to remove placeholder text in textboxes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private new void GotFocus(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.TextBox tb = (System.Windows.Controls.TextBox)sender;
            tb.Text = string.Empty;
            tb.GotFocus -= GotFocus;
        }

        /// <summary>
        /// Method to update selected parameter combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDisplayParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectedComboItemParameters != null && CbParameters.Count > 0)
            {
                int selectedItemIndex = CbParameters.IndexOf(SelectedComboItemParameters);
                this.SelectedComboItemParameters = CbParameters[selectedItemIndex];
            }
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Cancel = true;
            this.Dispose();
        }

        /// <summary>
        /// Function to accept user input and execute program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            StartNumber = this.StartNumberTextBox.Text;
            Prefix = this.PrefixTextBox.Text;

            this.Dispose();
        }

        /// <summary>
        /// Function to dispose of the window
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }
    }
}
