using SubhadraSolutions.Utils.Data.Annotations;
using System;

namespace SubhadraSolutions.Utils.Data.Metadata;

[Serializable]
public abstract class AbstractMetaRecord
{
    [NotInternable] public string Name { get; set; }

    public override string ToString()
    {
        return Name;
    }
}