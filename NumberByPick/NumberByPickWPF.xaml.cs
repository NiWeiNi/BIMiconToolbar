using System;
using System.Windows;

namespace BIMiconToolbar.NumberByPick
{
    /// <summary>
    /// Interaction logic for NumberByPickWPF.xaml
    /// </summary>
    public partial class NumberByPickWPF : Window, IDisposable
    {
        public NumberByPickWPF()
        {
            InitializeComponent();

            this.DataContext = new NumberByPickViewModel();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}
