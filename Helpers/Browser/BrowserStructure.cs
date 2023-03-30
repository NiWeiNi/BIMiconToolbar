using BIMicon.BIMiconToolbar.Helpers.Browser.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMicon.BIMiconToolbar.Helpers.Browser
{
    /// <summary>
    /// A helper class to query information about directories
    /// </summary>
    public static class BrowserStructure
    {
        /// <summary>
        /// Gets all logical drives on the computer
        /// </summary>
        /// <returns></returns>
        public static List<BrowserItem> GetLogicalDrives()
        {
            /// Get logical drives in the machine
            return Directory.GetLogicalDrives().Select(drive => new BrowserItem { FullPath = drive, Type = BrowserItemType.Drive }).ToList();
        }

        /// <summary>
        /// Get the directories top-level content
        /// </summary>
        /// <param name="fullPath">The full path to the directory</param>
        /// <returns></returns>
        public static List<BrowserItem> GetDirectoryContents(string fullPath)
        {
            var items = new List<BrowserItem>();

            #region Get folders

            try
            {
                var dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                {
                    foreach (var d in dirs)
                    {
                        DirectoryInfo di = new DirectoryInfo(d);

                        // Filter hidden directories
                        if (di.Attributes.HasFlag(FileAttributes.Hidden) == false)
                        {
                            items.Add(new BrowserItem { FullPath = d, Type = BrowserItemType.Folder });
                        }
                    }
                }
            }
            catch
            {

            }

            #endregion

            #region Get files

            try
            {
                var fs = Directory.GetFiles(fullPath);

                if (fs.Length > 0)
                {
                    foreach (var f in fs)
                    {
                        DirectoryInfo di = new DirectoryInfo(f);

                        // Filter out hidden directories
                        if (di.Attributes.HasFlag(FileAttributes.Hidden) == false)
                        {
                            BrowserItem bi = new BrowserItem { FullPath = f };

                            // Check type to add specific icon
                            if (f.EndsWith(".dwg"))
                                bi.Type = BrowserItemType.Dwg;
                            else if (f.ToLower().EndsWith(".pdf"))
                                bi.Type = BrowserItemType.Pdf;
                            else if (f.ToLower().EndsWith(".jpg") || f.ToLower().EndsWith(".png") || 
                                f.ToLower().EndsWith(".jpeg") || f.ToLower().EndsWith(".tiff"))
                                bi.Type = BrowserItemType.Image;
                            else if (f.ToLower().EndsWith(".txt"))
                                bi.Type = BrowserItemType.Txt;
                            else if (f.ToLower().EndsWith(".xls") || f.ToLower().EndsWith(".xlsx"))
                                bi.Type = BrowserItemType.Xls;
                            else if (f.ToLower().EndsWith(".rvt") || f.ToLower().EndsWith(".rfa"))
                                bi.Type = BrowserItemType.Revit;
                            else
                                bi.Type = BrowserItemType.File;

                            // Add file to dispaly list
                            items.Add(bi);
                        }
                    }
                }
            }
            catch
            {

            }

            #endregion

            return items;
        }

        /// <summary>
        /// A helper class to query info about directories
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
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
