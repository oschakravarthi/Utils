using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using SubhadraSolutions.Utils.Azure.Identity.Config;

namespace SubhadraSolutions.Utils.Azure.ResourceManager;

public static class ARMHelper
{
    public static ArmClient BuildArmClient(this IdentityConfigWithSecret config, string subscriptionId)
    {
        var key = config.Secret.GetSecret();

        var credential = new ClientSecretCredential(config.TenantId, config.ClientId, key.SecureStringToString());
        var client = new ArmClient(credential, subscriptionId);
        return client;
    }

    public static ArmClient BuildArmClient(this TokenCredential tokenCredential, string subscriptionId)
    {
        var client = new ArmClient(tokenCredential, subscriptionId);
        return client;
    }
}