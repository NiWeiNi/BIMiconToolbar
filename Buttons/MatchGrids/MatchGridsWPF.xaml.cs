using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Buttons.MatchGrids;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using View = Autodesk.Revit.DB.View;

namespace BIMicon.BIMiconToolbar.MatchGrids
{
    /// <summary>
    /// Interaction logic for MatchGridsWPF.xaml
    /// </summary>
    public partial class MatchGridsWPF : Window
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
        public BaseElement SelectedViewToCopy { get; set; }
        public List<int> IntegerIds { get; set; }
        public bool CopyDim { get; set; }
        public List<BaseElement> ViewsInProject { get; set; }
        public List<BaseElement> FilteredViewsByComboBox { get; set; }


        /// <summary>
        /// MatchGrids main window
        /// </summary>
        /// <param name="commandData"></param>
        public MatchGridsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            MatchGridsViewModel viewModel = new MatchGridsViewModel(doc);
            DataContext = viewModel;

            InitializeComponent();
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
                                   sh.ViewType == ViewType.Section ||
                                   sh.ViewType == ViewType.EngineeringPlan)
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
        /// Method to update views according to initial view selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MyComboChanged(object sender, SelectionChangedEventArgs e)
        {
            Views.Clear();

            View selectedView = Doc.GetElement(new ElementId(SelectedViewToCopy.Id)) as View;
            XYZ viewDirection = selectedView.ViewDirection;

            FilteredViewsByComboBox = ViewsInProject
                .Where(x => x.Id != SelectedViewToCopy.Id)
                .Where(x => HelpersGeometry.AreVectorsParallel(viewDirection, (Doc.GetElement(new ElementId(x.Id)) as View).ViewDirection))
                .ToList();

            foreach (BaseElement bs in FilteredViewsByComboBox)
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

            this.Close();
        }

        /// <summary>
        /// Function to cancel current operation
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
            var FilteredElements = FilteredViewsByComboBox.Where(x => Parsing.Contains(x.Name, searchTbox.Text, StringComparison.InvariantCultureIgnoreCase));

            // Remove elements not in search term
            for (int i = Views.Count - 1; i >= 0; i--)
            {
                var item = Views[i];
                if (!FilteredElements.Contains(item))
                {
                    Views.Remove(item);
                }
            }

            // Bring back elements when input search text changes
            foreach (var item in FilteredElements)
            {
                if (!Views.Contains(item))
                {
                    Views.Add(item);
                }
            }
        }
    }
}
