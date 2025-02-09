using System;
using System.Net;
using System.Security;

namespace SubhadraSolutions.Utils;

public static class SecureStringHelper
{
    public static string SecureStringToString(this SecureString secureString)
    {
        return new NetworkCredential(string.Empty, secureString).Password;
    }

    public static SecureString ConvertToSecureString(this string s)
    {
        if (s == null)
        {
            throw new ArgumentNullException(nameof(s));
        }

        unsafe
        {
            fixed (char* psz = s)
            {
                var secureString = new SecureString(psz, s.Length);
                secureString.MakeReadOnly();
                return secureString;
            }
        }
    }

    public static bool IsNullOrEmpty(this SecureString s)
    {
        return s == null || s.Length == 0;
    }
}