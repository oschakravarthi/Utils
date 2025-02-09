namespace SubhadraSolutions.Utils.Azure.Identity.Config;

public class IdentityConfigAndScopes : IdentityConfig
{
    private string _scopes = Defaults.AzureAdScopes;

    public string Scopes
    {
        get { return _scopes; }
        set
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _scopes = value;
            }
        }
    }

    public string[] GetScopes()
    {
        return IdentityHelper.GetScopes(this._scopes);
    }
}