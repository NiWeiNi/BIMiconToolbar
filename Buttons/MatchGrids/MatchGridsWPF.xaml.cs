using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

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
        public List<BaseElement> ViewsInProject { get; set; }


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

            CopyDim = false;
        }

        private void LoadViews()
        {
            FilteredElementCollector viewsCollector = new FilteredElementCollector(Doc).OfCategory(BuiltInCategory.OST_Views);
            List<View> viewsInProject = viewsCollector.Cast<View>()
                                   .Where(sh =>
                                   sh.ViewType == ViewType.AreaPlan ||
                                   sh.ViewType == ViewType.CeilingPlan ||
                                   sh.ViewType == ViewType.Elevation ||
                                   sh.ViewType == ViewType.EngineeringPlan ||
                                   sh.ViewType == ViewType.FloorPlan ||
                                   sh.ViewType == ViewType.Section)
                                   .Where(view => !view.IsTemplate)
                                   .ToList();

            ViewToCopy = new ObservableCollection<BaseElement>(viewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));

            Views = new ObservableCollection<BaseElement>(viewsInProject
                .Select(v => new BaseElement() { Name = v.ViewType.ToString() + " - " + v.Name, Id = v.Id.IntegerValue })
                .OrderBy(v => v.Name));

            ViewsInProject = new List<BaseElement>(viewsInProject
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
            Views.Clear();

            View selectedView = Doc.GetElement(new ElementId(SelectedViewToCopy.Id)) as View;
            XYZ viewDirection = selectedView.ViewDirection;

            List<BaseElement> selViews = ViewsInProject
                .Where(x => x.Id != SelectedViewToCopy.Id)
                .Where(x => HelpersGeometry.AreVectorsParallel(viewDirection, (Doc.GetElement(new ElementId(x.Id)) as View).ViewDirection))
                .ToList();

            foreach (BaseElement bs in selViews)
            {
                Views.Add(bs);
            }
        }

        /// <summary>
        /// Set properties once clicked Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {

            IntegerIds = new List<int>();

            // Add all checked checkboxes to global variable
            foreach (BaseElement x in viewsList.SelectedItems)
            {
                IntegerIds.Add(x.Id);
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

        private void searchTbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
