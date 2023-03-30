using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace BIMicon.BIMiconToolbar.NumberBySpline
{
    /// <summary>
    /// Interaction logic for NumberBySplineWPF.xaml
    /// </summary>
    public partial class NumberBySplineWPF : Window, IDisposable
    {
        private Document doc { get; set; }
        public UIDocument uidoc { get; set; }
        public ElementId CurveId { get; set; }
        public bool Cancel = false;
        public string StartNumber { get; set; }
        public string Prefix { get; set; }
        public ObservableCollection<ComboBoxItem> CbCategories { get; set; }
        public ComboBoxItem SelectedComboItemCategories { get; set; }
        public ObservableCollection<ComboBoxItem> CbLevels { get; set; }
        public ComboBoxItem SelectedComboItemLevels { get; set; }
        public ObservableCollection<ComboBoxItem> CbParameters { get; set; }
        public ComboBoxItem SelectedComboItemParameters { get; set; }
        public bool levelDisplay = false;

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

            // Associate the event-handling method with the category changed
            this.comboDisplayCategories.SelectionChanged += new SelectionChangedEventHandler(ComboDisplayCategories_SelectionChanged);
            this.comboDisplayParameters.SelectionChanged += new SelectionChangedEventHandler(ComboDisplayParameters_SelectionChanged);
        }

        /// <summary>
        /// Function to acceot user input and execute program
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
        /// Function that allows user to select curve
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectSpline_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

            ElementId selectedCurve = HelpersSelection.PickLine(uidoc);

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
                .Where(cat => new FilteredElementCollector(doc).OfCategoryId(cat.Id).WhereElementIsNotElementType().FirstOrDefault() != null)
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

            PopulateLevels(doc);
        }

        /// <summary>
        /// Function to populate combobox with categories
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateLevels(Document doc)
        {
            // Case category selected
            if (SelectedComboItemCategories != null)
            {
                Category cat = SelectedComboItemCategories.Tag as Category;

                // Check if elements in category are level based
                if (Parameters.IsElementLevelBased(doc, cat.Id))
                {
                    // Generate UI for user level input
                    if (levelDisplay == false)
                    {
                        CbLevels = new ObservableCollection<ComboBoxItem>();

                        // Style for controls
                        Style styleTextBlock = this.FindResource("Title") as Style;
                        Style styleComboBox = this.FindResource("comboDisplay") as Style;

                        // Create title for level input
                        TextBlock levelTitle = new TextBlock();
                        levelTitle.Text = "Select Level:";
                        levelTitle.Style = styleTextBlock;

                        // Create combobox for level selection
                        System.Windows.Controls.ComboBox comboBox = new System.Windows.Controls.ComboBox();
                        comboBox.ItemsSource = CbLevels;
                        comboBox.Style = styleComboBox;

                        // Retrieve levels
                        var levels = new FilteredElementCollector(doc)
                                    .OfCategory(BuiltInCategory.OST_Levels)
                                    .WhereElementIsNotElementType()
                                    .ToElements();

                        IOrderedEnumerable<Level> orderedLevels = levels.Cast<Level>().OrderBy(lvl => lvl.Name);

                        int count = 0;

                        System.Windows.Controls.ComboBox cbox = new System.Windows.Controls.ComboBox();

                        foreach (Level lvl in orderedLevels)
                        {
                            ComboBoxItem comb = new ComboBoxItem();
                            comb.Content = lvl.Name;
                            comb.Tag = lvl;
                            CbLevels.Add(comb);

                            if (count == 0)
                            {
                                SelectedComboItemLevels = comb;

                                // Bind selected level to property
                                System.Windows.Data.Binding comboBinding = new System.Windows.Data.Binding();
                                comboBinding.Mode = BindingMode.TwoWay;
                                comboBinding.Source = this;
                                comboBinding.Path = new PropertyPath("SelectedComboItemLevels");
                                comboBox.SetBinding(System.Windows.Controls.ComboBox.SelectedItemProperty, comboBinding);
                            }

                            count++;
                        }

                        comboBox.SelectionChanged += ComboBoxLevel_SelectionChanged;
                        // Add controls to wpf window
                        level.Children.Add(levelTitle);
                        level.Children.Add(comboBox);
                        levelDisplay = true;
                    }
                }
                // Remove level UI if selected category doesn't have level parameter
                else if (Parameters.IsElementLevelBased(doc, cat.Id) == false && levelDisplay == true)
                {
                    level.Children.Clear();
                    level.UpdateLayout();
                    levelDisplay = false;
                }
            }          
        }

        /// <summary>
        /// Method to change selected level
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBoxLevel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbLevels.IndexOf(SelectedComboItemLevels);
            SelectedComboItemLevels = CbLevels[selectedItemIndex];
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

            // Retrieve parameters
            Parameter[] parameters = Parameters.GetParametersOfInstance(element);

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
                    comboDisplayParameters.SelectedItem = SelectedComboItemParameters;
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
            PopulateParameters();
            PopulateLevels(doc);
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
            if (SelectedComboItemParameters != null && CbParameters.Count > 0)
            {
                int selectedItemIndex = CbParameters.IndexOf(SelectedComboItemParameters);
                SelectedComboItemParameters = CbParameters[selectedItemIndex];
            }
        }
    }
}
