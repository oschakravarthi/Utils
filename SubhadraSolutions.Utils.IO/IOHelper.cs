using SubhadraSolutions.Utils.Validation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SubhadraSolutions.Utils.IO;

public static class IOHelper
{
    public static IEnumerable<FileInfo> GetAllFiles(string path, string searchPattern, Func<DirectoryInfo, bool> excludeFunc)
    {
        var stack = new Stack<DirectoryInfo>();
        var directory = new DirectoryInfo(path);
        if (!excludeFunc(directory))
        {
            stack.Push(directory);
        }
        while (stack.Count > 0)
        {
            directory = stack.Pop();
            var files = directory.GetFiles(searchPattern, SearchOption.TopDirectoryOnly);
            foreach (var file in files)
            {
                yield return file;
            }

            var directories = directory.GetDirectories("*", SearchOption.TopDirectoryOnly);
            foreach (var d in directories)
            {
                if (!excludeFunc(d))
                {
                    stack.Push(d);
                }
            }
        }
    }

    public static void CopyAll(DirectoryInfo source, DirectoryInfo target, string[] directoryNamesToExclude,
        string[] extensionsToExclude)
    {
        if (source == null || target == null)
        {
            throw new ArgumentException("Invalid input");
        }

        var stack = new Stack<KeyValuePair<DirectoryInfo, DirectoryInfo>>();
        stack.Push(new KeyValuePair<DirectoryInfo, DirectoryInfo>(source, target));
        while (stack.Count > 0)
        {
            var kvp = stack.Pop();
            var from = kvp.Key;
            var shouldSkipDirectory = false;
            if (directoryNamesToExclude != null)
            {
                foreach (var dn in directoryNamesToExclude)
                {
                    if (from.Name.Equals(dn, StringComparison.OrdinalIgnoreCase))
                    {
                        shouldSkipDirectory = true;
                        break;
                    }
                }
            }

            if (shouldSkipDirectory)
            {
                continue;
            }

            var to = kvp.Value;
            Directory.CreateDirectory(to.FullName);
            var files = from.GetFiles();
            foreach (var fi in files)
            {
                var shouldSkip = false;
                if (extensionsToExclude != null)
                {
                    for (var i = 0; i < extensionsToExclude.Length; i++)
                        if (fi.Extension.Equals(extensionsToExclude[i], StringComparison.OrdinalIgnoreCase))
                        {
                            shouldSkip = true;
                            break;
                        }
                }

                if (shouldSkip)
                {
                    continue;
                }

                //Console.WriteLine(@"Copying {0}\{1}", target.ReflectionName, fi.Name);
                var toFile = Path.Combine(to.FullName, fi.Name);
                fi.CopyTo(toFile, true);
            }

            var directories = from.GetDirectories();
            foreach (var d in directories)
            {
                stack.Push(new KeyValuePair<DirectoryInfo, DirectoryInfo>(d,
                    new DirectoryInfo(Path.Combine(to.FullName, d.Name))));
            }
        }
    }

    public static void CopyDirectory(string sourceDirectory, string targetDirectory, string[] directoryNamesToExclude,
        string[] extensionsToExclude)
    {
        var diSource = new DirectoryInfo(sourceDirectory);
        var diTarget = new DirectoryInfo(targetDirectory);

        CopyAll(diSource, diTarget, directoryNamesToExclude, extensionsToExclude);
    }

