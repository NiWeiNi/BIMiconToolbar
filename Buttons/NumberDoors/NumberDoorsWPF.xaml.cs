using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.NumberDoors
{
    /// <summary>
    /// Interaction logic for NumberDoorsWPF.xaml
    /// </summary>
    public partial class NumberDoorsWPF : Window, IDisposable
    {
        public ObservableCollection<ComboBoxItem> CbPhases { get; set; }
        public ObservableCollection<ComboBoxItem> CbParameters { get; set; }
        public ComboBoxItem SelectedComboItemPhase { get; set; }
        public ComboBoxItem SelectedComboItemParameters { get; set; }
        public string Separator { get; set; }

        public bool optNumeric = false;

        public NumberDoorsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            // Fill properties
            PopulatePhases(doc);
            PopulateParameters(doc);

            // Associate the event-handling method with the SelectedIndexChanged event
            comboDisplayPhases.SelectionChanged += new SelectionChangedEventHandler(ComboChangedPhase);
        }

        /// <summary>
        /// Implement Disposable interface
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// Method to retrieve parameters in the document and populate the combo box
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateParameters(Document doc)
        {
            Parameter[] parameters = Parameters.GetParametersOfCategoryByStorageType(doc, BuiltInCategory.OST_Doors);

            if (parameters != null)
            {                
                CbParameters = new ObservableCollection<ComboBoxItem>();

                IOrderedEnumerable<Parameter> orderParams = parameters.OrderBy(ph => ph.Definition.Name);

                int count = 0;

                foreach (var param in orderParams)
                {
                    ComboBoxItem comb = new ComboBoxItem();
                    comb.Content = param.Definition.Name;
                    comb.Tag = param;
                    CbParameters.Add(comb);

                    if (count == 0)
                    {
                        SelectedComboItemParameters = comb;
                    }

                    count++;
                }
            }
        }

        /// <summary>
        /// Mehtod to retrieve phases from the project and add them to combo boxes
        /// </summary>
        /// <param name="doc"></param>
        private void PopulatePhases(Document doc)
        {
            CbPhases = new ObservableCollection<ComboBoxItem>();

            // Select phases
            PhaseArray aPhase = doc.Phases;

            IOrderedEnumerable<Phase> phases = aPhase.Cast<Phase>().OrderBy(ph => ph.Name);

            int count = 0;

            foreach (var ph in phases)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = ph.Name;
                comb.Tag = ph;
                CbPhases.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemPhase = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Function to change user number selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_Checked(object sender, RoutedEventArgs e)
        {
            optNumeric = !optNumeric;
        }

        /// <summary>
        /// Method to update phase according to initial phase selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedPhase(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbPhases.IndexOf(SelectedComboItemPhase);
            SelectedComboItemPhase = CbPhases[selectedItemIndex];
        }

        /// <summary>
        /// Function to output user selection
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            Separator = SeparatorTextBox.Text;
            Dispose();
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Use separator as flag to avoid execution
            Separator = null;
            Dispose();
        }
    }
}
