using System;

#if DEBUG

using Kusto.Cloud.Platform.Utils;

#endif

namespace SubhadraSolutions.Utils.Kusto.Server.Linq.QueryParts;

internal class ParameterQueryPart : AbstractQueryPart
{
#if DEBUG
    private readonly string stringValue;

    public ParameterQueryPart(string value, QueryContext context)
    {
        stringValue = value.Escape();
        if (stringValue != null)
        {
            stringValue = "\"" + stringValue + "\"";
        }
    }

    public ParameterQueryPart(TimeSpan value, QueryContext context)
    {
        stringValue = $"totimespan('{value.ToString()}')";
        //this.value = string.Format("time(\"{0}\")", value.ToString("c"));
    }

    public ParameterQueryPart(bool value, QueryContext context)
    {
        stringValue = value.ToString();
    }

    public ParameterQueryPart(DateTime value, QueryContext context)
    {
        if (value.Kind == DateTimeKind.Unspecified)
        {
            value = new DateTime(value.Ticks, DateTimeKind.Utc);
        }

        var valueString = value.FastToString();
        stringValue = $"datetime('{valueString}')";
    }

    public ParameterQueryPart(double value, QueryContext context)
    {
        stringValue = value.ToString();
        if (stringValue.LastIndexOf('.') < 0)
        {
            stringValue += ".0";
        }
    }

    public ParameterQueryPart(Guid value, QueryContext context)
    {
        stringValue = "\"" + value + "\"";
    }

    public ParameterQueryPart(char value, QueryContext context)
    {
        stringValue = value.ToString().Escape();
        stringValue = "'" + stringValue + "'";
    }

    public ParameterQueryPart(int value, QueryContext context)
    {
        stringValue = value.ToString();
    }

    public ParameterQueryPart(long value, QueryContext context)
    {
        stringValue = value.ToString();
    }

    public override string GetQuery()
    {
        return stringValue;
    }

#else
        private readonly string parameterName;

        public ParameterQueryPart(string value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(TimeSpan value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(bool value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(DateTime value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(double value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(Guid value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }
        public ParameterQueryPart(char value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(int value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public ParameterQueryPart(long value, QueryContext context)
        {
            this.parameterName = context.AddParameter(value);
        }

        public override string GetQuery()
        {
            return this.parameterName;
        }
#endif
}