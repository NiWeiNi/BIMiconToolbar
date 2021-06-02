using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.FloorFinish
{
    /// <summary>
    /// Interaction logic for FloorFinishWPF.xaml
    /// </summary>
    public partial class FloorFinishWPF : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store variables
        /// </summary>
        public ObservableCollection<ComboBoxItem> CbItemsFloorTypes { get; set; }
        public ComboBoxItem SelectedComboItemFloorType { get; set; }
        public List<int> IntegerIds { get; set; }
        public double FloorOffset { get; set; }

        /// <summary>
        /// Method to initialize the window and populate content
        /// </summary>
        public FloorFinishWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            // Populate room checkboxes
            RoomsCheckBoxes(doc);
            // Populate floor types
            ComboBoxFloorTypes(doc);

            // Associate the event-handling method with the SelectedIndexChanged event
            this.comboDisplayFloorTypes.SelectionChanged += new SelectionChangedEventHandler(ComboChangedFloorType);
        }

        /// <summary>
        /// Method to polpulate Floor Types Combo boxes
        /// </summary>
        /// <param name="doc"></param>
        private void ComboBoxFloorTypes(Document doc)
        {
            CbItemsFloorTypes= new ObservableCollection<ComboBoxItem>();

            FilteredElementCollector floorTypesCollector = new FilteredElementCollector(doc)
                                                           .OfCategory(BuiltInCategory.OST_Floors)
                                                           .WhereElementIsElementType();

            IOrderedEnumerable<ElementType> floorTypes = from ElementType fT in floorTypesCollector orderby fT.Name ascending select fT;

            int count = 0;

            foreach (var v in floorTypes)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.Name;
                comb.Tag = v;
                CbItemsFloorTypes.Add(comb);

                if (count == 0)
                {
                    SelectedComboItemFloorType = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Method to update flor type according to selected floor type
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboChangedFloorType(object sender, SelectionChangedEventArgs e)
        {
            int selectedItemIndex = CbItemsFloorTypes.IndexOf(SelectedComboItemFloorType);
            SelectedComboItemFloorType = CbItemsFloorTypes[selectedItemIndex];
        }

        /// <summary>
        /// Method to dispose of window
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

            var filter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);

            var roomsCollector = new FilteredElementCollector(doc).WherePasses(filter)
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

        /// <summary>
        /// ethod to accept user input and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            bool isDouble = Double.TryParse(this.offsetTextBox.Text, out double number);

            if (isDouble)
            {
                // Swap , for .
                string newStringNumber = this.offsetTextBox.Text.Replace(",", ".");

                // Assign floor offset to property
                FloorOffset = Double.Parse(newStringNumber);

                // Retrieve all checked checkboxes
                IEnumerable<CheckBox> list = this.roomsCheckBoxes.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

                IntegerIds = new List<int>();

                // Add all checked checkboxes to global variable
                foreach (var x in list)
                {
                    // Retrieve ids of checked rooms
                    int intId = Int32.Parse(x.Name.Replace("ID", ""));
                    IntegerIds.Add(intId);
                }

                this.Dispose();
            }
            else
            {
                Helpers.MessageWindows.AlertMessage("Error", "Please input a number for Offset from level.\n"
                                                             + "Use . or , only to separate decimal part.\n"
                                                             + "For example: 3.20 or 0,50");
            }
        }

        /// <summary>
        /// Method to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }
    }
}
