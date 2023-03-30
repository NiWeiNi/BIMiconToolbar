using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BIMicon.BIMiconToolbar.FamilyBrowser.ViewModel
{
    class FileExplorer
    {
        /// <summary>
        /// Method to retrieve information about files in a directory
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static IList<FamilyItem> GetFamiliesFromDirectory(string directory)
        {
            try
            {
                Regex backupPattern = new Regex(@"\.[0-9]{3,4}\.r(vt|fa)");

                List<FileInfo> fileList = Directory.GetFiles(directory)
                                                   .Where(x => backupPattern.Match(x).Success == false)
                                                   .Where(x => x.EndsWith(".rfa"))
                                                   .Select(x => new FileInfo(x))
                                                   .ToList();

                List<FamilyItem> familyList = (from file in Directory.GetFiles(directory)
                                             where file.EndsWith(".rfa")
                                             where backupPattern.Match(file).Success == false
                                             select new FamilyItem(new FileInfo(file)))
                                            .ToList();

                return familyList;
            }
            catch
            {
                // TODO: Log error
            }

            return new List<FamilyItem>();
        }
    }
}
