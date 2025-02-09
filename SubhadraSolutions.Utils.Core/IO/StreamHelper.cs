using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.IO
{
    public static class StreamHelper
    {
        public static string ReadAsString(this Stream stream)
        {
            return ReadAsString(stream, Encoding.UTF8);
        }

        public static string ReadAsString(this Stream stream, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return reader.ReadToEnd();
            }
        }

        public static Task<string> ReadAsStringAsync(this Stream stream)
        {
            return stream.ReadAsStringAsync(Encoding.UTF8);
        }

        public static async Task<string> ReadAsStringAsync(this Stream stream, Encoding encoding)
        {
            using (StreamReader reader = new StreamReader(stream, encoding))
            {
                return await reader.ReadToEndAsync().ConfigureAwait(false);
            }
        }
    }
}