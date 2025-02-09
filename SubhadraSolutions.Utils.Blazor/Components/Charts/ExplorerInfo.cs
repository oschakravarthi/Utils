using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components.Charts;

public class ExplorerInfo(Type explorerPageType)
{
    public Type ExplorerPageType { get; } = explorerPageType;
    public Dictionary<string, object> QueryParameters { get; set; } = [];
}