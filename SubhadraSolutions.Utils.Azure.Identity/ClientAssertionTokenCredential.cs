using Azure.Core;
using Azure.Identity;
using SubhadraSolutions.Utils.Azure.Identity.Config;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Identity
{
    public class ClientAssertionTokenCredential : TokenCredential
    {
        private readonly TokenCredential clientAssertionCredential;
        private readonly TokenCredential thisApplicationTokenCredential;
        private readonly TokenRequestContext tokenRequestContext;

        public ClientAssertionTokenCredential(TokenCredential thisApplicationTokenCredential, FederationConfig federation)
        {
            this.thisApplicationTokenCredential = thisApplicationTokenCredential;
            this.tokenRequestContext = new TokenRequestContext(federation.GetScopes());
            this.clientAssertionCredential = new ClientAssertionCredential(federation.TenantId, federation.ClientId, this.ComputeAssertionAsync);
        }

        /// <summary>
        /// Get an exchange token from our managed identity to use as an assertion.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A signed assertion to authenticate with AzureAD.</returns>
        private async Task<string> ComputeAssertionAsync(CancellationToken cancellationToken)
        {
            var token = await this.thisApplicationTokenCredential.GetTokenAsync(this.tokenRequestContext, cancellationToken).ConfigureAwait(false);
            return token.Token;
        }

        /// <inheritdoc/>
        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return this.clientAssertionCredential.GetToken(requestContext, cancellationToken);
        }

        /// <inheritdoc/>
        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            return this.clientAssertionCredential.GetTokenAsync(requestContext, cancellationToken);
        }
    }
}