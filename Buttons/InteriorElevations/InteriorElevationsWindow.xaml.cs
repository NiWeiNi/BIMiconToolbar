using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using BIMicon.BIMiconToolbar.Helpers;
using BIMicon.BIMiconToolbar.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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
        private Document Doc;
        private ObservableCollection<BaseElement> _rooms;
        public ObservableCollection<BaseElement> Rooms
        {
            get { return _rooms; }
            set { _rooms = value; }
        }
        private ObservableCollection<BaseElement> _titleblocks;

        public ObservableCollection<BaseElement> Titleblocks
        {
            get { return _titleblocks; }
            set { _titleblocks = value; }
        }
        private ObservableCollection<BaseElement> _viewTemplates;

        public ObservableCollection<BaseElement> ViewTemplates
        {
            get { return _viewTemplates; }
            set { _viewTemplates = value; }
        }

        private ObservableCollection<BaseElement> _viewTypes;

        public ObservableCollection<BaseElement> ViewTypes
        {
            get { return _viewTypes; }
            set { _viewTypes = value; }
        }

        public BaseElement SelectedViewType { get; set; }
        public BaseElement SelectedTitleblock { get; set; }
        public BaseElement SelectedViewTemplate { get; set; }
        public List<int> IntegerIds { get; set; }
        public double SheetDrawingHeight { get; set; }
        public double SheetDrawingWidth { get; set; }
        public List<BaseElement> FilteredRooms;

        /// <summary>
        /// Main Window
        /// </summary>
        /// <param name="commandData"></param>
        public InteriorElevationsWindow(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;
            DataContext = this;

            LoadRooms();
            LoadTitleblocks();
            LoadViewTemplates();
            LoadViewTypes();

            InitializeComponent();
        }

        private void LoadViewTemplates()
        {
            FilteredElementCollector viewTempsCollector = new FilteredElementCollector(Doc)
                                                 .OfCategory(BuiltInCategory.OST_Views);

            List<View> filteredViewTypes = viewTempsCollector.Cast<View>().Where(sh =>
                                           sh.IsTemplate).ToList();

            ViewTemplates = new ObservableCollection<BaseElement>(filteredViewTypes
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList());
        }

        private void LoadViewTypes()
        {
            FilteredElementCollector viewTypesCollector = new FilteredElementCollector(Doc).OfClass(typeof(ViewFamilyType))
                                                                               .WhereElementIsElementType();

            List<ViewFamilyType> filteredViewTypes = viewTypesCollector.Cast<ViewFamilyType>().Where(sh =>
                                   sh.ViewFamily == ViewFamily.Elevation).ToList();

            ViewTypes = new ObservableCollection<BaseElement>(filteredViewTypes
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList());
        }

        /// <summary>
        /// Method to load rooms
        /// </summary>
        private void LoadRooms()
        {
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Rooms);
            var roomsCollector = new FilteredElementCollector(Doc)
                .WherePasses(filter)
                .Cast<Room>()
                .Where(r => r.Area > 0)
                .ToList();

            Rooms = new ObservableCollection<BaseElement>(roomsCollector
                .OrderBy(x => x.Number)
                .Select(x => new BaseElement() { Name = x.Number + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList());

            FilteredRooms = roomsCollector
                .OrderBy(x => x.Number)
                .Select(x => new BaseElement() { Name = x.Number + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList();
        }

        /// <summary>
        /// Method to polpulate Floor Types Combo boxes
        /// </summary>
        private void LoadTitleblocks()
        {
            FilteredElementCollector titleBlocksCollector = new FilteredElementCollector(Doc)
                                                            .OfCategory(BuiltInCategory.OST_TitleBlocks)
                                                            .WhereElementIsElementType();

            Titleblocks = new ObservableCollection<BaseElement>(titleBlocksCollector
                .OrderBy(x => x.Name)
                .Select(x => new BaseElement() { Name = x.Name, Id = x.Id.IntegerValue })
                .ToList());
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

            IntegerIds = new List<int>();
            // Add all checked checkboxes to global variable
            foreach (BaseElement x in roomsList.SelectedItems)
            {
                IntegerIds.Add(x.Id);
            }

            this.Dispose();
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

        private void searchTbox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var FilteredElements = FilteredRooms.Where(x => Parsing.Contains(x.Name, searchTbox.Text, StringComparison.InvariantCultureIgnoreCase));

            // Remove elements not in search term
            for (int i = Rooms.Count - 1; i >= 0; i--)
            {
                var item = Rooms[i];
                if (!FilteredElements.Contains(item))
                {
                    Rooms.Remove(item);
                }
            }

            // Bring back elements when input search text changes
            foreach (var item in FilteredElements)
            {
                if (!Rooms.Contains(item))
                {
                    Rooms.Add(item);
                }
            }
        }
    }
}
