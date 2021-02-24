using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        /// <summary>
        /// Retrieve all files in folder
        /// </summary>
        /// <param name="selectedDirectory"></param>
        public static string[] RetrieveFiles(string selectedDirectory)
        {
            try
            {
                var files = Directory.GetFiles(selectedDirectory);
                return files;
            }

            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                // TODO: log error

            }

            catch (DirectoryNotFoundException e)
            {
                Console.WriteLine(e.Message);
                // TODO: log error
            }

            return new string[0];
        }

        /// <summary>
        /// Function to match and extract extension
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFilePathExtension(string filePath)
        {
            var extension = "";

            Regex backupPattern = new Regex(@"\.\w{2,4}$");
            Match fileMatch = backupPattern.Match(filePath);

            if (fileMatch.Success)
            {
                extension = fileMatch.Value;
            }

            return extension;
        }

        /// <summary>
        /// Function to retrieve file extension of files
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public static string[] GetFilesType(string[] files)
        {
            var fileTypes = new List<string>();

            if (Helpers.IsNullOrEmpty(files) != true)
            {
                foreach (string f in files)
                {
                    string match = GetFilePathExtension(f);
                    if (match != "")
                    {
                        fileTypes.Add(match);
                    }
                }
            }

            return fileTypes.Distinct().ToArray();
        }
    }
}
