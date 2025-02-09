using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Net.HeaderPropagation;

public class HeaderPropagationOptions
{
    public HashSet<string> HeaderNames { get; set; } = [];
}