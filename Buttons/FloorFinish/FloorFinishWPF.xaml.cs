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
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.FloorFinish
{
    /// <summary>
    /// Interaction logic for FloorFinishWPF.xaml
    /// </summary>
    public partial class FloorFinishWPF : Window
    {
        /// <summary>
        ///  Properties to store variables
        /// </summary>
        public ObservableCollection<BaseElement> _floorTypes;
        public ObservableCollection<BaseElement> FloorTypes
        {
            get { return _floorTypes; }
            set { _floorTypes = value; }
        }
        private BaseElement _selectedFloorType;

        public BaseElement SelectedFloorType
        {
            get { return _selectedFloorType; }
            set { _selectedFloorType = value; }
        }
        private Document Doc { get; set; }
        public double FloorOffset { get; set; }
        public BaseElement SelectedTitleblock { get; set; }
        public string StringInternalUnits { get; set; }
        private bool IsExecuteReady = false;
        private ObservableCollection<BaseElement> _rooms;
        public ObservableCollection<BaseElement> Rooms
        {
            get { return _rooms; }
            set { _rooms = value; }
        }

        public List<BaseElement> FilteredRooms;

        /// <summary>
        /// Method to initialize the window and populate content
        /// </summary>
        public FloorFinishWPF(ExternalCommandData commandData)
        {
            Doc = commandData.Application.ActiveUIDocument.Document;

            DataContext = this;
            // Populate floor types and rooms
            LoadFloorTypes();
            LoadRooms();

            InitializeComponent();
            
            // Set the input units
            offsetTextBlock.Text = "Offset from level:" ;
        }

        /// <summary>
        /// Method to polpulate Floor Types Combo boxes
        /// </summary>
        private void LoadFloorTypes()
        {
            FilteredElementCollector floorTypesCollector = new FilteredElementCollector(Doc)
                                                           .OfCategory(BuiltInCategory.OST_Floors)
                                                           .WhereElementIsElementType();

            FloorTypes = new ObservableCollection<BaseElement>(floorTypesCollector
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

            Rooms = new ObservableCollection<BaseElement> ( roomsCollector
                .OrderBy(x => x.Number)
                .Select(x => new BaseElement() { Name = x.Number + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList() );

            FilteredRooms = roomsCollector
                .OrderBy(x => x.Number)
                .Select(x => new BaseElement() { Name = x.Number + " - " + x.Name, Id = x.Id.IntegerValue })
                .ToList();
        }

        /// <summary>
        /// Method to accept user input and close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OK_Click(object sender, RoutedEventArgs e)
        {
            if (IsExecuteReady)
            {
                // Close window
                this.Close();

                // SpatialElement boundary options
                SpatialElementBoundaryOptions sEBOpt = new SpatialElementBoundaryOptions();

                // Group transaction
                TransactionGroup tg = new TransactionGroup(Doc, "Create Floor/s");
                tg.Start();

                // Add all checked checkboxes to global variable
                foreach (BaseElement bE in roomsList.SelectedItems)
                {
                    Element element = Doc.GetElement(new ElementId(bE.Id));
                    SpatialElement sE = element as SpatialElement;

                    // Retrieve level
                    Level level = sE.Level;

                    // Retrieve floor type
                    FloorType selectedFloorType = Doc.GetElement(new ElementId(SelectedFloorType.Id)) as FloorType;

                    // Store the floor created with first boundaries
                    Element floorElement = null;

                    // Retrieve boundaries of rooms
                    IList<IList<BoundarySegment>> boundaries = sE.GetBoundarySegments(sEBOpt);

                    // Retrieve boundaries and create floor and openings
                    for (int i = 0; i < boundaries.Count; i++ )
                    {
                        if (boundaries[i].Count != 0 && i == 0)
                        {
#if v2023 || v2024
                            // Floor boundary
                            List<Curve> curves = new List<Curve>();
                            curves = boundaries[i].Select(xC => xC.GetCurve()).ToList();
                            CurveLoop curveLoop = CurveLoop.Create(curves);
#else
                            // Floor boundary
                            CurveArray floorBoundary = new CurveArray();

                            foreach (var b in boundaries[i])
                            {
                                floorBoundary.Append(b.GetCurve());
                            }
#endif

                            // Create floor
                            Transaction transaction = new Transaction(Doc, "Create Floor");
                            transaction.Start();
                            RevitTransaction.SetWarningDialogSupressor(transaction);

                            Floor floor = null;
#if v2023 || v2024
                            List<CurveLoop> profile = new List<CurveLoop>() { curveLoop };
                            floor = Floor.Create(Doc, profile, selectedFloorType.Id, level.Id);
#else
                            floor = Doc.Create.NewFloor(floorBoundary, selectedFloorType, level, false);
#endif
                            floorElement = floor as Element;

                            transaction.Commit();
                        }
                        // Create openings
                        else
                        {
                            // Opening boundary
                            CurveArray openingBoundary = new CurveArray();

                            foreach (var b in boundaries[i])
                            {
                                openingBoundary.Append(b.GetCurve());
                            }

                            if (floorElement != null)
                            {
                                // Create opening
                                Transaction transaction = new Transaction(Doc, "Create Opening");
                                transaction.Start();

                                Doc.Create.NewOpening(floorElement, openingBoundary, false);

                                transaction.Commit();
                            }
                        }
                    }

                    // Offset the floor
                    if (FloorOffset != 0)
                    {
                        // Move floor
                        Transaction transaction = new Transaction(Doc, "Move floor");
                        transaction.Start();
                        RevitTransaction.SetWarningDialogSupressor(transaction);

                        ElementTransformUtils.MoveElement(Doc, floorElement.Id, new XYZ(0, 0, FloorOffset));

                        transaction.Commit();
                    }
                }
                tg.Assimilate();
            }
            else
            {
                internalTextBox.Text = "Please input valid number";
            }
        }

        /// <summary>
        /// Method to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Method to update internal units display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OffsetTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Swap , for .
            string newStringNumber = this.offsetTextBox.Text.Replace(",", ".");

            // Check string contains only allowed characters
            bool wellFormatted = newStringNumber.All(c => "\"'/0123456789. ".Contains(c));

            // Parse input
            if (wellFormatted)
            {
                // Check if string can be converted to double
                bool isDouble = Double.TryParse(newStringNumber, out double number);

                // Input parsed and converted directly to internal units
                if (isDouble)
                {
                    // Assign floor offset to property
                    FloorOffset = Helpers.UnitsConverter.ConvertProjectLengthToInternal(Doc, number);
                    StringInternalUnits = FloorOffset.ToString();
                    IsExecuteReady = true;
                }
                // Input as imperial fractions
                else if (Helpers.Parsing.IsImperialFraction(newStringNumber))
                {
                    FloorOffset = Helpers.Parsing.ImperialFractionToDecimalFeet(newStringNumber);
                    StringInternalUnits = FloorOffset.ToString();
                    IsExecuteReady = true;
                }
            }
            else
            {
                StringInternalUnits = "Please change format to: 2' 5 3/4\", 1/4' or 3.5";
                IsExecuteReady = false;
            }

            // Set the conversion units textbox, skip when it is not yet initialized
            if (internalTextBox != null)
            {
                internalTextBox.Text = StringInternalUnits;
            }
        }

        private void searchTbox_TextChanged(object sender, TextChangedEventArgs e)
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
