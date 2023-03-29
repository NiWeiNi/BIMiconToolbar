using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace BIMicon.BIMiconToolbar.Helpers
{
    class HelpersDirectory
    {
        /// <summary>
        /// Function to move directories
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void MoveDirectories(string[] source, string[] destination)
        {
            // Check source list is not empty
            if (GeneralHelpers.IsNullOrEmpty(source) || GeneralHelpers.IsNullOrEmpty(destination))
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
        /// Function to move files
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public static void RenameFiles(string[] source, string[] destination)
        {
            // Check source list is not empty
            if (GeneralHelpers.IsNullOrEmpty(source) || GeneralHelpers.IsNullOrEmpty(destination))
            {
                // TODO: Log error
            }
            else
            {
                // Check the numebr of files from origin match number in destination
                if (source.Length == destination.Length)
                {
                    // Move all files in array
                    for (int i = 0; i < source.Length; i++)
                    {
                        File.Move(source[i], destination[i]);
                    }
                }
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
        /// Retrieve subdirectories in directory
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static string[] GetDirectoriesFromPath(string directoryPath)
        {
            try
            {
                var subdirectories = Directory.GetDirectories(directoryPath);
                return subdirectories;
            }

            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                // TODO: log error
            }

            return new string[0];  
        }

        /// <summary>
        /// Function to retrieve files that match a pattern
        /// </summary>
        /// <param name="path"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static string[] GetFilesMatchPattern(string path, string pattern)
        {
            try
            {
                var files = Directory.GetFiles(path, pattern);
                return files;
            }

            catch (UnauthorizedAccessException e)
            {
                Console.WriteLine(e.Message);
                // TODO: log error
            }

            return new string[0];
        }

        /// <summary>
        /// Function to update file names
        /// </summary>
        /// <param name="oldnames"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string[] CreateNewNames(string[] oldnames,
                                              string prefix,
                                              string suffix,
                                              string find,
                                              string replace,
                                              bool folder,
                                              bool toTitleCase)
        {
            var newNames = new List<string>();

            // Strip replace string of forbidden characters
            string goodReplace = Parsing.RemoveForbiddenChars(replace);

            // Create new folder names
            if (folder)
            {
                foreach(string oName in oldnames)
                {
                    string fileName = UpdateFileName(oName, prefix, suffix, find, goodReplace, toTitleCase);

                    newNames.Add(fileName);
                }
            }
            // Create new file names
            else
            {
                foreach (string oName in oldnames)
                {
                    string fileName = UpdateDirectoryName(oName, prefix, suffix, find, goodReplace, toTitleCase);

                    newNames.Add(fileName);
                }
            }

            return newNames.ToArray();
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

            if (GeneralHelpers.IsNullOrEmpty(files) != true)
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

        /// <summary>
        /// Retrieve file name from path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileFromFilePath(string filePath)
        {
            var fileName = "";

            if (filePath != null && filePath != "")
            {
                Regex filePattern = new Regex(@"\\[^/\\:*?<>\|""]+\.\w{2,4}$");
                Match fileMatch = filePattern.Match(filePath);

                if (fileMatch.Success)
                {
                    fileName = fileMatch.Value.Remove(0, 1);
                }
            }

            return fileName;
        }

        /// <summary>
        /// Retrieve folder from path
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static string GetFirstFolderFromPath(string folderPath)
        {
            string folder = "";

            string[] folders = GetDirectoriesFromPath(folderPath);

            if (GeneralHelpers.IsNullOrEmpty(folders) != true)
            {
                // Retrieve name of folder and remove / before name
                folder = folders[0].Replace(folderPath, "").Remove(0, 1);
            }

            return folder;
        }

        /// <summary>
        /// Function to return first file name from file path
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFirstFileFromFilePath(string filePath)
        {
            string fileName = "";
            
            if (filePath != null && filePath != "")
            {
                string[] filePaths = RetrieveFiles(filePath);
                
                if (filePaths.Length != 0)
                {
                    string firstFilePath = RetrieveFiles(filePath)[0];
                    fileName = GetFileFromFilePath(firstFilePath);
                }
            }

            return fileName;
        }

        /// <summary>
        /// Function to update file name
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        /// <returns></returns>
        public static string UpdateFileName(string filePath, string prefix, string suffix, string find, string replace, bool ToTitleCase)
        {
            string updatedName = "";

            if (filePath != null || filePath != "")
            {
                string originalName = GetFileFromFilePath(filePath);

                if (originalName != "")
                {
                    string pathToFile = filePath.Replace(originalName, "");
                    string extension = GetFilePathExtension(filePath);
                    string nameWithoutExt = originalName.Replace(extension, "");
                    // Replace text in path
                    if (find != null && find != "" && replace != null)
                    {
                        nameWithoutExt = nameWithoutExt.Replace(find, replace);
                    }
                    if (ToTitleCase)
                    {
                        string placeholder = "";
                        Parsing.StringToTitleCase(nameWithoutExt, ref placeholder);
                        nameWithoutExt = placeholder;
                    }
                    updatedName = pathToFile + prefix + nameWithoutExt + suffix + extension;
                }
            }

            return updatedName;
        }

        /// <summary>
        /// Function to update folder name
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <param name="prefix"></param>
        /// <param name="originalName"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string UpdateDirectoryName(string directoryPath, string prefix, string suffix, string find, string replace, bool ToTitleCase)
        {
            string updatedName = "";

            if (directoryPath != null || directoryPath != "")
            {
                string directoryName = GetDirectoryNameFromPath(directoryPath);

                if (directoryName != "")
                {
                    string pathToFolder = directoryPath.Replace(directoryName, "");
                    // Replace text in path
                    if (find != null && find != "" && replace != null)
                    {
                        directoryName = directoryName.Replace(find, replace);
                    }
                    if (ToTitleCase)
                    {
                        string placeholder = "";
                        Parsing.StringToTitleCase(directoryName, ref placeholder);
                        directoryName = placeholder;
                    }
                    updatedName = pathToFolder + prefix + directoryName + suffix;
                }
            }

            return updatedName;
        }

        /// <summary>
        /// Function to retrieve folder name from path
        /// </summary>
        /// <param name="selectedPath"></param>
        /// <returns></returns>
        public static string GetDirectoryNameFromPath(string selectedPath)
        {
            string directoryName = "";

            Regex regex = new Regex(@"\\{1}[^/\\:*?<>\|""]+$");
            var match = regex.Match(selectedPath);

            if (match.Success)
            {
                directoryName = match.Value.Remove(0, 1);
            }

            return directoryName;
        }


        public static string[] UpdatedFileNames(string selectedPath, string prefix, string suffix, string originalName, string extension, string find, string replace)
        {
            var originalFiles = GetFilesMatchPattern(selectedPath, "*" + extension);

            var destinationFiles = new List<string>();

            if (GeneralHelpers.IsNullOrEmpty(originalFiles) != true)
            {
                foreach (string oFile in originalFiles)
                {
                    //var updatedName = UpdateFileName(oFile, prefix, suffix, find, replace, useTitleCase);
                    //destinationFiles.Add(updatedName);
                }
            }

            return destinationFiles.ToArray();
        }

        /// <summary>
        /// Function to update a path name
        /// </summary>
        /// <param name="IsFile"></param>
        /// <param name="selectedPath"></param>
        /// <param name="find"></param>
        /// <param name="replace"></param>
        /// <param name="prefix"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public static string UpdatePathName(bool IsFile,
                                            bool toTitleCase,
                                            ComboBoxItem comboItem,
                                            string selectedPath,
                                            string find,
                                            string replace,
                                            string prefix,
                                            string suffix)
        {
            string originalName = "";

            // If path is a file
            if (IsFile)
            {
                if (comboItem != null)
                {
                    string stringMatch = comboItem.Content as string;

                    string[] fileNames = GetFilesMatchPattern(selectedPath, "*" + stringMatch);
                    if (GeneralHelpers.IsNullOrEmpty(fileNames) != true)
                    {
                        originalName = fileNames[0].Replace(selectedPath, "").Remove(0, 1);
                    }
                }
                else
                {
                    string fileName = GetFirstFileFromFilePath(selectedPath);
                    originalName = fileName;
                }
            }
            // If path is a directory
            else
            {
                string folderName = GetFirstFolderFromPath(selectedPath);
                originalName = folderName;
            }

            string updatedName = "";

            // Update file name
            if (IsFile)
            {
                // Split file name into name and extension
                string extension = GetFilePathExtension(originalName);
                if (extension != "")
                {             
                    updatedName = UpdateFileName(selectedPath + "\\" + originalName, prefix, suffix, find, replace, toTitleCase);  
                }
            }
            else
            {
                updatedName = UpdateDirectoryName(selectedPath, prefix, suffix, find, replace, toTitleCase);
            }

            return updatedName;
        }
    }
}
