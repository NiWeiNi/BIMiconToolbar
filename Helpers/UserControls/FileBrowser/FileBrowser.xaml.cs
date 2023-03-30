using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using UserControl = System.Windows.Controls.UserControl;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.FileBrowser
{
    /// <summary>
    /// Interaction logic for FileBrowser.xaml
    /// </summary>
    public partial class FileBrowser : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public string _filePath;
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = value;
                OnPropertyChanged();
            }
        }

        // Create the OnPropertyChanged method to raise the event
        // The calling member's name will be used as the parameter.
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public FileBrowser()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void File_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = "";
                openFileDialog.Filter = "Revit files (*.rvt)|*.rvt|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    // Assign the path to property
                    FilePath = openFileDialog.FileName;
                }
            }
        }
    }
}
