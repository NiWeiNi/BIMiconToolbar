using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.ExportSchedules
{
    /// <summary>
    /// Interaction logic for BrowserCheckboxes.xaml
    /// </summary>
    public partial class BrowserCheckboxes : Window, IDisposable
    {
        /// <summary>
        /// Store selected sheets for main programs use
        /// </summary>
        public List<int> listIds { get; set; }

        public BrowserCheckboxes(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            ViewCheckBoxes(doc);
        }

        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Dynamically populate checkboxes
        /// </summary>
        /// <param name="doc"></param>
        private void ViewCheckBoxes(Document doc)
        {
            // Collect schedules
            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc)
                                                    .OfCategory(BuiltInCategory.OST_Schedules);

            List<ViewSchedule> schedules = viewsCollector.Cast<ViewSchedule>().Where(sh =>
                                   sh.Name.Contains("<Revision Schedule>") != false).ToList();

            IOrderedEnumerable<ViewSchedule> views = from ViewSchedule view in viewsCollector orderby view.Name ascending select view;

            foreach (var v in views)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = v.Name;
                checkBox.Name = "ID" + v.Id.ToString();
                viewSchedules.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Method to set properties when click OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            listIds = new List<int>();

            // Retrieve all checked checkboxes
            IEnumerable<CheckBox> list = this.viewSchedules.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

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
