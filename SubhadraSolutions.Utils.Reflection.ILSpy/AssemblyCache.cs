using ICSharpCode.Decompiler.TypeSystem;
using System.Collections.Generic;
using System.IO;

namespace SubhadraSolutions.Utils.Reflection.ILSpy;

public class AssemblyCache
{
    public string AssemblyFileName => Path.GetFileName(FileFullName);

    public string FileFullName { get; set; }
    public HashSet<IType> Types { get; } = [];
    public DecompilerTypeSystem TypeSystem { get; set; }
}