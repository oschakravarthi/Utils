using System;

namespace SubhadraSolutions.Utils.Data.Common;

public struct FieldNameAndType(string fieldName, Type fieldType)
{
    public string FieldName { get; } = fieldName;
    public Type FieldType { get; } = fieldType;
}