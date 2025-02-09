using System.IO;
using System.Security.Cryptography;

namespace SubhadraSolutions.Utils.Security.Cryptography;

public static class HashHelper
{
    public static string ComputeHash(this Stream stream, HashAlgorithm algorithm)
    {
        var hash = algorithm.ComputeHash(stream);
        return hash.ToFormattedString();
    }

    public static string ComputeHash(this Stream stream, HashAlgorithm algorithm, string format)
    {
        var hash = algorithm.ComputeHash(stream);
        return hash.ToFormattedString(format);
    }

    public static string ComputeSHA256Hash(this Stream stream)
    {
        HashAlgorithm sha256 = SHA256.Create();
        return ComputeHash(stream, sha256);
    }
}