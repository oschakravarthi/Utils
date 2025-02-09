using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using SubhadraSolutions.Utils.Azure.Identity.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Identity;

public static class IdentityHelper
{
    public static readonly DefaultAzureCredential DefaultCredential = new DefaultAzureCredential();

    public static async Task<IdentityConfig> GetAzureAdConfigAsync(IConfiguration configuration, string configSectionName = IdentityConfig.DefaultSectionName)
    {
        var azureAdConfig = configuration.GetSection(configSectionName).Get<IdentityConfig>();
        if (azureAdConfig == null || Debugger.IsAttached)
        {
            azureAdConfig = new IdentityConfig();
        }
        if (string.IsNullOrWhiteSpace(azureAdConfig.ClientId) || string.IsNullOrWhiteSpace(azureAdConfig.TenantId))
        {
            TokenCredential credential = string.IsNullOrWhiteSpace(azureAdConfig.ClientId) ? IdentityHelper.DefaultCredential : new ManagedIdentityCredential(azureAdConfig.ClientId);
            await PopulateRequiredPropertiesAsync(azureAdConfig, credential).ConfigureAwait(false);
        }
        return azureAdConfig;
    }

    public static TokenCredential CreateTokenCredential(IAzureIdentity thisApplicationIdentity = null, SecureString secret = null)
    {
        if (thisApplicationIdentity == null || thisApplicationIdentity.ClientId == null || thisApplicationIdentity.TenantId == null)
        {
            return DefaultCredential;
        }
        if (secret != null)
        {
            return new ClientSecretCredential(thisApplicationIdentity.TenantId, thisApplicationIdentity.ClientId, secret.SecureStringToString());
        }

        if (Debugger.IsAttached)
        {
            return DefaultCredential;
        }

        return new ManagedIdentityCredential(thisApplicationIdentity.ClientId);
    }

    public static TokenCredential CreateTokenCredential(TokenCredential thisApplicationTokenCredential, FederationConfig federation)
    {
        if (federation == null)
        {
            return thisApplicationTokenCredential;
        }
        return new ClientAssertionTokenCredential(thisApplicationTokenCredential, federation);
    }

    public static TokenCredential EnableTokenCaching(this TokenCredential tokenCredential)
    {
        return new CachedTokenCredentialDecorator(tokenCredential);
    }

    public static TokenCredential EnableTokenCaching(this TokenCredential tokenCredential, TimeSpan tokenExpiryBuffer)
    {
        return new CachedTokenCredentialDecorator(tokenCredential, tokenExpiryBuffer);
    }

    public static async Task<IdentityInfo> GetIdentityInfoAsync(TokenCredential tokenCredential, string[] scopes = null, CancellationToken? cancellationToken = null)
    {
        var lookup = await GetParsedTokenAsync(tokenCredential, scopes, cancellationToken).ConfigureAwait(false);
        var info = BuildIdentityInfoFromParsedToken(lookup);
        return info;
    }

    public static async Task<Dictionary<string, string>> GetParsedTokenAsync(TokenCredential tokenCredential, string[] scopes = null, CancellationToken? cancellationToken = null)
    {
        if (scopes == null || scopes.Length == 0)
        {
            scopes = Defaults.GetAzureAdScopes();
        }
        var token = await tokenCredential.GetTokenAsync(new TokenRequestContext(scopes), cancellationToken ?? CancellationToken.None).ConfigureAwait(false);
        var lookup = ParseToken(token.Token);
        return lookup;
    }

    public static string[] GetScopes(string scopes)
    {
        var result = scopes == null ? Array.Empty<string>() : scopes.Split([';'], StringSplitOptions.RemoveEmptyEntries);
        return result;
    }

    public static Dictionary<string, string> ParseToken(string token)
    {
        string[] array = token.Split('.');
        if (array.Length != 3)
        {
            return null;
        }

        try
        {
            var result = new Dictionary<string, string>();
            string text = array[1].Replace('_', '/').Replace('-', '+');
            switch (array[1].Length % 4)
            {
                case 2:
                    text += "==";
                    break;

                case 3:
                    text += "=";
                    break;
            }

            var data = Convert.FromBase64String(text);
            Utf8JsonReader utf8JsonReader = new Utf8JsonReader(data);
            while (utf8JsonReader.Read())
            {
                if (utf8JsonReader.TokenType == JsonTokenType.PropertyName)
                {
                    var name = utf8JsonReader.GetString();
                    utf8JsonReader.Read();
                    if (utf8JsonReader.TokenType == JsonTokenType.String)
                    {
                        var value = utf8JsonReader.GetString();
                        result.Add(name, value);
                    }
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
        }

        return null;
    }

    public static IdentityInfo ParseIdentityInfoFromToken(string token)
    {
        var lookup = ParseToken(token);
        return BuildIdentityInfoFromParsedToken(lookup);
    }

    public static IdentityInfo BuildIdentityInfoFromParsedToken(IReadOnlyDictionary<string, string> parsedToken)
    {
        if (parsedToken == null)
        {
            return null;
        }
        parsedToken.TryGetValue("appid", out var clientId);
        parsedToken.TryGetValue("tid", out var tenantId);
        parsedToken.TryGetValue("upn", out var upn);
        parsedToken.TryGetValue("oid", out var objectId);
        parsedToken.TryGetValue("aud", out var authority);
        return new IdentityInfo(clientId, tenantId, upn, objectId, authority);
    }

    public static async Task PopulateRequiredPropertiesAsync(IdentityConfig azureAdConfig, TokenCredential credential)
    {
        var info = await GetIdentityInfoAsync(credential).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(azureAdConfig.TenantId))
        {
            azureAdConfig.TenantId = info.TenantId;
        }

        if (string.IsNullOrWhiteSpace(azureAdConfig.ClientId))
        {
            azureAdConfig.ClientId = info.ClientId;
        }

        if (string.IsNullOrWhiteSpace(azureAdConfig.AuthorityUri))
        {
            azureAdConfig.AuthorityUri = info.Authority ?? Defaults.AzureAdAuthorityUri;
        }
    }

    ///// <summary>
    ///// Builds a ConfidentialClientApplicationAndScopes from the specified config.
    ///// </summary>
    ///// <param name="config"></param>
    ///// <returns></returns>
    //public static async Task<ConfidentialClientApplicationToTokenProviderAdapter> BuildConfidentialClientApplicationAsync(
    //    IdentityConfigAndScopes config)
    //{
    //    var clientSecret = await KeyVaultHelper.GetSecretValueAsync(config.ClientSecretUri);

    //    var confidentialClientApplication = ConfidentialClientApplicationBuilder
    //        .Create(config.ClientId.ToString())
    //        .WithClientSecret(clientSecret.SecureStringToString())
    //        .WithAuthority(new Uri(config.AuthorityUri + config.TenantId))
    //        //.WithLegacyCacheCompatibility(false)
    //        .Build();
    //    var scopes = config.GetScopes();
    //    if (scopes == null || scopes.Length == 0)
    //    {
    //        scopes = IdentityHelper.GetAzureAdScopes();
    //    }

    //    var confidentialClientApplicationAndScopes =
    //        new ConfidentialClientApplicationToTokenProviderAdapter(confidentialClientApplication, scopes);
    //    return confidentialClientApplicationAndScopes;
    //}
}