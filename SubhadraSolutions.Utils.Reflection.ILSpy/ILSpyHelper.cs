using ICSharpCode.Decompiler.IL;
using ICSharpCode.Decompiler.TypeSystem;
using ICSharpCode.Decompiler.TypeSystem.Implementation;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace SubhadraSolutions.Utils.Reflection.ILSpy;

public static class ILSpyHelper
{
    public static IType CleanType(IType type)
    {
        if (type is NullabilityAnnotatedType nullable)
        {
            type = nullable.TypeWithoutAnnotation;
        }

        return type;
    }

    public static ILFunction GetILFunction(IMethod method)
    {
        if (!method.HasBody)
        {
            return null;
        }

        var peFile = method.MemberDefinition.ParentModule.PEFile;
        var methodDef = peFile.Metadata.GetMethodDefinition((MethodDefinitionHandle)method.MetadataToken);
        var methodBody = peFile.Reader.GetMethodBody(methodDef.RelativeVirtualAddress);
        var ilReader = new ILReader((MetadataModule)method.MemberDefinition.ParentModule);
        var function = ilReader.ReadIL((MethodDefinitionHandle)method.MetadataToken, methodBody);
        return function;
    }

    public static IEnumerable<IMethod> GetMethodsOfType(IType type, bool includePropertyGettersAndSetters)
    {
        foreach (var method in type.GetMethods())
        {
            yield return method;
        }

        if (includePropertyGettersAndSetters)
        {
            foreach (var property in type.GetProperties(options: GetMemberOptions.IgnoreInheritedMembers))
            {
                if (property.Getter != null)
                {
                    yield return property.Getter;
                }

                if (property.Setter != null)
                {
                    yield return property.Setter;
                }
            }
        }
    }

    public static bool HasBaseType(this ITypeDefinition type, string baseType)
    {
        var stack = new Stack<IType>();
        stack.Push(type);
        while (stack.Count > 0)
        {
            var t = stack.Pop();
            foreach (var b in t.DirectBaseTypes)
            {
                if (b.FullName == baseType)
                {
                    return true;
                }

                stack.Push(b);
            }
        }

        return false;
    }

    public static bool IsGenericTypeDefinitoion(IType type)
    {
        var parameterizedReference = type as ParameterizedType;
        return parameterizedReference == null;
    }

    public static bool IsValidType(IType type)
    {
        if (type == null)
        {
            return false;
        }

        var typeName = type.Name;
        if (typeName == null)
        {
            return false;
        }

        if (typeName is "<Module>" or "<PrivateImplementationDetails>")
        {
            return false;
        }

        if (typeName.StartsWith("<>f__AnonymousType")
            || type.ReflectionName.StartsWith("<PrivateImplementationDetails>")
            || type.ReflectionName.StartsWith("Microsoft.CodeAnalysis."))
        {
            return false;
        }

        if (type.ReflectionName.StartsWith("System.Runtime.CompilerServices.") &&
            type.ReflectionName.EndsWith("Attribute"))
        {
            return false;
        }

        return true;
    }
}