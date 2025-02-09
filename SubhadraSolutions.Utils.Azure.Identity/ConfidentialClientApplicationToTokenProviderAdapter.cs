using Microsoft.Identity.Client;
using SubhadraSolutions.Utils.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Identity;

public class ConfidentialClientApplicationToTokenProviderAdapter : ITokenProvider
{
    private readonly IConfidentialClientApplication confidentialClientApplication;
    private readonly string[] scopes;

    public ConfidentialClientApplicationToTokenProviderAdapter(IConfidentialClientApplication confidentialClientApplication, string[] scopes)
    {
        this.confidentialClientApplication = confidentialClientApplication;
        this.scopes = scopes;
    }

    public async Task<AccessTokenInfo> GetTokenAsync(CancellationToken cancellationToken)
    {
        var authenticationResult = await confidentialClientApplication.AcquireTokenForClient(scopes)
            .ExecuteAsync().ConfigureAwait(false);

        return new AccessTokenInfo(authenticationResult.AccessToken, authenticationResult.ExpiresOn, authenticationResult.TokenType);
    }
}