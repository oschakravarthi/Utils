using SubhadraSolutions.Utils.Azure.Security;
using System.Security;

namespace SubhadraSolutions.Utils.Azure.Identity.Config
{
    public class SecretConfig
    {
        private readonly object _synclock = new();
        private SecureString _secret;
        private string _secretUri;

        public SecureString GetSecret()
        {
            if (_secret == null)
            {
                lock (_synclock)
                {
                    if (_secret == null)
                    {
                        _secret = KeyVaultHelper.GetSecretValueAsync(this._secretUri).Result;
                    }
                }
            }

            return _secret;
        }

        public string SecretUri
        {
            get => _secretUri;
            set
            {
                if (_secretUri != value)
                {
                    _secretUri = value;
                    _secret = null;
                }
            }
        }
    }
}