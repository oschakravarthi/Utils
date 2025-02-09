using DiscUtils;
using System.IO;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Compression.Wim;

internal static class FileSystemHelper
{
    public static async Task<bool> CopyAsync(IFileSystem source, IFileSystem target)
    {
        var result = await CopyDirectoryAsync(source, target, @"\").ConfigureAwait(false);
        return result;
    }

    private static async Task<bool> CopyDirectoryAsync(IFileSystem source, IFileSystem target, string directoryPath)
    {
        if (!target.DirectoryExists(directoryPath))
        {
            target.CreateDirectory(directoryPath);
        }

        var files = source.GetFiles(directoryPath);
        foreach (var file in files)
        {
            await CopyFileAsync(source, target, file).ConfigureAwait(false);
        }

        var directories = source.GetDirectories(directoryPath);
        foreach (var directory in directories)
        {
            await CopyDirectoryAsync(source, target, directory).ConfigureAwait(false);
        }

        return true;
    }

    private static async Task<bool> CopyFileAsync(IFileSystem source, IFileSystem target, string filePath)
    {
        var sourceFileInfo = source.GetFileInfo(filePath);
        using var sourceStream = sourceFileInfo.Open(FileMode.Open);
        using var targetStream = target.OpenFile(filePath, FileMode.CreateNew);
        await sourceStream.CopyToAsync(targetStream).ConfigureAwait(false);

        return true;
    }

    public static async Task<bool> CopyAsync(IFileSystem source, DirectoryInfo target)
    {
        var result = await CopyDirectoryAsync(source, target, @"\").ConfigureAwait(false);
        return result;
    }

    private static async Task<bool> CopyDirectoryAsync(IFileSystem source, DirectoryInfo target, string directoryPath)
    {
        if (!target.Exists)
        {
            target.Create();
        }

        var files = source.GetFiles(directoryPath);
        foreach (var file in files)
        {
            await CopyFileAsync(source, file, target).ConfigureAwait(false);
        }

        var directories = source.GetDirectories(directoryPath);
        foreach (var directory in directories)
        {
            var directoryInfo = source.GetDirectoryInfo(directory);
            await CopyDirectoryAsync(source,
                new DirectoryInfo(target.FullName + Path.DirectorySeparatorChar + directoryInfo.Name), directory).ConfigureAwait(false);
        }

        return true;
    }

    private static async Task<bool> CopyFileAsync(IFileSystem source, string sourceFilePath, DirectoryInfo target)
    {
        var sourceFileInfo = source.GetFileInfo(sourceFilePath);
        using var sourceStream = sourceFileInfo.Open(FileMode.Open);
        var targetFile = target.FullName + Path.DirectorySeparatorChar + sourceFileInfo.Name;
        using var targetStream = File.Open(targetFile, FileMode.OpenOrCreate);
        await sourceStream.CopyToAsync(targetStream).ConfigureAwait(false);

        return true;
    }
}