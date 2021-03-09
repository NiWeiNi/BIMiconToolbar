using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.NumberBySpline
{
    /// <summary>
    /// Interaction logic for NumberBySplineWPF.xaml
    /// </summary>
    public partial class NumberBySplineWPF : Window, IDisposable
    {
        public UIDocument uidoc { get; set; }
        public ElementId CurveId { get; set; }
        public bool NumericNumber = true;
        public string StartNumber;
        public ObservableCollection<ComboBoxItem> CbCategories { get; set; }
        public ComboBoxItem SelectedComboItemCategories { get; set; }

        /// <summary>
        /// Function to initialize window
        /// </summary>
        /// <param name="commandData"></param>
        public NumberBySplineWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            uidoc = commandData.Application.ActiveUIDocument;

            InitializeComponent();
            DataContext = this;

            PopulateCategories(doc);
        }

        /// <summary>
        /// Function to acceot user input and execute program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            StartNumber = this.StartNumberTextBox.Text;

            this.Dispose();
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Function that allows user to select curve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSpline_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ElementId selectedCurve = Helpers.HelpersSelection.PickLine(uidoc);

            CurveId = selectedCurve;
            
            if (CurveId != null)
            {
                this.selectSpline.Content = "ID: " + CurveId.ToString();
                this.ShowDialog();
            }
        }

        /// <summary>
        /// Function to dispose of the window
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Function to select number with numbers or letters
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Numeric_Unchecked(object sender, RoutedEventArgs e)
        {
            NumericNumber = !NumericNumber;
        }

        /// <summary>
        /// Function to populate combobox with categories
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateCategories(Document doc)
        {
            CbCategories = new ObservableCollection<ComboBoxItem>();

            // Retrieve categories in document
            Categories categories = doc.Settings.Categories;

            IOrderedEnumerable<Category> orderedCategories = categories.Cast<Category>().OrderBy(cat => cat.Name);

            int count = 0;

            foreach (Category cat in orderedCategories)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = cat.Name;
                comb.Tag = cat;
                CbCategories.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemCategories= comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Function to update combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDisplayCategories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbCategories.IndexOf(SelectedComboItemCategories);
            SelectedComboItemCategories = CbCategories[selectedItemIndex];
        }
    }
}
