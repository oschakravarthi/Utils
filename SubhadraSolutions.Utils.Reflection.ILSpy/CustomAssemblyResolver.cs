using ICSharpCode.Decompiler;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;
using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Reflection.ILSpy;

public class CustomAssemblyResolver(IEnumerable<string> files) : AbstractOnProgress, IAssemblyResolver, IDisposable
{
    public static DecompilerSettings SETTINGS = new()
    {
        LoadInMemory = true,
        ApplyWindowsRuntimeProjections = true,
        ThrowOnAssemblyResolveErrors = false,
        AlwaysCastTargetsOfExplicitInterfaceImplementationCalls = true
    };

    private readonly Dictionary<string, PEFile> fileNameAndPEFileLookup = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, PEFile> assemblyNameAndPEFileLookup = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> files = new(files);

    public PEFile Resolve(IAssemblyReference reference)
    {
        assemblyNameAndPEFileLookup.TryGetValue(reference.Name, out var peFile);
        return peFile;
    }

    public Task<PEFile> ResolveAsync(IAssemblyReference reference)
    {
        return Task.Factory.StartNew(() => Resolve(reference));
    }

    public PEFile ResolveModule(PEFile mainModule, string moduleName)
    {
        var baseDir = Path.GetDirectoryName(mainModule.FileName);
        var moduleFileName = Path.Combine(baseDir, moduleName);
        if (!File.Exists(moduleFileName))
        {
            throw new Exception($"Module {moduleName} could not be found");
        }

        return new PEFile(moduleFileName, new FileStream(moduleFileName, FileMode.Open, FileAccess.Read));
    }

    public Task<PEFile> ResolveModuleAsync(PEFile mainModule, string moduleName)
    {
        return Task.Factory.StartNew(() => ResolveModule(mainModule, moduleName));
    }

    public void Dispose()
    {
        foreach (var kvp in fileNameAndPEFileLookup)
        {
            kvp.Value.Dispose();
        }
    }

    public DecompilerTypeSystem GetDecompilerTypeSystemByFile(string file)
    {
        var peFile = GetPEFileByFile(file);
        if (peFile == null)
        {
            return null;
        }

        return new DecompilerTypeSystem(peFile, this, SETTINGS);
    }

    public PEFile GetPEFileByFile(string file)
    {
        var fileName = Path.GetFileName(file);
        fileNameAndPEFileLookup.TryGetValue(fileName, out var peFile);
        return peFile;
    }

    public void Prepare()
    {
        foreach (var file in files)
        {
            NotifyOnProgress($"Reading PEHeader of assembly {file}");
            var peFile = GetPEFile(file);
            var fileName = Path.GetFileName(file);
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);

            fileNameAndPEFileLookup.Add(fileName, peFile);
            assemblyNameAndPEFileLookup.Add(fileNameWithoutExtension, peFile);
        }

        NotifyOnProgress();
    }

    private PEFile GetPEFile(string file)
    {
        try
        {
            var peFile = new PEFile(file, new FileStream(file, FileMode.Open, FileAccess.Read),
                PEStreamOptions.PrefetchEntireImage,
                SETTINGS.ApplyWindowsRuntimeProjections
                    ? MetadataReaderOptions.ApplyWindowsRuntimeProjections
                    : MetadataReaderOptions.None
            );
            return peFile;
        }
        catch (Exception ex)
        {
            NotifyOnProgress(ex.ToString());
        }

        return null;
    }
}