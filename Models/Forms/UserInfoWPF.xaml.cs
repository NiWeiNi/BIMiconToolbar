using System.Windows;
using System.Windows.Input;

namespace BIMicon.BIMiconToolbar.Models.Forms
{
    /// <summary>
    /// Interaction logic for ResultWindowWPF.xaml
    /// </summary>
    public partial class UserInfoWPF : Window
    {
        public UserInfoWPF(Message message)
        {
            InitializeComponent();
            this.Title = message.Type.ToString();
            this.messageTitle.Text = message.Title;
            this.message.Text = message.Content;
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
