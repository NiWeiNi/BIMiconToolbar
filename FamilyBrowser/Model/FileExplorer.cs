using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BIMiconToolbar.FamilyBrowser.Model
{
    class FileExplorer
    {
        /// <summary>
        /// Method to retrieve information about files in a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IList<FileInfo> GetFilesFromDirectory(string directory)
        {
               try
               {
                   return (from x in Directory.GetFiles(directory) select new FileInfo(x)).ToList();
               }
               catch (Exception e)
               {
                     // TODO: Log error
               }

               return new List<FileInfo>();
        }
    }
}