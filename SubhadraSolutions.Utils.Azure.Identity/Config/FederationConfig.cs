namespace SubhadraSolutions.Utils.Azure.Identity.Config
{
    public class FederationConfig : AbstractScopesConfig, IAzureIdentity
    {
        public FederationConfig()
            : base(Defaults.AzureADTokenExchangeScopes)
        {
        }

        public string ClientId { get; set; }
        public string TenantId { get; set; }
    }
}