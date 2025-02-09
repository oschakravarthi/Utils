using System;
using System.Runtime.InteropServices;

namespace SubhadraSolutions.Utils.Runtime.InteropServices;

public static class InteropServicesHelper
{
    public static void EnsurePlatform(OSPlatform platform)
    {
        var isRequiredPlatform = RuntimeInformation.IsOSPlatform(platform);
        if (!isRequiredPlatform)
        {
            throw new NotSupportedException($"This operation is allowed only in {platform} platform.");
        }
    }

    public static void EnsureWindowsPlatform()
    {
        EnsurePlatform(OSPlatform.Windows);
    }
}