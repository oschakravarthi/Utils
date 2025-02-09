using System;

namespace SubhadraSolutions.Utils.CodeContracts.Annotations;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Constructor)]
public sealed class ILEmitterAttribute : Attribute
{
}