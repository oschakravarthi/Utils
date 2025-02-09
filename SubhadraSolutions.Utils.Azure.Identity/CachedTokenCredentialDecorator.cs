using Azure.Core;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Identity
{
    public class CachedTokenCredentialDecorator : TokenCredential
    {
        private readonly object syncLock = new object();

        private AccessToken? lastAccessToken = null;
        private readonly TimeSpan tokenExpiryBuffer;

        private readonly TokenCredential tokenCredential;
        public static TimeSpan DefaultTokenExpiryBuffer = TimeSpan.FromSeconds(30);

        public CachedTokenCredentialDecorator(TokenCredential tokenCredential)
            : this(tokenCredential, DefaultTokenExpiryBuffer)
        {
        }

        public CachedTokenCredentialDecorator(TokenCredential tokenCredential, TimeSpan tokenExpiryBuffer)
        {
            this.tokenCredential = tokenCredential;
            this.tokenExpiryBuffer = tokenExpiryBuffer;
        }

        public override AccessToken GetToken(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            var token = GetTokenAsync(requestContext, cancellationToken).Result;
            return token;
        }

        public override ValueTask<AccessToken> GetTokenAsync(TokenRequestContext requestContext, CancellationToken cancellationToken)
        {
            if (ShouldGetToken())
            {
                lock (syncLock)
                {
                    if (ShouldGetToken())
                    {
                        this.lastAccessToken = this.tokenCredential.GetTokenAsync(requestContext, cancellationToken).Result;
                    }
                }
            }
            return new ValueTask<AccessToken>(this.lastAccessToken.Value);
        }

        private bool ShouldGetToken()
        {
            if (this.lastAccessToken == null)
            {
                return true;
            }
            return DateTimeOffset.UtcNow <= (this.lastAccessToken.Value.ExpiresOn - tokenExpiryBuffer);
        }
    }
}