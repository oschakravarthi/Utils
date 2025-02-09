using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

public sealed class AnonymousListContainer
{
    public IList List { get; set; }
    public List<Tuple<string, Type>> Properties { get; set; } = [];
}