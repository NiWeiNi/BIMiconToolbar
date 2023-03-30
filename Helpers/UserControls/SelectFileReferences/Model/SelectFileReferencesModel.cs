using Autodesk.Revit.DB;

namespace BIMicon.BIMiconToolbar.Helpers.UserControls.SelectFileReferences.Model
{
    public class SelectFileReferencesModel
    {
        public ExternalFileReferenceType ReferenceType { get; set; }
        public bool IsSelected { get; set; }
        public string Text
        {
            get
            {
                return ReferenceType.ToString();
            }
        }

        public SelectFileReferencesModel(ExternalFileReferenceType externalFileReferenceType)
        {
            ReferenceType = externalFileReferenceType;
        }
    }
}
