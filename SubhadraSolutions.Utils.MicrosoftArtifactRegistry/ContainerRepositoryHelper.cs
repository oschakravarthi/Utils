using Newtonsoft.Json.Linq;
using SubhadraSolutions.Utils.Net.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.MicrosoftArtifactRegistry;

public static class ContainerRepositoryHelper
{
    public const string ContainerApiEndpointDefaultUrl = "https://mcr.microsoft.com/v2";

    private static readonly MediaTypeWithQualityHeaderValue ManifestHeaderV1 =
        new("application/vnd.docker.distribution.manifest.v1+json");

    private static readonly MediaTypeWithQualityHeaderValue ManifestListHeaderV2 =
        new("application/vnd.docker.distribution.manifest.list.v2+json");

    public static async Task<ContainerImageManifests> GetContainerImageManifestsAsync(string repository,
        string tag, IHttpClient httpClient, string containerApiEndpointUrl = ContainerApiEndpointDefaultUrl)
    {
        if (string.IsNullOrWhiteSpace(containerApiEndpointUrl))
        {
            containerApiEndpointUrl = ContainerApiEndpointDefaultUrl;
        }
        else
        {
            containerApiEndpointUrl = containerApiEndpointUrl.TrimEnd('/');
        }

        ServicePointManager.CheckCertificateRevocationList = true;
        if (repository == null)
        {
            throw new ArgumentException($"{nameof(repository)} should not be null", nameof(repository));
        }

        if (tag == null)
        {
            throw new ArgumentException($"{nameof(tag)} should not be null", nameof(tag));
        }

        // Initialize the HttpClient in a synchronized block to avoid rare infinite loop condition in System.Net.Http.Headers.HttpHeaders.ContainsParsedValue
        var relative = WebUtility.UrlEncode($"{repository}/manifests/{tag}");
        var uri = $"{containerApiEndpointUrl}/{relative}";
        var result = new ContainerImageManifests(uri);

        try
        {
            var responseManifestList = await GetAsync(uri, httpClient, ManifestListHeaderV2).ConfigureAwait(false);
            using (responseManifestList)
            {
                var imageManifest = await responseManifestList.Content.ReadJObjectAsync().ConfigureAwait(false);

                if (responseManifestList.Content.Headers.ContentType.ToString().Contains("manifest.list.v2"))
                {
                    var manifests = imageManifest["manifests"];
                    if (manifests?.HasValues == true)
                    {
                        foreach (var architecture in manifests)
                        {
                            var platform = architecture["platform"];
                            var arch = platform["architecture"].ToString();
                            var osVersion = platform["os.version"].ToString();
                            result.Manifests.Add(new ContainerImageManifest(arch, osVersion));
                        }
                    }
                    else
                    {
                        throw new ValidationException("Invalid manifest format");
                    }
                }
                else
                {
                    if (responseManifestList.Content.Headers.ContentType.ToString().Contains("manifest.v1"))
                    {
                        var info = ParseManifestv1(imageManifest, repository, tag);
                        if (info != null)
                        {
                            result.Manifests.Add(info);
                        }
                    }
                }
            }
        }
        catch (HttpRequestException requestException)
        {
            if (requestException.StatusCode == HttpStatusCode.NotFound)
            {
                // In case the registry does not support 'manifest.list.v2' , try falling back to 'manifest.v1'
                try
                {
                    var responseManifestImage = await GetAsync(uri, httpClient, ManifestHeaderV1).ConfigureAwait(false);
                    using (responseManifestImage)
                    {
                        var imageManifest = await responseManifestImage.Content.ReadJObjectAsync().ConfigureAwait(false);

                        var info = ParseManifestv1(imageManifest, repository, tag);
                        if (info != null)
                        {
                            result.Manifests.Add(info);
                        }
                    }
                }
                catch (HttpRequestException ex)
                {
                    if (ex.StatusCode != HttpStatusCode.NotFound)
                    {
                        throw;
                    }
                }
            }
            else
            {
                throw;
            }
        }

        return result;
    }

    private static async Task<HttpResponseMessage> GetAsync(string uri, IHttpClient httpClient,
        MediaTypeWithQualityHeaderValue header)
    {
        var requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
        requestMessage.Headers.Accept.Add(header);

        return await httpClient.SendAsync(requestMessage).ConfigureAwait(false);
    }

    private static ContainerImageManifest ParseManifestv1(JObject imageManifest, string repository, string tag)
    {
        var history = imageManifest["history"];
        if (history?.HasValues == true && history[0] != null)
        {
            var temp = history[0]["v1Compatibility"] as JValue;
            var imageManifestv1Compatibility = JToken.Parse(temp.Value as string);
            if (imageManifestv1Compatibility != null)
            {
                var architecture = imageManifestv1Compatibility["architecture"].ToString();
                var osVersion = imageManifestv1Compatibility["os.version"].ToString();

                return new ContainerImageManifest(architecture, osVersion);
            }

            return null;
        }

        throw new ValidationException("Invalid manifest format");
    }
}