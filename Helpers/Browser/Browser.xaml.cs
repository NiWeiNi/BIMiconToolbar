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
        }

        public void Dispose()
        {
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(var drive in Directory.GetLogicalDrives())
            {
                var item = new TreeViewItem() 
                {
                    Header = drive,
                    Tag = drive
                };

                item.Items.Add(null);

                // Listen for item expansion
                item.Expanded += Item_Expanded;

                FolderView.Items.Add(item);
            }
        }

        #region Folder expanded
        private void Item_Expanded(object sender, RoutedEventArgs e)
        {
            var item = (TreeViewItem)sender;

            if (item.Items.Count != 1 || item.Items[0] != null)
            {
                return;
            }

            item.Items.Clear();

            var fullPath = (string)item.Tag;

            var directories = new List<string>();

            try
            {
                var dirs = Directory.GetDirectories(fullPath);
                
                if (dirs.Length > 0)
                {
                    directories.AddRange(dirs);
                }
            }
            catch
            {

            }

            directories.ForEach(directoryPath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(directoryPath),
                    Tag = directoryPath
                };

                subItem.Items.Add(null);

                subItem.Expanded += Item_Expanded;

                item.Items.Add(subItem);
            });

            #region Get files

            var files = new List<string>();

            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                {
                    files.AddRange(fs);
                }
            }
            catch
            {

            }

            files.ForEach(filePath =>
            {
                var subItem = new TreeViewItem()
                {
                    Header = GetFileFolderName(filePath),
                    Tag = filePath
                };

                item.Items.Add(subItem);
            });

            #endregion  
        }
        #endregion
        public static string GetFileFolderName(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return string.Empty;
            }

            var normalizedPath = path.Replace("/", "\\");

            var lastIndex = normalizedPath.LastIndexOf("\\");

            if (lastIndex <= 0)
            {
                return path;
            }

            return path.Substring(lastIndex + 1);
        }
    }
}
