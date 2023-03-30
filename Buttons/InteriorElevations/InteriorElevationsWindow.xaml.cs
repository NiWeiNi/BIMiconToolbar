using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.InteriorElevations
{
    /// <summary>
    /// Interaction logic for InteriorElevationsWindow.xaml
    /// </summary>
    public partial class InteriorElevationsWindow : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store variables
        /// </summary>
        public ObservableCollection<ComboBoxItem> CbItemsViewType { get; set; }
        public ComboBoxItem SelectedComboItemViewType { get; set; }
        public ObservableCollection<ComboBoxItem> CbItemsTitleBlock { get; set; }
        public ComboBoxItem SelectedComboItemTitleBlock { get; set; }
        public ObservableCollection<ComboBoxItem> CbItemsViewTemplate { get; set; }
        public ComboBoxItem SelectedComboItemViewTemplate { get; set; }
        public List<int> IntegerIds { get; set; }
        public double SheetDrawingHeight { get; set; }
        public double SheetDrawingWidth { get; set; }

        /// <summary>
        /// Main Window
        /// </summary>
        /// <param name="commandData"></param>
        public InteriorElevationsWindow(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            // ComboBoxes
            ComboBoxTitleBlock(doc);
            ComboBoxViewTemplate(doc);
            ComboBoxViewType(doc);

            // Check boxes
            RoomsCheckBoxes(doc);

            // Associate the event-handling method with the SelectedIndexChanged event
            this.comboDisplayViewType.SelectionChanged += new SelectionChangedEventHandler(ComboChangedViewType);
            this.comboDisplayTitleBlock.SelectionChanged += new SelectionChangedEventHandler(ComboChangedTitleBlock);
            this.comboDisplayViewTemplate.SelectionChanged += new SelectionChangedEventHandler(ComboChangedViewTemplate);
        }

        /// <summary>
        /// Method to polpulate View Type Combo boxes
        /// </summary>
        /// <param name="doc"></param>
        private void ComboBoxViewType(Document doc)
        {
            CbItemsViewType = new ObservableCollection<ComboBoxItem>();

            FilteredElementCollector viewTypesCollector = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType))
                                                                                           .WhereElementIsElementType();

            List<ViewFamilyType> filteredViewTypes = viewTypesCollector.Cast<ViewFamilyType>().Where(sh =>
                                   sh.ViewFamily == ViewFamily.Elevation).ToList();

            IOrderedEnumerable<ViewFamilyType> viewTypes = from ViewFamilyType view in filteredViewTypes orderby view.FamilyName ascending select view;

            int count = 0;

            foreach (var v in viewTypes)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.Name;
                comb.Tag = v;
                CbItemsViewType.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemViewType = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Method to polpulate View Type Combo boxes
        /// </summary>
        /// <param name="doc"></param>
        private void ComboBoxViewTemplate(Document doc)
        {
            CbItemsViewTemplate = new ObservableCollection<ComboBoxItem>();

            FilteredElementCollector viewTempsCollector = new FilteredElementCollector(doc)
                                                             .OfCategory(BuiltInCategory.OST_Views);

            List<View> filteredViewTypes = viewTempsCollector.Cast<View>().Where(sh =>
                                           sh.IsTemplate).ToList();

            IOrderedEnumerable<View> viewTypes = from View view in filteredViewTypes orderby view.Name ascending select view;

            int count = 0;

            foreach (var v in viewTypes)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.Name;
                comb.Tag = v;
                CbItemsViewTemplate.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemViewTemplate = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Method to polpulate Title Block Combo boxes
        /// </summary>
        /// <param name="doc"></param>
        private void ComboBoxTitleBlock(Document doc)
        {
            CbItemsTitleBlock = new ObservableCollection<ComboBoxItem>();

            FilteredElementCollector titleBlocksCollector = new FilteredElementCollector(doc)
                                                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                            .WhereElementIsElementType();

            IOrderedEnumerable<ElementType> titleBlocks = from ElementType tB in titleBlocksCollector orderby tB.Name ascending select tB;

            int count = 0;

            foreach (var v in titleBlocks)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.Name;
                comb.Tag = v;
                CbItemsTitleBlock.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemTitleBlock = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Method to update views according to initial view selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedViewType(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbItemsViewType.IndexOf(SelectedComboItemViewType);
            SelectedComboItemViewType = CbItemsViewType[selectedItemIndex];
        }

        /// <summary>
        /// Method to update views according to initial view selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedTitleBlock(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbItemsTitleBlock.IndexOf(SelectedComboItemTitleBlock);
            SelectedComboItemTitleBlock = CbItemsTitleBlock[selectedItemIndex];
        }

        /// <summary>
        /// Method to update views according to initial view selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedViewTemplate(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbItemsViewTemplate.IndexOf(SelectedComboItemViewTemplate);
            SelectedComboItemViewTemplate = CbItemsViewTemplate[selectedItemIndex];
        }

        /// <summary>
        /// Method when clicked Ok
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            // Check height and width are numbers
            try
            {
                this.SheetDrawingHeight = Double.Parse(this.Height.Text);
                this.SheetDrawingWidth = Double.Parse(this.Width.Text);
            }
            catch
            {
                TaskDialog.Show("Warning", "Please input a number in Height and Width");
            }

            // Retrieve all checked checkboxes
            IEnumerable<CheckBox> list = this.roomsCheckBoxes.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

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
        /// Method when click Reset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reset_Click(object sender, RoutedEventArgs e)
        {
            var list = this.roomsCheckBoxes.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            foreach (var x in list)
            {
                x.IsChecked = false;
            }
        }

        /// <summary>
        /// Method hen clicked Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }

        /// <summary>
        /// Implement Disposable interface
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Dynamically populate checkboxes
        /// </summary>
        /// <param name="doc"></param>
        private void RoomsCheckBoxes(Document doc)
        {
            var roomsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rooms)
                                                                  .Cast<Room>()
                                                                  .Where(r => r.Area > 0);

            IOrderedEnumerable<Room> rooms = from Room room in roomsCollector orderby room.Number ascending select room;

            foreach (var room in rooms)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = room.Number + " - " + room.Name;
                checkBox.Name = "ID" + room.Id.ToString();
                roomsCheckBoxes.Children.Add(checkBox);
            }
        }
    }
}
