using DiscUtils.Wim;
using System.IO;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Compression.Wim;

public static class WimHelper
{
    public static async Task<bool> ExtractAsync(string wimFilePath, DirectoryInfo targetDirectory)
    {
        using var stream = File.OpenRead(wimFilePath);
        return await ExtractAsync(stream, targetDirectory).ConfigureAwait(false);
    }

    public static async Task<bool> ExtractAsync(Stream stream, DirectoryInfo targetFolder)
    {
        var wimFile = new WimFile(stream);
        var imageCount = wimFile.ImageCount;
        for (var i = 0; i < imageCount; i++)
        {
            var fileSystem = wimFile.GetImage(i);
            var target = new DirectoryInfo(targetFolder.FullName + Path.DirectorySeparatorChar + i);
            await FileSystemHelper.CopyAsync(fileSystem, target).ConfigureAwait(false);
        }

        return true;
    }
}