using System;

namespace SubhadraSolutions.Utils.Identity
{
    public class AccessTokenInfo
    {
        public AccessTokenInfo(string accessToken, DateTimeOffset expiresOn, string tokenType = null)
        {
            AccessToken = accessToken;
            TokenType = tokenType;
            ExpiresOn = expiresOn;
        }

        //
        // Summary:
        //     Get the access token value.
        public string AccessToken { get; }

        //
        // Summary:
        //     Gets the time when the provided token expires.
        public DateTimeOffset ExpiresOn { get; }

        //
        // Summary:
        //     Get the access token type.
        public string TokenType { get; }
    }
}