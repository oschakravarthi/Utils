using SubhadraSolutions.Utils.IO;
using System;
using System.IO;
using System.IO.Compression;

namespace SubhadraSolutions.Utils.Compression.Zip;

public static class ZipHelper
{
    public static void Extract(string zipFile, string extractPath)
    {
        ZipFile.ExtractToDirectory(zipFile, extractPath);
    }

    public static string UnwrapDirectory(string sourceDirectory, Action<string> statusFunc = null)
    {
        var tempDirectory = Path.GetTempPath().TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar +
                            Guid.NewGuid();
        UnwrapDirectory(sourceDirectory, tempDirectory, false, statusFunc);
        return tempDirectory;
    }

    public static void UnwrapDirectory(string sourceDirectory, string targetDirectory, Action<string> statusFunc = null)
    {
        UnwrapDirectory(sourceDirectory, targetDirectory, false, statusFunc);
    }

    public static void UnwrapDirectoryInplace(string directory, Action<string> statusFunc = null)
    {
        UnwrapDirectory(directory, directory, true, statusFunc);
    }

    private static void UnwrapDirectory(string sourceDirectory, string targetDirectory, bool isExtracted,
        Action<string> statusFunc)
    {
        if (!isExtracted)
        {
            IOHelper.CopyDirectory(sourceDirectory, targetDirectory, [], [".zip"]);
        }

        var zipFiles = Directory.GetFiles(sourceDirectory, "*.zip", SearchOption.AllDirectories);
        foreach (var zipFile in zipFiles)
        {
            //var fi = new FileInfo(zipFile);
            var extractPath = Path.GetDirectoryName(zipFile) + Path.DirectorySeparatorChar +
                              IOHelper.GetNameWithoutExtension(zipFile);
            extractPath = extractPath.Replace(sourceDirectory, targetDirectory, StringComparison.OrdinalIgnoreCase);
            if (statusFunc != null)
            {
                statusFunc("Unzipping " + zipFile);
            }

            Extract(zipFile, extractPath);

            if (isExtracted)
            {
                File.Delete(zipFile);
            }

            UnwrapDirectory(extractPath, extractPath, true, statusFunc);
        }
    }
}