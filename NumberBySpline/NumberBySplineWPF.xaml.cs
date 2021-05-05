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
        private Document doc { get; set; }
        public UIDocument uidoc { get; set; }
        public ElementId CurveId { get; set; }
        public bool NumericNumber = true;
        public string StartNumber { get; set; }
        public ObservableCollection<ComboBoxItem> CbCategories { get; set; }
        public ComboBoxItem SelectedComboItemCategories { get; set; }
        public ObservableCollection<ComboBoxItem> CbLevels { get; set; }
        public ComboBoxItem SelectedComboItemLevels { get; set; }
        public ObservableCollection<ComboBoxItem> CbParameters { get; set; }
        public ComboBoxItem SelectedComboItemParameters { get; set; }

        /// <summary>
        /// Function to initialize window
        /// </summary>
        /// <param name="commandData"></param>
        public NumberBySplineWPF(ExternalCommandData commandData)
        {
            doc = commandData.Application.ActiveUIDocument.Document;
            uidoc = commandData.Application.ActiveUIDocument;

            InitializeComponent();
            DataContext = this;

            PopulateCategories(doc);
            PopulateLevels(doc);
            PopulateParameters();

            // Associate the event-handling method with the category changed
            this.comboDisplayCategories.SelectionChanged += new SelectionChangedEventHandler(ComboDisplayCategories_SelectionChanged);
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

            IOrderedEnumerable<Category> orderedCategories = categories.Cast<Category>()
                .Where(cat => cat.Name.Contains("Tag") == false)
                .OrderBy(cat => cat.Name);

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
        /// Function to populate combobox with categories
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateLevels(Document doc)
        {
            CbLevels = new ObservableCollection<ComboBoxItem>();

            // Retrieve levels
            var levels = new FilteredElementCollector(doc)
                        .OfCategory(BuiltInCategory.OST_Levels)
                        .WhereElementIsNotElementType()
                        .ToElements();
           
            IOrderedEnumerable<Level> orderedLevels = levels.Cast<Level>().OrderBy(lvl => lvl.Name);

            int count = 0;

            foreach (Level lvl in orderedLevels)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = lvl.Name;
                comb.Tag = lvl;
                CbLevels.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemLevels = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Method to populate parameters selection
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateParameters()
        {
            if (CbParameters == null)
            {
                CbParameters = new ObservableCollection<ComboBoxItem>();
            }
            else
            {
                CbParameters.Clear();
            }

            Category cat = SelectedComboItemCategories.Tag as Category;
            Element element = new FilteredElementCollector(doc)
                .OfCategoryId(cat.Id)
                .WhereElementIsNotElementType()
                .FirstOrDefault();

            if (element != null)
            {
                CbParameters.Clear();

                // Retrieve parameters
                Parameter[] parameters = Helpers.Parameters.GetParametersOfInstance(element);

                IOrderedEnumerable<Parameter> orderParams = parameters.OrderBy(ph => ph.Definition.Name);

                foreach (var param in orderParams)
                {
                    ComboBoxItem comb = new ComboBoxItem();
                    comb.Content = param.Definition.Name;
                    comb.Tag = param;
                    CbParameters.Add(comb);
                }
            }
            else
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = "No instances of selected category found in the model";
                comb.Tag = "";
                CbParameters.Add(comb);
            }

            SelectedComboItemParameters = CbParameters[0];
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
            PopulateParameters();
        }

        /// <summary>
        /// Function to update levels combobox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDisplayLevels_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbLevels.IndexOf(SelectedComboItemLevels);
            SelectedComboItemLevels = CbLevels[selectedItemIndex];
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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboDisplayParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void comboDisplayParameters_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            if (CbParameters.Count != 0)
            {
                int selectedItemIndex = CbParameters.IndexOf(SelectedComboItemParameters);
                SelectedComboItemParameters = CbParameters[selectedItemIndex];
            }
        }
    }
}
