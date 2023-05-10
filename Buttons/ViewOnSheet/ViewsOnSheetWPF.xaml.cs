using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private Document _doc;
        public Document Doc
        {
            get { return _doc; }
            set { _doc = value; }
        }

        private ObservableCollection<BaseElement> _sheets;
        public ObservableCollection<BaseElement> Sheets
        {
            get { return _sheets; }
            set { _sheets = value; }
        }

        /// <summary>
        /// Create window for user input
        /// </summary>
        /// <param name="commandData"></param>
        public ViewSheetsWindow(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            DataContext = this;

            LoadSheets();
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
        }

        /// <summary>
        /// Make window disposable
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Store selected sheets for main programs use
        /// </summary>
        public List<int> listIds = new List<int>();

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
            // Add all checked checkboxes to global variable
            foreach (BaseElement x in sheetsList.SelectedItems)
            {
                listIds.Add(x.Id);
            }

            this.Dispose();
        }
    }
}
