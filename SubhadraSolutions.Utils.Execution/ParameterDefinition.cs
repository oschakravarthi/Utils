using SubhadraSolutions.Utils.Execution.Abstractions;
using System;

namespace SubhadraSolutions.Utils.Execution;

public sealed class ParameterDefinition : AbstractNamed
{
    public Type Type { get; set; }
}