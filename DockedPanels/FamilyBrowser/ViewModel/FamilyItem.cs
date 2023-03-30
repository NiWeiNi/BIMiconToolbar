using BIMicon.BIMiconToolbar.FamilyBrowser.Model;
using System.IO;

namespace BIMicon.BIMiconToolbar.FamilyBrowser.ViewModel
{
    class FamilyItem
    {
        public FileInfo FileInformation { get; set; }
        public FamilyItemCategory Type { get; set; }

        public string FamilyCategory { get; set; }

        public string ModDate
        {
            get
            {
                return FileInformation.LastWriteTime.ToString("dd/MM/yy HH:mm:ss");
            }
        }
        public string Name
        {
            get
            {
                return FileInformation.Name;
            }
        }
        public string Size
        {
            get
            {
                return (FileInformation.Length * 0.000001).ToString() + " MB";
            }
        }

        public FamilyItem (FileInfo fileInformation)
        {
            FileInformation = fileInformation;
        }

    }
}
