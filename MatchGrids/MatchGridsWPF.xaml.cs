using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMiconToolbar.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.MatchGrids
{
    /// <summary>
    /// Interaction logic for MatchGridsWPF.xaml
    /// </summary>
    public partial class MatchGridsWPF : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store ComboBox items
        /// </summary>
        public ObservableCollection<ComboBoxItem> CbItems { get; set; }
        public ComboBoxItem SelectedComboItem { get; set; }
        public IEnumerable<View> FilteredViewsCheckBox { get; set; }
        public List<int> IntegerIds { get; set; }
        public bool CopyDim { get; set; }


        /// <summary>
        /// MatchGrids main window
        /// </summary>
        /// <param name="commandData"></param>
        public MatchGridsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            CbItems = new ObservableCollection<ComboBoxItem>();
            CopyDim = false;

            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
            List<View> filteredV = viewsCollector.Cast<View>()
                                   .Where(sh =>
                                   sh.ViewType == ViewType.AreaPlan ||
                                   sh.ViewType == ViewType.CeilingPlan ||
                                   sh.ViewType == ViewType.Elevation ||
                                   sh.ViewType == ViewType.FloorPlan ||
                                   sh.ViewType == ViewType.Section)
                                   .Where(view => !view.IsTemplate)
                                   .ToList();

            IOrderedEnumerable<View> views = filteredV.OrderBy(v => v.ViewType).ThenBy(v => v.Name);

            FilteredViewsCheckBox = filteredV;

            int count = 0;

            foreach (var v in views)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.ViewType + " - " + v.Name;
                comb.Tag = v;
                CbItems.Add(comb);

                if (count == 0)
                {
                    SelectedComboItem = comb;
                }

                count++;
            }

            // Associate the event-handling method with the SelectedIndexChanged event
            this.comboDisplay.SelectionChanged += new SelectionChangedEventHandler(MyComboChanged);

            ViewCheckBoxes(doc);
        }

        /// <summary>
        /// Make window disposable
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Method to update views according to initial view selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyComboChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbItems.IndexOf(SelectedComboItem);
            View selectedView= (View)CbItems[selectedItemIndex].Tag;
            ViewType selectedViewType = selectedView.ViewType;

            XYZ viewDirection = selectedView.ViewDirection;

            IEnumerable<View> views = FilteredViewsCheckBox
                                    .Where(v => v != selectedView)
                                    .Where(v => HelpersGeometry.AreVectorsParallel(v.ViewDirection, viewDirection));

            UpdateViewCheckBoxes(views);
        }

        /// <summary>
        /// Update views for selection
        /// </summary>
        /// <param name="views"></param>
        private void UpdateViewCheckBoxes(IEnumerable<View> views)
        {
            viewsCheckBox.Children.Clear();

            IOrderedEnumerable<View> orderViews = views.OrderBy(v => v.ViewType).ThenBy(v => v.Name);

            foreach (var v in orderViews)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = v.ViewType + " - " + v.Name,
                    Name = "ID" + v.Id.ToString()
                };
                viewsCheckBox.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Dynamically populate checkboxes
        /// </summary>
        /// <param name="doc"></param>
        private void ViewCheckBoxes(Document doc)
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);

            IOrderedEnumerable<View> views = from View view in viewsCollector orderby view.ViewType orderby view.Name ascending select view;

            foreach (var v in views)
            {
                CheckBox checkBox = new CheckBox
                {
                    Content = v.Name,
                    Name = "ID" + v.Id.ToString()
                };
                viewsCheckBox.Children.Add(checkBox);
            }
        }

        /// <summary>
        /// Set properties once clicked Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve all checked checkboxes
            IEnumerable<CheckBox> list = this.viewsCheckBox.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            IntegerIds = new List<int>();

            // Add all checked checkboxes to global variable
            foreach (var x in list)
            {
                // Retrieve ids of checked sheets
                int intId = Int32.Parse(x.Name.Replace("ID", ""));
                IntegerIds.Add(intId);
            }

            this.Dispose();
        }

        /// <summary>
        /// Function to reset selected views
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            var list = this.viewsCheckBox.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            foreach (var x in list)
            {
                x.IsChecked = false;
            }
        }

        /// <summary>
        /// Function to cancel current operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// Reverse boolean property to copy dimensions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dimYes_Checked(object sender, RoutedEventArgs e)
        {
            this.CopyDim = !CopyDim;
        }
    }
}
