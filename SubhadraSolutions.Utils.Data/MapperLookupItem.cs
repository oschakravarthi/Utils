using System;
using System.Data;
using System.Text;

namespace SubhadraSolutions.Utils.Data;

internal class MapperLookupItem : IEquatable<MapperLookupItem>
{
    private readonly string[] fields;
    private readonly Type[] types;

    public MapperLookupItem(IDataRecord record)
    {
        var fieldCount = record.FieldCount;
        fields = new string[fieldCount];
        types = new Type[fieldCount];

        for (var i = 0; i < fieldCount; i++)
        {
            fields[i] = record.GetName(i);
            types[i] = record.GetFieldType(i);
        }
    }

    public bool Equals(MapperLookupItem other)
    {
        if (fields.Length != other.fields.Length)
        {
            return false;
        }

        for (var i = 0; i < fields.Length; i++)
        {
            if (!string.Equals(fields[i], other.fields[i], StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            if (types[i] != other.types[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as MapperLookupItem);
    }

    public override int GetHashCode()
    {
        var count = fields.Length;
        var sb = new StringBuilder();

        for (var i = 0; i < count; i++) sb.Append(fields[i] + ",");

        return sb.ToString().GetHashCode();
    }
}