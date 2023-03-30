using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.ViewOnSheet
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ViewSheetsWindow : Window, IDisposable
    {
        /// <summary>
        /// Create window for user input
        /// </summary>
        /// <param name="commandData"></param>
        public ViewSheetsWindow(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            SheetCheckboxes(doc);
        }

        /// <summary>
        /// Make window disposable
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Dynamically populate checkboxes
        /// </summary>
        /// <param name="doc"></param>
        private void SheetCheckboxes(Document doc)
        {
            FilteredElementCollector sheetsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets);

            IOrderedEnumerable<ViewSheet> vSheets =  from ViewSheet vSheet in sheetsCollector orderby vSheet.SheetNumber ascending select vSheet;

            foreach (var sheet in vSheets)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = sheet.SheetNumber + " - " + sheet.Name;
                checkBox.Name = "ID" + sheet.Id.ToString();
                sheets.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Store selected sheets for main programs use
        /// </summary>
        public List<int> listIds = new List<int>();

        /// <summary>
        /// Function to reset selected elements
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            var list = this.sheets.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            foreach (var x in list)
            {
                x.IsChecked = false;
            }
        }

        /// <summary>
        /// Cancel button function to exit the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
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
                listIds.Add(intId);
            }
            
            this.Dispose();
        }
    }
}
