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
        public ObservableCollection<ComboBoxItem> CbItemsFloorType { get; set; }
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

        /// <summary>
        /// Method to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dispose();
        }
    }
}
