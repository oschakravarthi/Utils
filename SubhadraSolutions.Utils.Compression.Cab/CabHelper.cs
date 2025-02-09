using SubhadraSolutions.Utils.Diagnostics.Tracing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SubhadraSolutions.Utils.Compression.Cab;

public static class CabHelper
{
    public static string DecompressAndGetFirstFileContentsAsString(this byte[] compressedData)
    {
        return DecompressAndGetFirstFileContentsAsString(compressedData, Encoding.Unicode);
    }

    public static string DecompressAndGetFirstFileContentsAsString(this byte[] compressedData, Encoding encoding)
    {
        using var engine = new CabEngine();
        using var input = new MemoryStream(compressedData);
        if (engine.IsArchive(input))
        {
            foreach (var archiveFileInfo in engine.GetFileInfo(input))
            {
                using var stream = engine.Unpack(input, archiveFileInfo.Name);
                using var reader = new StreamReader(stream, encoding);
                return reader.ReadToEnd();
            }
        }

        return null;
    }

    /// <summary>
    /// Extract files in the CAB that match a specific pattern into the specified directory.
    /// Note this will only work for files in the root directory of the CAB.
    /// Make sure the output directory exists and is empty. (To avoid hitting this, pass in a destDir value related to the input CAB file name.)
    /// </summary>
    /// <param name="cabFile">Path to the CAB file</param>
    /// <param name="searchPattern">The search string, such as &quot;*.txt&quot;</param>
    /// <param name="destDir">Path to the output directory</param>
    public static void ExpandCabFiles(string cabFile, string searchPattern, string destDir)
    {
        TraceLogger.TraceInformation("[ExpandCabFiles] cabFile = {0}, searchPattern = {1}, destDir = {2}", cabFile, searchPattern, destDir);
        if (Directory.Exists(destDir))
        {
            if (Directory.EnumerateFileSystemEntries(destDir).Any())
            {
                throw new ArgumentException("The destination directory needs to be empty!", nameof(destDir));
            }
        }
        else
        {
            throw new DirectoryNotFoundException("The destination directory does not exist. (Path = \"" + destDir + "\")");
        }

        try
        {
            var cab = new CabInfo(cabFile);
            var files = cab.GetFiles(searchPattern);
            IList<string> filenames = files.Select(x => x.Name).Distinct().ToList();
            cab.UnpackFiles(filenames, destDir, null);
        }
        catch (CabException ex)
        {
            TraceLogger.TraceInformation("[ExpandCabFiles] Warning: Hit {0} while processing CAB file \"{1}\"; skipping this Update. Exception details:\r\n{2}",
                ex.GetType().Name, cabFile, ex);
        }
    }
}