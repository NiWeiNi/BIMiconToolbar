using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace BIMicon.BIMiconToolbar.Tab
{
    class CommandAvailability : IExternalCommandAvailability
    {
        /// <summary>
        /// Method to check if a document is open so tools can be used
        /// </summary>
        /// <param name="applicationData"></param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            // Allow use of tool even when there is no document opened
            return true;
        }
    }
}
