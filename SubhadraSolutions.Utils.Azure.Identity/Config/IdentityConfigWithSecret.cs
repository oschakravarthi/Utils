namespace SubhadraSolutions.Utils.Azure.Identity.Config;

public class IdentityConfigWithSecret : IdentityConfig
{
    public SecretConfig Secret { get; set; }
}