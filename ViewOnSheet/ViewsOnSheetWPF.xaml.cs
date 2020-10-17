using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
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

namespace BIMiconToolbar.ViewOnSheet
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ViewSheetsWindow : Window, IDisposable
    {
        public ViewSheetsWindow(ExternalCommandData commandData)
        {
            Autodesk.Revit.DB.Document doc = commandData.Application.ActiveUIDocument.Document;

            InitializeComponent();
            SheetCheckboxes(doc);
        }

        public void Dispose()
        {
            this.Close();
        }

        private void SheetCheckboxes(Document doc)
        {
            FilteredElementCollector sheetsCollector = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets);

            foreach (var sheet in sheetsCollector)
            {
                CheckBox checkBox = new CheckBox();
                checkBox.Content = sheet.Name;
                checkBox.Name = "ID" + sheet.Id.ToString();
                sheets.Children.Add(checkBox);
            }
        }

        public List<int> listIds = new List<int>();

        private void reset_Click(object sender, RoutedEventArgs e)
        {
            var list = this.sheets.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            foreach (var x in list)
            {
                x.IsChecked = false;
            }
        }

        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            
            IEnumerable<CheckBox> list = this.sheets.Children.OfType<CheckBox>().Where(x => x.IsChecked == true);

            foreach (var x in list)
            {
                int intId = Int32.Parse(x.Name.Replace("ID", ""));

                listIds.Add(intId);
                // MessageBox.Show(sheet.Name);
            }
            
            this.Close();
        }

        public void intIds()
        {
            List<int> intIds = listIds;
        }
    }
}
