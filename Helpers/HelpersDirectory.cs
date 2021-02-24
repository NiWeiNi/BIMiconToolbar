using System.IO;
using System.Linq;

namespace BIMiconToolbar.Helpers
{
    class HelpersDirectory
    {
        /// <summary>
        /// Function to move directories
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void MoveDirectory(string[] source, string[] destination)
        {
            // Check source list is not empty
            if (Helpers.IsNullOrEmpty(source) || Helpers.IsNullOrEmpty(destination))
            {
                // TODO: Log error
            }
            // Move all directories in list
            else
            {
                for(int i = 0; i < source.Length; i++)
                {
                    Directory.Move(source[i], destination[i]);
                }
            }
        }

        /// <summary>
        /// Function to rename directories
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void RenameDirectory(string[] source, string oldChar, string newChar)
        {
            if (Helpers.IsNullOrEmpty(source) != true)
            {
                var destination = HelpersString.ReplaceString(source, oldChar, newChar);

                HelpersDirectory.MoveDirectory(source, new string[] { @"C:\Users\BIMicon\Desktop\test\123" });
            }
        }

    }
}
