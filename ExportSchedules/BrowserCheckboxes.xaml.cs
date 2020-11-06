using Autodesk.Revit.UI;
using System;
using System.Windows;

namespace BIMiconToolbar.ExportSchedules
{
    /// <summary>
    /// Interaction logic for BrowserCheckboxes.xaml
    /// </summary>
    public partial class BrowserCheckboxes : Window, IDisposable
    {
        public BrowserCheckboxes(ExternalCommandData commandData)
        {
            InitializeComponent();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
