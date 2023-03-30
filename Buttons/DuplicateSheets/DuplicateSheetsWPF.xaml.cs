using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.DuplicateSheets
{
    /// <summary>
    /// Interaction logic for DuplicateSheetsWPF.xaml
    /// </summary>
    public partial class DuplicateSheetsWPF : Window, IDisposable
    {
        /// <summary>
        /// Store selected sheets for main programs use
        /// </summary>
        public List<int> sheetIds = new List<int>();
        public ObservableCollection<ComboBoxItem> CbTitleBlocks { get; set; }
        public ComboBoxItem SelectedComboItemTitleBlock { get; set; }
        public Boolean copyViews = true;
        public Boolean optDuplicate = true;
        public Boolean optDuplicateDetailing = false;
        public Boolean optDuplicateDependant = false;
        public string viewPrefix { get; set; }
        public string viewSuffix { get; set; }
        public string sheetPrefix { get; set; }
        public string sheetSuffix { get; set; }

        /// <summary>
        /// Main function to call window
        /// </summary>
        /// <param name="commandData"></param>
        public DuplicateSheetsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            // Populate sheet checkboxes
            SheetCheckboxes(doc);
            PopulateTitleBlocks(doc);

            // Associate the event-handling method with the SelectedIndexChanged event
            this.comboDisplayTitleBlock.SelectionChanged += new SelectionChangedEventHandler(ComboChangedTitleBlock);
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        public void Dispose()
        {
            Close();
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
        /// Dynamically populate checkboxes
        /// </summary>
        /// <param name="doc"></param>
        private void SheetCheckboxes(Document doc)
        {
            FilteredElementCollector sheetsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets);

            IOrderedEnumerable<ViewSheet> vSheets = from ViewSheet vSheet in sheetsCollector orderby vSheet.SheetNumber ascending select vSheet;

            foreach (var sheet in vSheets)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = sheet.SheetNumber + " - " + sheet.Name;
                checkBox.Name = "ID" + sheet.Id.ToString();
                sheets.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Function to craete comboBox with title blocks
        /// </summary>
        /// <param name="doc"></param>
        private void PopulateTitleBlocks(Document doc)
        {
            CbTitleBlocks = new ObservableCollection<ComboBoxItem>();

            // Select TitleBlocks
            PhaseArray aPhase = doc.Phases;
            FilteredElementCollector titleBlocksCollector = new FilteredElementCollector(doc)
                                                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                            .WhereElementIsElementType();

            IOrderedEnumerable<FamilySymbol> titleBlocks = titleBlocksCollector.Cast<FamilySymbol>().OrderBy(tb => tb.Family.Name);

            int count = 0;

            foreach (var tb in titleBlocks)
            {
                if (count == 0)
                {
                    ComboBoxItem combo = new ComboBoxItem();
                    combo.Content = "Current Title Block";
                    CbTitleBlocks.Add(combo);
                    SelectedComboItemTitleBlock = combo;
                }

                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = tb.Family.Name + " - " + tb.Name;
                comb.Tag = tb;
                CbTitleBlocks.Add(comb);

                count++;
            }
        }

        /// <summary>
        /// Function to update title block
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedTitleBlock(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbTitleBlocks.IndexOf(SelectedComboItemTitleBlock);
            SelectedComboItemTitleBlock = CbTitleBlocks[selectedItemIndex];
        }

        /// <summary>
        /// Function to flag copy views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void withoutViews_Checked(object sender, RoutedEventArgs e)
        {
            copyViews = !copyViews;
            duplicate.IsEnabled = !duplicate.IsEnabled;
            duplicateDetail.IsEnabled = !duplicateDetail.IsEnabled;
            duplicateDependant.IsEnabled = !duplicateDependant.IsEnabled;
        }

        /// <summary>
        /// Function to flag duplicate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicate_Checked(object sender, RoutedEventArgs e)
        {
            optDuplicate = true;
            optDuplicateDetailing = false;
            optDuplicateDependant = false;
        }

        /// <summary>
        /// Function to flag duplicate with detail
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateDetail_Checked(object sender, RoutedEventArgs e)
        {
            optDuplicate = false;
            optDuplicateDetailing = true;
            optDuplicateDependant = false;
        }

        /// <summary>
        /// Function to flag duplicate as dependant
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void duplicateDependant_Checked(object sender, RoutedEventArgs e)
        {
            optDuplicate = false;
            optDuplicateDetailing = false;
            optDuplicateDependant = true;
        }

        /// <summary>
        /// Function to execute when click OK button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve all checked checkboxes
            IEnumerable<CheckBox> list = this.sheets.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            // Add all checked checkboxes to global variable
            foreach (var x in list)
            {
                // Retrieve ids of checked sheets
                int intId = Int32.Parse(x.Name.Replace("ID", ""));
                sheetIds.Add(intId);
            }

            // Retrieve user's input for prefixes and suffixes
            viewPrefix = viewPrefixTextBox.Text;
            viewSuffix = viewSuffixTextBox.Text;
            sheetPrefix = sheetPrefixTextBox.Text;
            sheetSuffix = sheetSuffixTextBox.Text;

            this.Dispose();
        }

        /// <summary>
        /// Function to close this window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Dispose();
        }
    }
}
