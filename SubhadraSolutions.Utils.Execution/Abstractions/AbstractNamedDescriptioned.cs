using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils.Execution.Abstractions;

public abstract class AbstractNamedDescriptioned : AbstractNamed, IDescriptioned
{
    public string Description { get; set; }
}