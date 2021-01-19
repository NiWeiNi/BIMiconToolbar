using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.DuplicateSheets
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

        /// <summary>
        /// Main function to call window
        /// </summary>
        /// <param name="commandData"></param>
        public DuplicateSheetsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();

            // Populate sheet checkboxes
            SheetCheckboxes(doc);
        }

        /// <summary>
        /// Function to close window
        /// </summary>
        public void Dispose()
        {
            Close();
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

        private void withoutViews_Unchecked(object sender, RoutedEventArgs e)
        {

        }

        private void withoutViews_Checked(object sender, RoutedEventArgs e)
        {

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

            this.Dispose();
        }
    }
}
