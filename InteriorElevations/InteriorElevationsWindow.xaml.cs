using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.InteriorElevations
{
    /// <summary>
    /// Interaction logic for InteriorElevationsWindow.xaml
    /// </summary>
    public partial class InteriorElevationsWindow : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store ComboBox items
        /// </summary>
        public List<int> IntegerIds { get; set; }

        /// <summary>
        /// Main Window
        /// </summary>
        /// <param name="commandData"></param>
        public InteriorElevationsWindow(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            RoomsCheckBoxes(doc);
        }

        /// <summary>
        /// Method when clicked Ok
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
