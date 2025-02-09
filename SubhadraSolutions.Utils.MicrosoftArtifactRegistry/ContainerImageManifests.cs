using System.Collections.Generic;

namespace SubhadraSolutions.Utils.MicrosoftArtifactRegistry
{
    public class ContainerImageManifests
    {
        public ContainerImageManifests(string uri)
        {
            Uri = uri;
        }

        public string Uri { get; private set; }
        public List<ContainerImageManifest> Manifests { get; private set; } = [];
    }
}