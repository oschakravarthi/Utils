using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using SubhadraSolutions.Utils.Security;
using System;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Azure.Security;

public static class KeyVaultHelper
{
    private static readonly SecretClientOptions SECRET_CLIENT_OPTIONS = new()
    {
        Retry =
        {
            Delay = TimeSpan.FromSeconds(2),
            MaxDelay = TimeSpan.FromSeconds(16),
            MaxRetries = 5,
            Mode = RetryMode.Exponential
        }
    };

    /// <summary>
    ///     Gets the named secret from the CQE Key Vault.
    /// </summary>
    /// <param name="secretUri"></param>
    /// <returns></returns>
    public static async Task<SecureString> GetSecretValueAsync(string secretUri)
    {
        if (string.IsNullOrWhiteSpace(secretUri))
        {
            return null;
        }
        return await GetSecretValueAsync(secretUri, new DefaultAzureCredential()).ConfigureAwait(false);
    }

    public static async Task<SecureString> GetSecretValueAsync(string secretUri, TokenCredential tokenCredential)
    {
        if (string.IsNullOrWhiteSpace(secretUri))
        {
            return null;
        }
        var secret = await GetSecretValueCoreAsync(secretUri, tokenCredential, "/secrets/").ConfigureAwait(false);
        return secret.ConvertToSecureString();
    }

    public static async Task<X509Certificate2> GetCertificateFromKeyVaultAsync(string secretUri)
    {
        if (string.IsNullOrWhiteSpace(secretUri))
        {
            return null;
        }
        return await GetCertificateFromKeyVaultAsync(secretUri, new DefaultAzureCredential()).ConfigureAwait(false);
    }

    public static async Task<X509Certificate2> GetCertificateFromKeyVaultAsync(string secretUri,
        TokenCredential tokenCredential)
    {
        if (string.IsNullOrWhiteSpace(secretUri))
        {
            return null;
        }
        var secret = await GetSecretValueCoreAsync(secretUri, tokenCredential, "/secrets/").ConfigureAwait(false);
        var certificate = CertificateHelper.BuildCertificateFromBase64EncodedCert(secret);
        return certificate;
    }

    private static async Task<string> GetSecretValueCoreAsync(string secretUri, TokenCredential tokenCredential,
        string typePart)
    {
        if (string.IsNullOrWhiteSpace(secretUri))
        {
            return null;
        }
        string pattern = @"https:\/\/[a-zA-Z0-9\-_]+\.vault\.azure\.net\/secrets\/[a-zA-Z0-9\-_/]+(\/[a-zA-Z0-9\-_]+)?";
        Regex regex = new Regex(pattern);
        if (!regex.IsMatch(secretUri))
        {
            return secretUri;
        }

        var pair = GetKeyVaultUriAndSecretName(secretUri, typePart);
        var keyVaultUri = pair.Item1;
        var name = pair.Item2;
        var secretClient = new SecretClient(new Uri(keyVaultUri), tokenCredential, SECRET_CLIENT_OPTIONS);

        var secretValue = await GetSecretValueCoreAsync(name, secretClient).ConfigureAwait(false);
        return secretValue;
    }

    private static async Task<string> GetSecretValueCoreAsync(string name, SecretClient secretClient)
    {
        KeyVaultSecret secret = await secretClient.GetSecretAsync(name).ConfigureAwait(false);
        return secret.Value;
    }

    public static async Task<SecureString> GetSecretValueAsync(string name, SecretClient secretClient)
    {
        name = FixName(name, "/secrets/");

        var secretValue = await GetSecretValueCoreAsync(name, secretClient).ConfigureAwait(false);
        return secretValue.ConvertToSecureString();
    }

    private static Tuple<string, string> GetKeyVaultUriAndSecretName(string url, string typePart)
    {
        var uri = new Uri(url);
        var keyVaultUri = uri.AbsoluteUri;
        var name = uri.AbsolutePath;
        keyVaultUri = keyVaultUri.Substring(0, keyVaultUri.Length - name.Length);
        name = FixName(name, typePart);
        return new Tuple<string, string>(keyVaultUri, name);
    }

    private static string FixName(string name, string typePart)
    {
        if (name.StartsWith(typePart, StringComparison.OrdinalIgnoreCase))
        {
            name = name.Substring(typePart.Length);
        }

        name = name.TrimEnd('/');

        return name;
    }
}