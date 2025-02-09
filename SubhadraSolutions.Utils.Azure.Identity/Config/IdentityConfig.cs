namespace SubhadraSolutions.Utils.Azure.Identity.Config;

public class IdentityConfig : SecretConfig, IAzureIdentity
{
    public const string DefaultSectionName = "AzureAd";
    private string _authorityUri = Defaults.AzureAdAuthorityUri;

    public string ClientId { get; set; }
    public string TenantId { get; set; }

    public string AuthorityUri
    {
        get { return _authorityUri; }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                this._authorityUri = value;
            }
        }
    }
}