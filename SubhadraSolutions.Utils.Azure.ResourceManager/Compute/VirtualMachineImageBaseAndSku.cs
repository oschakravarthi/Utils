using Azure.ResourceManager.Compute.Models;

namespace SubhadraSolutions.Utils.Azure.ResourceManager.Compute;

public class VirtualMachineImageBaseAndSku
{
    public VirtualMachineImageBaseAndSku(VirtualMachineImageBase image, string skuName)
    {
        Image = image;
        SkuName = skuName;
    }

    public VirtualMachineImageBase Image { get; }
    public string SkuName { get; }
}