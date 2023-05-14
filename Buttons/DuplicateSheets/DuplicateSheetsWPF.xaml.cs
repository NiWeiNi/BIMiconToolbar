using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

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
        private ObservableCollection<BaseElement> _sheets;
        public ObservableCollection<BaseElement> Sheets
        {
            get { return _sheets; }
            set { _sheets = value; }
        }
        private ObservableCollection<BaseElement> _titleblocks;

        public ObservableCollection<BaseElement> Titleblocks
        {
            get { return _titleblocks; }
            set { _titleblocks = value; }
        }
        private Document _doc;
        public Document Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }
        public BaseElement SelectedTitleblock { get; set; }
        public Boolean copyViews = true;
        public Boolean optDuplicate = true;
        public Boolean optDuplicateDetailing = false;
        public Boolean optDuplicateDependant = false;
        public string viewPrefix { get; set; }
        public string viewSuffix { get; set; }
        public string sheetPrefix { get; set; }
        public string sheetSuffix { get; set; }
        public List<BaseElement> FilteredSheets;

        /// <summary>
        /// Main function to call window
        /// </summary>
        /// <param name="commandData"></param>
        public DuplicateSheetsWPF(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            DataContext = this;

            LoadSheets();
            LoadTitleblocks();
            InitializeComponent();
        }

        private void LoadSheets()
        {
            FilteredElementCollector sheetsCollector = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Sheets);
            List<ViewSheet> sheets = sheetsCollector.Cast<ViewSheet>().ToList();

            Sheets = new ObservableCollection<BaseElement>(sheets
                .OrderBy(x => x.SheetNumber)
                .Select(x => new BaseElement() { Name = x.SheetNumber + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList());

            FilteredSheets = sheets
                .OrderBy(x => x.SheetNumber)
                .Select(x => new BaseElement() { Name = x.SheetNumber + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList();
        }

        /// <summary>
        /// Method to polpulate Floor Types Combo boxes
        /// </summary>
        private void LoadTitleblocks()
        {
            FilteredElementCollector titleBlocksCollector = new FilteredElementCollector(Doc)
                                                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                            .WhereElementIsElementType();
            List<BaseElement> titleblocks = titleBlocksCollector
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList();

            titleblocks.Insert(0, new BaseElement() { Name = "Current Title Block", Id = 0 });

            Titleblocks = new ObservableCollection<BaseElement>(titleblocks);
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
            // Add all checked checkboxes to global variable
            foreach (BaseElement x in sheetsList.SelectedItems)
            {
                sheetIds.Add(x.Id);
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

        private void searchTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var FilteredElements = FilteredSheets.Where(x => Parsing.Contains(x.Name, searchTbox.Text, StringComparison.InvariantCultureIgnoreCase));

            // Remove elements not in search term
            for (int i = Sheets.Count - 1; i >= 0; i--)
            {
                var item = Sheets[i];
                if (!FilteredElements.Contains(item))
                {
                    Sheets.Remove(item);
                }
            }

            // Bring back elements when input search text changes
            foreach (var item in FilteredElements)
            {
                if (!Sheets.Contains(item))
                {
                    Sheets.Add(item);
                }
            }
        }
    }
}
