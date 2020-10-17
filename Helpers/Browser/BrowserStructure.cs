using BIMiconToolbar.Helpers.Browser.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMiconToolbar.Helpers.Browser
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
                    items.AddRange(dirs.Select(dir => new BrowserItem { FullPath = dir, Type = BrowserItemType.Folder}));
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
                    items.AddRange(fs.Select(file => new BrowserItem { FullPath = file, Type = BrowserItemType.File}));
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
