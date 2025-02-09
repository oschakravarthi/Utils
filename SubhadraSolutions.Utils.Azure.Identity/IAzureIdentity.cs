namespace SubhadraSolutions.Utils.Azure.Identity
{
    public interface IAzureIdentity
    {
        string ClientId { get; }
        public string TenantId { get; }
    }
}