using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Buttons.MatchGrids;
using BIMicon.BIMiconToolbar.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.MatchGrids
{
    /// <summary>
    /// Interaction logic for MatchGridsView.xaml
    /// </summary>
    public partial class MatchGridsView : Window
    {
        public bool CopyDim { get; set; }

        /// <summary>
        /// MatchGrids main window
        /// </summary>
        /// <param name="commandData"></param>
        public MatchGridsView(ExternalCommandData commandData)
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
