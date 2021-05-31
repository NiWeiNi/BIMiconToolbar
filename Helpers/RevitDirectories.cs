using Autodesk.Revit.ApplicationServices;
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
    }
}
