using SubhadraSolutions.Utils.Contracts;

namespace SubhadraSolutions.Utils;

public class ProductInfo : INamed
{
    public const string DefaultSectionName = "ProductInfo";
    public string Description { get; set; }
    public string Environment { get; set; }
    public string Name { get; set; }
    public string ShortName { get; set; }
    public bool IsServer { get; set; }
    public string Version { get; set; }
    public bool ForInternalUseOnly { get; set; } = false;
}