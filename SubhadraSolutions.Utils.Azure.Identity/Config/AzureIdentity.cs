namespace SubhadraSolutions.Utils.Azure.Identity.Config
{
    public class AzureIdentity : IAzureIdentity
    {
        public string ClientId { get; set; }
        public string TenantId { get; set; }
    }
}