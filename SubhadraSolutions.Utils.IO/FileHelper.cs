using SubhadraSolutions.Utils.Diagnostics;
using SubhadraSolutions.Utils.Diagnostics.Tracing;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.IO;

public static class FileHelper
{
    /// <summary>
    ///     Find the first *.txt file in the directory and get its contents as a string.
    /// </summary>
    /// <param name="dirPath">The directory to look in for the *.txt file</param>
    /// <returns>The content of the file if it exists, NULL if there were no *.txt files.</returns>
    public static string GetTextContents(string dirPath)
    {
        var d = new DirectoryInfo(dirPath);
        var files = d.GetFiles("*.txt");
        if (files.Length > 1)
        {
            TraceLogger.TraceWarning("[GetTextContents] Found {0} \"*.txt\" files! (Expected only 1.)", files.Length);
            var ii = 0;
            foreach (var file in files)
            {
                TraceLogger.TraceInformation("[GetTextContents] Debug: files[{0}]: \"{1}\" ({2} bytes)",
                    ii++,
                    file.Name,
                    file.Length);
            }
        }

        return files.Select(file => File.ReadAllText(file.FullName)).FirstOrDefault();
    }

    public static TextWriter OpenFileForSave(string nameFile, string nameFn, string descFile = null)
    {
        if (nameFile == null)
        {
            throw new ArgumentNullException(nameof(nameFile));
        }

        if (descFile == null)
        {
            descFile = "configuration file";
        }

        TextWriter result = null;
        var text = string.Empty;
        if (nameFile.LastIndexOf(Path.DirectorySeparatorChar) > 0)
        {
            text = nameFile.Substring(0, nameFile.LastIndexOf(Path.DirectorySeparatorChar));
        }

        if (!string.IsNullOrEmpty(text) && !Directory.Exists(text))
        {
            Directory.CreateDirectory(text);
        }

        try
        {
            result = File.CreateText(nameFile);
            return result;
        }
        catch (DirectoryNotFoundException ex)
        {
            TraceLogger.TraceWarning(
                nameFn +
                ": Couldn't open {0} \"{1}\" for writing -- the target directory doesn't exist. (Exception details: {2})",
                descFile, nameFile, ex);
            return result;
        }
        catch (IOException ex2)
        {
            TraceLogger.TraceWarning(
                nameFn +
                ": Couldn't open {0} \"{1}\" for writing -- file may already be locked for reading/writing by another process. (Exception details: {2})",
                descFile, nameFile, ex2);
            return result;
        }
    }

    public static StreamReader OpenResourceForRead(string resourceName, string resourceNamespace = null,
        Assembly fromAssembly = null, string nameFn = null)
    {
        nameFn = nameFn != null ? ActiveCode.Current.Name + " [for " + nameFn + "]" : ActiveCode.Current.Name;
        if (resourceName == null)
        {
            throw new ArgumentNullException(nameof(resourceName));
        }

        if (fromAssembly == null)
        {
            fromAssembly = Assembly.GetEntryAssembly();
        }

        string text = null;
        text = resourceNamespace == null
            ? fromAssembly.GetManifestResourceNames().FirstOrDefault(item => item.EndsWith("." + resourceName))
            : resourceNamespace + "." + resourceName;
        StreamReader streamReader = null;
        if (string.IsNullOrEmpty(text))
        {
            TraceLogger.TraceWarning(nameFn + ": Couldn't determine full path for resource \"[...].{0}\"!",
                resourceName);
        }
        else
        {
            try
            {
                TraceLogger.TraceWarning(nameFn + ": Opening resource \"{0}\".", text);
                streamReader = new StreamReader(fromAssembly.GetManifestResourceStream(text));
            }
            catch (FileNotFoundException)
            {
            }
            catch (FileLoadException)
            {
            }
            catch (BadImageFormatException)
            {
            }
            catch (NotImplementedException)
            {
            }

            if (streamReader == null)
            {
                TraceLogger.TraceWarning(nameFn + ": Couldn't open resource \"{0}\"!", text);
            }
        }

        return streamReader;
    }
}