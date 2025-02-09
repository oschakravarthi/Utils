using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace SubhadraSolutions.Utils.Security;

public static class CertificateHelper
{
    private static readonly List<Tuple<StoreName, StoreLocation>> storesToLook =
    [
        new(StoreName.My, StoreLocation.LocalMachine),
        new(StoreName.My, StoreLocation.CurrentUser),
        new(StoreName.TrustedPeople, StoreLocation.LocalMachine),
        new(StoreName.Root, StoreLocation.LocalMachine)
    ];

    public static X509Certificate2 BuildCertificateFromBase64EncodedCert(string base64EncodedCert)
    {
        if (string.IsNullOrWhiteSpace(base64EncodedCert))
        {
            return null;
        }

        var privateKeyBytes = Convert.FromBase64String(base64EncodedCert);
        return new X509Certificate2(privateKeyBytes, (string)null, X509KeyStorageFlags.MachineKeySet);
    }

    public static X509Certificate2 FindCertificateByValue(X509FindType findType, string value)
    {
        foreach (var storeInfo in storesToLook)
        {
            var cert = FindCertificateByValueInStore(findType, value, storeInfo.Item1, storeInfo.Item2);
            if (cert != null)
            {
                return cert;
            }
        }

        return null;
    }

    public static X509Certificate2Collection FindCertificatesByValue(X509FindType findType, string value)
    {
        var result = new X509Certificate2Collection();
        foreach (var storeInfo in storesToLook)
        {
            var certificates = FindCertificatesByValueInStore(findType, value, storeInfo.Item1, storeInfo.Item2);
            if (certificates.Count > 0)
            {
                result.AddRange(certificates);
            }
        }

        return result;
    }

    public static X509Certificate2 FindCertificateByValueInStore(X509FindType findType, string value, StoreName storeName, StoreLocation storeLocation)
    {
        var certificates = FindCertificatesByValueInStore(findType, value, storeName, storeLocation);
        //return certificates.Count > 0 ? certificates[0] : null;
        X509Certificate2 result = null;
        foreach (var certificate in certificates)
        {
            if (result == null || result.NotBefore < certificate.NotBefore)
            {
                result = certificate;
            }
        }

        return result;
    }

    public static X509Certificate2Collection FindCertificatesByValueInStore(X509FindType findType, string value, StoreName storeName, StoreLocation storeLocation)
    {
        using var store = new X509Store(storeName, storeLocation);
        store.Open(OpenFlags.ReadOnly);
        var certificates = store.Certificates.Find(findType, value, true);
        return certificates;
    }
}