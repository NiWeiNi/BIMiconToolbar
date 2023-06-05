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

        private ObservableCollection<BaseElement> _views;

        public ObservableCollection<BaseElement> Views
        {
            get { return _views; }
            set { _views = value; }
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
            MatchGridsModel model = new MatchGridsModel(viewModel);
            viewModel.MatchGridsModel = model;
            viewModel.RequestClose += ViewModel_RequestClose;
            DataContext = viewModel;

            InitializeComponent();
        }

        /// <summary>
        /// Set properties once clicked Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
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

        private void viewsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MatchGridsViewModel viewmodel = (MatchGridsViewModel)DataContext;
            viewmodel.SelectedViews = viewsList.SelectedItems
                .Cast<BaseElement>()
                .ToList();
        }

        private void ViewModel_RequestClose(object sender, EventArgs e)
        {
            Close();
        }
    }
}
