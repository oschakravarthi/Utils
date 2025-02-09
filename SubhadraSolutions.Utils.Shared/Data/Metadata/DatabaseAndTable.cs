using System;

namespace SubhadraSolutions.Utils.Data.Metadata;

[Serializable]
public class DatabaseAndTable : IEquatable<DatabaseAndTable>, IComparable<DatabaseAndTable>
{
    //public string ClusterName { get; set; }
    public string DatabaseName { get; set; }

    public string TableName { get; set; }

    public int CompareTo(DatabaseAndTable other)
    {
        if (other == null)
        {
            return 1;
        }

        //var result = string.Compare(ClusterName, other.ClusterName);
        //if (result != 0)
        //{
        //    return result;
        //}

        var result = string.Compare(DatabaseName, other.DatabaseName);
        if (result != 0)
        {
            return result;
        }

        return string.Compare(TableName, other.TableName);
    }

    public bool Equals(DatabaseAndTable other)
    {
        return CompareTo(other) == 0;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as DatabaseAndTable);
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