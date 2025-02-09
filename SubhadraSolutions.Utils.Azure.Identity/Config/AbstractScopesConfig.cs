namespace SubhadraSolutions.Utils.Azure.Identity.Config
{
    public abstract class AbstractScopesConfig
    {
        private string _scopes;

        protected AbstractScopesConfig(string scopes)
        {
            this._scopes = scopes;
        }

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
}