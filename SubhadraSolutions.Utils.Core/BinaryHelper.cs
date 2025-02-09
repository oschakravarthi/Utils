using System.Runtime.InteropServices;
using System.Text;

namespace SubhadraSolutions.Utils;

public static class BinaryHelper
{
    /// <summary>
    /// Checks whether the contents of the given two byte arrays are equal.
    /// </summary>
    /// <param name="b1">The first byte array</param>
    /// <param name="b2">The second byte array</param>
    /// <returns>True if the two byte arrays have the same contents, false if they don't.</returns>
    public static bool Equals(byte[] b1, byte[] b2)
    {
        // Validate buffers are the same length.
        // This also ensures that the count does not exceed the length of either buffer.
        if (b1.Length != b2.Length)
        {
            return false;
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return memcmp(b1, b2, b1.Length) == 0;
        }

        var length = b1.Length;
        for (var i = 0; i < length; i++)
            if (b1[i] != b2[i])
            {
                return false;
            }

        return true;
    }

    /// <summary>
    /// Convert a byte array to a formatted string.
    /// </summary>
    /// <param name="bytes"></param>
    /// <param name="format"></param>
    /// <returns></returns>
    public static string ToFormattedString(this byte[] bytes, string format)
    {
        var result = new StringBuilder();
        for (var i = 0; i < bytes.LongLength; i++) result.Append(bytes[i].ToString(format));
        return result.ToString();
    }

    /// <summary>
    /// Convert a byte array to a formatted string.
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static string ToFormattedString(this byte[] bytes)
    {
        return ToFormattedString(bytes, "X2");
    }

    [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern int memcmp(byte[] b1, byte[] b2, long count);
}