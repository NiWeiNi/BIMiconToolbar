using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.FileIO;

namespace BIMicon.BIMiconToolbar.RemoveBackups.Helpers
{
    class RemoveBackupHelpers
    {
        public static void DeleteBackups(string folderPath, ref int countBackups)
        {
            // Data structure to hold names of subfolders to be examined for files initialized with selected folder
            Stack<string> dirs = new Stack<string>();
            dirs.Push(folderPath);

            // Loop through folders adding subfolders
            while (dirs.Count > 0)
            {
                string currentDir = dirs.Pop();

                try
                {
                    string[] subDirs = Directory.GetDirectories(currentDir);
                    if (subDirs != null)
                    {
                        foreach (string sub in subDirs)
                        {
                            dirs.Push(sub);
                        }
                    }
                }

                // Catch not enough permission to access
                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                // Catch folder deleted after code check
                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                string[] files = null;
                try
                {
                    files = Directory.GetFiles(currentDir);
                }

                catch (UnauthorizedAccessException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                catch (DirectoryNotFoundException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }

                // Delete files that match a backup file
                foreach (string file in files)
                {
                    if (Match(file))
                    {
                        deleteBackups(file);
                        countBackups += 1;
                    }
                }
            }
        }

        // Delete backup files
        private static void deleteBackups(string filePathToDelete)
        {
            try
            {
                FileSystem.DeleteFile(filePathToDelete, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }

        // Method to match file pattern
        private static bool Match(string fileName)
        {
            Regex backupPattern = new Regex(@"\.[0-9]{3,4}\.r(vt|fa|te)");
            Match fileMatch = backupPattern.Match(fileName);

            if (fileMatch.Success)
            {
                return true;
            }
            return false;
        }
    }
}
