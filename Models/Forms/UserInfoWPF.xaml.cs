using System.Windows;
using System.Windows.Input;

namespace BIMicon.BIMiconToolbar.Models.Forms
{
    /// <summary>
    /// Interaction logic for ResultWindowWPF.xaml
    /// </summary>
    public partial class UserInfoWPF : Window
    {
        public UserInfoWPF(string title, string message)
        {
            InitializeComponent();
            this.Title = title;
            this.message.Text = message;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void OK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