    public static string CreateAndGetTemporaryDirectory()
    {
        var tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDirectory);
        return tempDirectory;
    }

    public static string CreateDirectory(string parentDirectoryPath, string directoryName)
    {
        var path = Path.Combine(parentDirectoryPath, directoryName);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return path;
    }

    public static void CreateFileWithContent(string directoryName, string fileName, string fileContent,
        Encoding encoding = null)
    {
        Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(directoryName, nameof(directoryName));
        Guard.ArgumentShouldNotBeNullOrEmptyOrWhiteSpace(fileName, nameof(fileName));
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }

        var filePath = Path.Combine(directoryName, fileName);
        if (encoding == null)
        {
            File.WriteAllText(filePath, fileContent);
        }
        else
        {
            File.WriteAllText(filePath, fileContent, encoding);
        }
    }

    public static bool DeleteFile(string fileName, bool checkExists)
    {
        if (checkExists)
        {
            if (File.Exists(fileName))
            {
                return DeleteFileUnsafe(fileName);
            }

            return true;
        }

        return DeleteFileUnsafe(fileName);
    }

    public static IEnumerable<string> GetLinesEnumerable(this TextReader reader)
    {
        return new TextReaderEnumerable(reader);
    }

    public static string GetNameWithoutExtension(string fileName)
    {
        var fi = new FileInfo(fileName);
        var name = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        return name;
    }

    public static bool IsFileLocked(string fileName)
    {
        FileStream stream = null;
        try
        {
            stream = File.Open(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        }
        catch (IOException)
        {
            return true;
        }
        finally
        {
            stream?.Close();
        }

        return false;
    }

    public static bool IsInsideADirectoryWithGivenName(string path, string directoryName)
    {
        return path.IndexOf(directoryName + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) == 0 ||
               path.IndexOf(Path.DirectorySeparatorChar + directoryName + Path.DirectorySeparatorChar,
                   StringComparison.OrdinalIgnoreCase) > -1;
    }

    public static bool IsInsidePackagesDirectory(string path)
    {
        return IsInsideADirectoryWithGivenName(path, "packages");
    }

    public static bool MoveFile(string source, string dest, bool checkExists)
    {
        if (checkExists)
        {
            if (File.Exists(source))
            {
                return MoveFileUnsafe(source, dest);
            }

            return true;
        }

        return MoveFileUnsafe(source, dest);
    }

    //public static string ResolvePath(string path)
    //{
    //    return ResolvePath(path, ProcessHelper.RootPathOfApplication);
    //}

    //public static string RelativePath(string absPath, string relTo)
    //{
    //    if (absPath == null || relTo == null)
    //    {
    //        throw new ArgumentException("Invalid input");
    //    }

    //    var absDirs = absPath.Split(Path.DirectorySeparatorChar);
    //    var relDirs = relTo.Split(Path.DirectorySeparatorChar);

    //    // Get the shortest of the two paths
    //    var len = absDirs.Length < relDirs.Length ? absDirs.Length : relDirs.Length;

    //    // Use to determine where in the loop we exited
    //    var lastCommonRoot = -1;
    //    int index;

    //    // Find common root
    //    for (index = 0; index < len; index++)
    //        if (absDirs[index] == relDirs[index])
    //        {
    //            lastCommonRoot = index;
    //        }
    //        else
    //        {
    //            break;
    //        }

    //    // If we didn't find a common prefix then throw
    //    if (lastCommonRoot == -1)
    //    {
    //        throw new ArgumentException("Paths do not have a common base");
    //    }

    //    // Build up the relative path
    //    var relativePath = new StringBuilder();

    //    // Add on the ..
    //    for (index = lastCommonRoot + 1; index < absDirs.Length; index++)
    //        if (absDirs[index].Length > 0)
    //        {
    //            relativePath.Append(".." + Path.DirectorySeparatorChar);
    //        }

    //    // Add on the folders
    //    for (index = lastCommonRoot + 1; index < relDirs.Length - 1; index++)
    //        relativePath.Append(relDirs[index] + Path.DirectorySeparatorChar);
    //    relativePath.Append(relDirs[relDirs.Length - 1]);

    //    return relativePath.ToString();
    //}

    public static string ResolvePath(string path, string relativeDirectory)
    {
        if (Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(path);
        }

        if (!Path.IsPathRooted(path))
        {
            path = Path.GetFullPath(relativeDirectory + Path.DirectorySeparatorChar + path);
        }

        return path;
    }

    //public static string ResolvePath(string path, string basePath)
    //{
    //    return Path.IsPathRooted(path) ? path : Path.Combine(basePath, path);
    //}
    private static bool DeleteFileUnsafe(string fileName)
    {
        try
        {
            File.Delete(fileName);
            return true;
        }
        catch (FileNotFoundException)
        {
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }
    }

    private static bool MoveFileUnsafe(string source, string dest)
    {
        var attemptCount = 0;
        while (true)
            try
            {
                File.Move(source, dest);
                return true;
            }
            catch (FileNotFoundException)
            {
                return true;
            }
            catch (IOException ex)
            {
                Debug.WriteLine(ex);
                if (attemptCount > 1)
                {
                    return false;
                }

                attemptCount++;
                if (!DeleteFile(dest, false))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return false;
            }
    }
}