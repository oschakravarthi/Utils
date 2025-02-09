using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SubhadraSolutions.Utils.IO;

public static class FindReplaceHelper
{
    public static List<string> RenameFiles(string directoryName, string oldText, string newText,
        params string[] filePatterns)
    {
        var fileNames = GetAllFilesFromDirectory(directoryName, filePatterns);
        return RenameFiles(fileNames, oldText, newText);
    }

    public static List<string> RenameFiles(IEnumerable<string> fileNames, string oldText, string newText)
    {
        var newList = new List<string>();
        foreach (var fileName in fileNames)
        {
            var newFileName = fileName.Replace(oldText, newText);
            if (fileName != newFileName)
            {
                File.Move(fileName, newFileName);
            }

            newList.Add(newFileName);
        }

        return newList;
    }

    public static string ReplaceDirectoryNames(string directoryName, string oldText, string newText)
    {
        var newDirectoryName = directoryName.Replace(oldText, newText);
        if (directoryName != newDirectoryName)
        {
            Directory.Move(directoryName, newDirectoryName);
        }

        var directories = Directory.GetDirectories(newDirectoryName);
        foreach (var directory in directories)
        {
            ReplaceDirectoryNames(directory, oldText, newText);
        }

        return newDirectoryName;
    }

    public static string ReplaceInDirectoriesAndFiles(string directoryName, string oldText, string newText,
        bool renameDirectories, bool renameFiles, bool replaceFileContents, params string[] filePatterns)
    {
        if (filePatterns == null || filePatterns.Length == 0)
        {
            filePatterns = ["*.*"];
        }

        if (!renameFiles && !renameDirectories && !replaceFileContents)
        {
            throw new ArgumentException(
                "Any one of renameFiles, renameDirectories, replaceFileContents should be true");
        }

        directoryName = renameDirectories ? ReplaceDirectoryNames(directoryName, oldText, newText) : directoryName;
        var directories = Directory.GetDirectories(directoryName, "*", SearchOption.AllDirectories);
        List<string> fileNames;
        if (renameFiles)
        {
            fileNames = RenameFiles(directoryName, oldText, newText, filePatterns);
        }
        else
        {
            fileNames = GetAllFilesFromDirectory(directoryName, filePatterns);
        }

        if (replaceFileContents)
        {
            ReplaceInFiles(fileNames, oldText, newText);
        }

        return directoryName;
    }

    public static void ReplaceInFile(string fileName, string oldText, string newText)
    {
        var fileContent = File.ReadAllText(fileName);
        var newFileContent = fileContent.Replace(oldText, newText);
        if (newFileContent != fileContent)
        {
            try
            {
                File.WriteAllText(fileName, newFileContent);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToDetailedString());
            }
        }
    }

    public static void ReplaceInFiles(IEnumerable<string> fileNames, string oldText, string newText)
    {
        foreach (var fileName in fileNames)
        {
            ReplaceInFile(fileName, oldText, newText);
        }
    }

    private static List<string> GetAllFilesFromDirectory(string directoryName, params string[] filePatterns)
    {
        var filesList = new List<string>();
        foreach (var filePattern in filePatterns)
        {
            var files = Directory.GetFiles(directoryName, filePattern, SearchOption.AllDirectories);
            filesList.AddRange(files);
        }

        return filesList;
    }
}