namespace SubhadraSolutions.Utils.MicrosoftArtifactRegistry;

public class ContainerImageManifest(string architecture, string osVersion)
{
    public string Architecture { get; } = architecture;

    public string OSVersion { get; } = osVersion;
}