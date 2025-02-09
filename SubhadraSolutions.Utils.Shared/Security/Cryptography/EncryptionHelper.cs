using System;
using System.Security.Cryptography;
using System.Text;

namespace SubhadraSolutions.Utils.Security.Cryptography;

public static class EncryptionHelper
{
    private const string CONST_DefaultPassphrase = "0123456789";

    public static string DecryptPasswordInString(string connectionString)
    {
        return DecryptPasswordInString(connectionString, "0123456789");
    }

    public static string DecryptPasswordInString(string connectionString, string passPhrase)
    {
        if (passPhrase == null)
        {
            throw new ArgumentNullException(nameof(passPhrase), "Passphrase cannot be null");
        }

        var result = connectionString;
        var text = connectionString.ToLower();
        var text2 = "password=";
        var value = ";";
        var num = text.IndexOf(text2);
        if (num <= 0)
        {
            text2 = "pwd=";
            num = text.IndexOf(text2);
        }

        if (num > 0)
        {
            num += text2.Length;
            var num2 = text.IndexOf(value, num);
            var text3 = connectionString.Substring(num, num2 - num);
            var newValue = DecryptString(text3, passPhrase);
            result = connectionString.Replace(text3, newValue);
        }

        return result;
    }

    public static string DecryptString(string encryptedString)
    {
        return DecryptString(encryptedString, "0123456789");
    }

    public static string DecryptString(string message, string passPhrase)
    {
        if (passPhrase == null)
        {
            throw new ArgumentNullException(nameof(passPhrase), "Passphrase cannot be null");
        }

        var uTF8Encoding = new UTF8Encoding();
        var mD5CryptoServiceProvider = MD5.Create();
        var key = mD5CryptoServiceProvider.ComputeHash(uTF8Encoding.GetBytes(passPhrase));
        var tripleDESCryptoServiceProvider = TripleDES.Create();
        tripleDESCryptoServiceProvider.Key = key;
        tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
        tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
        var array = Convert.FromBase64String(message);
        byte[] bytes;
        try
        {
            bytes = tripleDESCryptoServiceProvider.CreateDecryptor().TransformFinalBlock(array, 0, array.Length);
        }
        finally
        {
            tripleDESCryptoServiceProvider.Clear();
            mD5CryptoServiceProvider.Clear();
        }

        return uTF8Encoding.GetString(bytes);
    }
}