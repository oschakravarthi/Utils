using ICSharpCode.Decompiler.TypeSystem;
using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace SubhadraSolutions.Utils.Reflection.ILSpy;

public class ReflectionCache : AbstractOnProgress, IDisposable
{
    private readonly List<string> files;
    private readonly CustomAssemblyResolver resolver;

    public ReflectionCache(IEnumerable<string> files)
    {
        this.files = new List<string>(files);
        resolver = new CustomAssemblyResolver(this.files);
        resolver.OnProgress += (o, e) => { NotifyOnProgress(e.Payload); };
    }

    public List<AssemblyCache> AssemblyCachesInSequence { get; set; } = [];

    //public HashSet<IType> AllTypes { get; private set; } = new HashSet<IType>();
    public HashSet<string> FailedToCache { get; } = new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<string, AssemblyCache> FileNameAndAssemblyCacheLookup { get; } =
        new(StringComparer.OrdinalIgnoreCase);

    public Dictionary<IType, AssemblyCache> TypeAndAssemblyLookup { get; } = [];

    public void Dispose()
    {
        resolver.Dispose();
    }

    public void BuildCache()
    {
        resolver.Prepare();
        foreach (var file in files)
        {
            NotifyOnProgress($"Reading assembly {file}");
            var info = CacheAssembly(file, resolver);
            if (info == null)
            {
                FailedToCache.Add(file);
                //this.analyzer.Errors.Add($"Could not read assembly {file}");
                NotifyOnProgress($"Could not read assembly {file}");
                continue;
            }

            foreach (var type in info.Types)
            {
                var cleaned = ILSpyHelper.CleanType(type);
                TypeAndAssemblyLookup.Add(cleaned, info);
            }

            var fileName = Path.GetFileName(file);
            FileNameAndAssemblyCacheLookup.Add(fileName, info);
            AssemblyCachesInSequence.Add(info);
        }

        NotifyOnProgress();
    }

    public AssemblyCache GetAssemblyCache(IType type)
    {
        while (true)
        {
            if (TypeAndAssemblyLookup.TryGetValue(type, out var assemblyCache))
            {
                return assemblyCache;
            }

            if (type is not ParameterizedType parameterizedType)
            {
                return null;
            }

            type = parameterizedType.GenericType;
        }
    }

    private static AssemblyCache CacheAssembly(string file, CustomAssemblyResolver resolver)
    {
        try
        {
            var typeSystem = resolver.GetDecompilerTypeSystemByFile(file);

            if (typeSystem != null)
            {
                var info = new AssemblyCache
                {
                    FileFullName = file,
                    TypeSystem = typeSystem
                };
                //AllDecompilerTypeSystems.Add(fileName, new Tuple<DecompilerTypeSystem, PEFile>(typeSystem, peFile));
                foreach (var type in typeSystem.MainModule.TypeDefinitions)
                {
                    if (ILSpyHelper.IsValidType(type))
                    {
                        info.Types.Add(type);
                    }
                }

                return info;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(file + "\t" + ex);
        }

        return null;
    }
}