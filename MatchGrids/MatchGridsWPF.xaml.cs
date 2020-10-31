using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BIMiconToolbar.MatchGrids
{
    /// <summary>
    /// Interaction logic for MatchGridsWPF.xaml
    /// </summary>
    public partial class MatchGridsWPF : Window, IDisposable
    {
        /// <summary>
        ///  Properties to store ComboBox items
        /// </summary>
        public ObservableCollection<ComboBoxItem> cbItems { get; set; }
        public ComboBoxItem SelectedComboItem { get; set; }

        /// <summary>
        /// MatchGrids main window
        /// </summary>
        /// <param name="commandData"></param>
        public MatchGridsWPF(ExternalCommandData commandData)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            DataContext = this;

            cbItems = new ObservableCollection<ComboBoxItem>();

            FilteredElementCollector viewsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views);
            IOrderedEnumerable<View> views = from View view in viewsCollector orderby view.Name ascending select view;

            int count = 0;

            foreach (var v in views)
            {
                ComboBoxItem comb = new ComboBoxItem();
                comb.Content = v.Name;
                cbItems.Add(comb);

                if (count == 0)
                {
                    SelectedComboItem = comb;
                }

                count++;
            }
        }

        /// <summary>
        /// Make window disposable
        /// </summary>
        public void Dispose()
        {
            this.Close();
        }

    }
}
