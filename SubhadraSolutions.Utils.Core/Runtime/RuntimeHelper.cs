using System.Runtime.InteropServices;

namespace SubhadraSolutions.Utils.Runtime
{
    public static class RuntimeHelper
    {
        public static readonly bool IsBrowserApplication = RuntimeInformation.IsOSPlatform(OSPlatform.Create("Browser"));
    }
}