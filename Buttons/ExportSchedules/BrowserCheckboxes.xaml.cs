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

namespace BIMicon.BIMiconToolbar.ExportSchedules
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
        public Document Doc { get; set; }
        private ObservableCollection<BaseElement> _viewSchedules;
        public ObservableCollection<BaseElement> ViewSchedules
        {
            get { return _viewSchedules; }
            set { _viewSchedules = value; }
        }
        public List<BaseElement> FilteredViewSchedules;

        public BrowserCheckboxes(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            DataContext = this;

            LoadSchedules();
            InitializeComponent();
        }

        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Dynamically populate checkboxes
        /// </summary>
        private void LoadSchedules()
        {
            // Collect schedules
            FilteredElementCollector viewsCollector = new FilteredElementCollector(Doc)
                                                    .OfCategory(BuiltInCategory.OST_Schedules);

            List<ViewSchedule> schedules = viewsCollector.Cast<ViewSchedule>().Where(sh => !sh.Name.Contains(@"<Revision Schedule>")).ToList();

            ViewSchedules = new ObservableCollection<BaseElement>(schedules
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList());

            FilteredViewSchedules = schedules
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList();
        }

        /// <summary>
        /// Method to set properties when click OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            listIds = new List<int>();

            // Add all checked checkboxes to global variable
            foreach (BaseElement x in viewSchedules.SelectedItems)
            {
                listIds.Add(x.Id);
            }

            this.Dispose();
        }

        private void searchTbox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var FilteredElements = FilteredViewSchedules.Where(x => Parsing.Contains(x.Name, searchTbox.Text, StringComparison.InvariantCultureIgnoreCase));

            // Remove elements not in search term
            for (int i = ViewSchedules.Count - 1; i >= 0; i--)
            {
                var item = ViewSchedules[i];
                if (!FilteredElements.Contains(item))
                {
                    ViewSchedules.Remove(item);
                }
            }

            // Bring back elements when input search text changes
            foreach (var item in FilteredElements)
            {
                if (!ViewSchedules.Contains(item))
                {
                    ViewSchedules.Add(item);
                }
            }
        }
    }
}
