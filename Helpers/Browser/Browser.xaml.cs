using BIMiconToolbar.Helpers.Browser.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace BIMiconToolbar.Helpers.Browser
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class BrowserWindow : Window, IDisposable 
    {
        public BrowserWindow()
        {
            InitializeComponent();

            this.DataContext = new BrowserStructureViewModel();
        }

        public void Dispose()
        {
            this.Close();
        }

        #region Folder expanded
        private void Item_Expanded(object sender, RoutedEventArgs e)
        {
            
        }
        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
