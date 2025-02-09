namespace SubhadraSolutions.Utils.Net.Http;

public static class HttpClientExtensions
{
#if NETFRAMEWORK
        public static async Task<Stream> GetStreamAsync(this HttpClient httpClient, string url, CancellationToken cancellationToken)
        {
            var response = await httpClient.GetAsync(new Uri(url), cancellationToken);
            return await response.Content.ReadAsStreamAsync();
        }
        public static async Task<string> ReadAsStringAsync(this HttpContent content, CancellationToken cancellationToken)
        {
            var response = await content.ReadAsStringAsync();
            return response;
        }
        public static async Task<byte[]> ReadAsByteArrayAsync(this HttpContent content, CancellationToken cancellationToken)
        {
            var response = await content.ReadAsByteArrayAsync();
            return response;
        }
#endif
}