namespace SubhadraSolutions.Utils.Azure.Identity
{
    public class IdentityInfo : IAzureIdentity
    {
        public IdentityInfo(string clientId, string tenantId, string upn, string objectId, string authority)
        {
            this.ClientId = clientId;
            this.TenantId = tenantId;
            this.Upn = upn;
            this.ObjectId = objectId;
            this.Authority = authority;
        }

        public string ClientId { get; }
        public string TenantId { get; }
        public string Upn { get; }
        public string ObjectId { get; }
        public string Authority { get; }
    }
}