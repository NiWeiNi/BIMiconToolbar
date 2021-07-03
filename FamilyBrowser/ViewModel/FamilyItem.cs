using BIMiconToolbar.FamilyBrowser.Model;
using System.IO;

namespace BIMiconToolbar.FamilyBrowser.ViewModel
{
    class FamilyItem
    {
        public FileInfo FileInformation { get; set; }
        public FamilyItemCategory Type { get; set; }

        public string FamilyCategory { get; set; }

        public string FamilyName
        {
            get
            {
                return FileInformation.Name;
            }
        }

        public FamilyItem (FileInfo fileInformation)
        {
            FileInformation = fileInformation;
        }

    }
}
