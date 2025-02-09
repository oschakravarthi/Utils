using System;

namespace SubhadraSolutions.Utils.Data.Metadata;

public class DatabaseMetaRecord : AbstractMetaRecord, IEquatable<DatabaseMetaRecord>, IComparable<DatabaseMetaRecord>
{
    public string Cluster { get; set; }

    public int CompareTo(DatabaseMetaRecord other)
    {
        if (other == null)
        {
            return 1;
        }

        var result = string.Compare(Cluster, other.Cluster);
        if (result != 0)
        {
            return result;
        }

        return string.Compare(Name, other.Name);
    }

    public bool Equals(DatabaseMetaRecord other)
    {
        return CompareTo(other) == 0;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as DatabaseMetaRecord);
    }

    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }

    public override string ToString()
    {
        return this.ExportObjectAsString();
    }
}