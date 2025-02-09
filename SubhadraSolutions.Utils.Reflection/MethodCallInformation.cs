using System;

namespace SubhadraSolutions.Utils.Reflection;

[Serializable]
internal sealed class MethodCallInformation : AbstractMethodInformation
{
    public string MethodName { get; set; }
}