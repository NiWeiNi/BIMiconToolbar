using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace BIMiconToolbar.Helpers
{
    class RevitDirectories
    {
        /// <summary>
        /// Method to retrieve local path of content library
        /// </summary>
        /// <param name="app"></param>
        /// <returns></returns>
        public static string RevitContentPath(Application app)
        {
            IDictionary<string, string> paths = app.GetLibraryPaths();

            foreach (var p in paths)
            {
                if (p.Key == "Library")
                {
                    return p.Value;
                }
            }

            return null;
        }

        public static void RevitLocalDoctPath()
        {

        }

        /// <summary>
        /// Method to unload links the next time a Revit project is opened
        /// </summary>
        /// <param name="location"></param>
        public static void UnloadRevitLinks(ModelPath location)
        {
            // Get transmission data from selected Revit file
            TransmissionData transData = TransmissionData.ReadTransmissionData(location);

            if (transData != null)
            {
                // Retrieve all first level links
                ICollection<ElementId> externalReferences = transData.GetAllExternalFileReferenceIds();

                // Retrieve all links
                foreach (ElementId refId in externalReferences)
                {
                    ExternalFileReference extRef = transData.GetLastSavedReferenceData(refId);

                    if (extRef.ExternalFileReferenceType == ExternalFileReferenceType.RevitLink)
                    {
                        // Unload links
                        transData.SetDesiredReferenceData(refId, extRef.GetPath(), extRef.PathType, false);
                    }
                }

                // Set the IsTransmitted property
                transData.IsTransmitted = true;

                // Set modified transmission data back to the model
                TransmissionData.WriteTransmissionData(location, transData);
            }
            else
            {
                Autodesk.Revit.UI.TaskDialog.Show("Unload Links", "The document does not have any transmission data");
            }
        }
    }
}
