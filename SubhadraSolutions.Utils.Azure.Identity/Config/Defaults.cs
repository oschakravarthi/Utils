namespace SubhadraSolutions.Utils.Azure.Identity.Config;

public static class Defaults
{
    public const string AzureAdAuthorityUri = "https://login.microsoftonline.com";
    public const string AzureAdScopes = "https://management.azure.com/.default";
    public const string AzureADTokenExchangeScopes = "api://AzureADTokenExchange/.default";

    public static string[] GetAzureAdScopes()
    {
        return IdentityHelper.GetScopes(AzureAdScopes);
    }

    public static string[] GetAzureADTokenExchangeScopes()
    {
        return IdentityHelper.GetScopes(AzureADTokenExchangeScopes);
    }
}