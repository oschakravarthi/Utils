using Azure.Core;
using SubhadraSolutions.Utils.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Identity
{
    public class TokenCredentialToTokenProviderAdapter : ITokenProvider
    {
        private readonly string[] scopes;
        private readonly TokenCredential tokenCredential;

        public TokenCredentialToTokenProviderAdapter(TokenCredential tokenCredential, string[] scopes)
        {
            this.tokenCredential = tokenCredential;
            this.scopes = scopes;
        }

        public async Task<AccessTokenInfo> GetTokenAsync(CancellationToken cancellationToken)
        {
            var context = new TokenRequestContext(this.scopes);
            var token = await this.tokenCredential.GetTokenAsync(context, cancellationToken).ConfigureAwait(false);
            return new AccessTokenInfo(token.Token, token.ExpiresOn);
        }
    }
}