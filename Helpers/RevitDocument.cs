using Autodesk.Revit.DB;

namespace BIMicon.BIMiconToolbar.Helpers
{
    internal class RevitDocument
    {
        public static bool IsDocumentNotProjectDoc(Document document)
        {
            if (document.IsFamilyDocument)
            {
                string warningMessage = "The active document is a Family document, \n" +
                    "please use the tool in a Project document.";
                MessageWindows.AlertMessage("Warning", warningMessage);
                return true;
            }
            return false;
        }
    }
}
