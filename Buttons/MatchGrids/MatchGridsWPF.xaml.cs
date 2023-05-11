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

namespace BIMicon.BIMiconToolbar.MatchGrids
{
    /// <summary>
    /// Interaction logic for MatchGridsWPF.xaml
    /// </summary>
    public partial class MatchGridsWPF : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store ComboBox items
        /// </summary>

        private Document Doc;
        private ObservableCollection<BaseElement> _views;

        public ObservableCollection<BaseElement> Views
        {
            get { return _views; }
            set { _views = value; }
        }

        private ObservableCollection<BaseElement> _viewToCopy;

        public ObservableCollection<BaseElement> ViewToCopy
        {
            get { return _viewToCopy; }
            set { _viewToCopy = value; }
        }

        public ObservableCollection<ComboBoxItem> CbItems { get; set; }
        public BaseElement SelectedViewToCopy { get; set; }
        public IEnumerable<View> FilteredViewsCheckBox { get; set; }
        public List<int> IntegerIds { get; set; }
        public bool CopyDim { get; set; }


        /// <summary>
        /// MatchGrids main window
        /// </summary>
        /// <param name="commandData"></param>
        public MatchGridsWPF(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            DataContext = this;

            LoadViews();
            InitializeComponent();

            CbItems = new ObservableCollection<ComboBoxItem>();
            CopyDim = false;
        }

        private void LoadViews()
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Views);
            List<View> filteredV = viewsCollector.Cast<View>()
                                   .Where(sh =>
                                   sh.ViewType == ViewType.AreaPlan ||
                                   sh.ViewType == ViewType.CeilingPlan ||
                                   sh.ViewType == ViewType.Elevation ||
                                   sh.ViewType == ViewType.EngineeringPlan ||
                                   sh.ViewType == ViewType.FloorPlan ||
                                   sh.ViewType == ViewType.Section)
                                   .Where(view => !view.IsTemplate)
                                   .ToList();

            ViewToCopy = new ObservableCollection<BaseElement>(filteredV
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));

            Views = new ObservableCollection<BaseElement>(filteredV
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));
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
            //int selectedItemIndex = CbItems.IndexOf(SelectedComboItem);
            View selectedView= (View)CbItems[0].Tag;
            ViewType selectedViewType = selectedView.ViewType;

            XYZ viewDirection = selectedView.ViewDirection;

            IEnumerable<View> views = FilteredViewsCheckBox
                                    .Where(v => v != selectedView)
                                    .Where(v => HelpersGeometry.AreVectorsParallel(v.ViewDirection, viewDirection));

            //UpdateViewCheckBoxes(views);
        }

        /// <summary>
        /// Set properties once clicked Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Retrieve all checked checkboxes
            IEnumerable<CheckBox> list = null;//this.viewsCheckBox.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

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
