using System.Reflection;

namespace SubhadraSolutions.Utils.Reflection;

public static class ExecutingApplication
{
    public static string Name => Assembly.GetExecutingAssembly().FullName.Split(',')[0];
}