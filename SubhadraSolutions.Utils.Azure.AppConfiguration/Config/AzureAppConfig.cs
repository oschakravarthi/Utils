namespace SubhadraSolutions.Utils.Azure.AppConfiguration.Config
{
    public class AzureAppConfig
    {
        public const string DefaultSectionName = "AzureApp";

        public string ReloadKey { get; set; }
        public string Endpoint { get; set; }
        public string Label { get; set; }
        public string DefaultLabel { get; set; }
        public int RefreshTimeInSeconds { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(ReloadKey)
                && !string.IsNullOrEmpty(Endpoint)
                && !string.IsNullOrEmpty(Label)
                && !string.IsNullOrEmpty(DefaultLabel);
        }
    }
}