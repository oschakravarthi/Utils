using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Data;

public class DuplicatesSkipDataReaderDecorator : AbstractDataReaderDecorator
{
    private bool isInitializedAfterFirstRead;
    private List<int> ordinals;
    private string[] stringValues;
    public List<string> Fields { get; set; }

    public string TimestampColumnName { get; set; }

    public override bool Read()
    {
        while (Actual.Read())
        {
            if (!isInitializedAfterFirstRead)
            {
                InitializeAfterFirstRead();
                return true;
            }

            var duplicate = true;
            for (var i = 0; i < ordinals.Count; i++)
            {
                var ordinal = ordinals[i];
                var newString = Convert.ToString(Actual[ordinal]);
                if (!string.Equals(newString, stringValues[ordinal]))
                {
                    stringValues[ordinal] = newString;
                    duplicate = false;
                }
            }

            if (!duplicate)
            {
                return true;
            }
        }

        return false;
    }

    protected override void OnActualChanged()
    {
        ordinals = [];
        Fields = [];
        stringValues = new string[Actual.FieldCount];
        isInitializedAfterFirstRead = false;
    }

    private void InitializeAfterFirstRead()
    {
        if (Fields == null || Fields.Count == 0)
        {
            for (var i = 0; i < Actual.FieldCount; i++)
                if (GetName(i) != TimestampColumnName)
                {
                    ordinals.Add(i);
                }
        }
        else
        {
            for (var i = 0; i < Fields.Count; i++)
            {
                var ordinal = Actual.GetOrdinal(Fields[i]);
                if (ordinal > -1)
                {
                    ordinals.Add(ordinal);
                }
            }
        }

        for (var i = 0; i < ordinals.Count; i++)
        {
            var ordinal = ordinals[i];
            var newString = Convert.ToString(Actual[ordinal]);
            stringValues[ordinal] = newString;
        }

        isInitializedAfterFirstRead = true;
    }
}