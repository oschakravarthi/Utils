using Microsoft.AspNetCore.Components;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Blazor.Components
{
    public partial class PropertyGrid : AbstractSmartComponent
    {
        private class PropertyAndValue
        {
            public PropertyAndValue(string property, string value)
            {
                Property = property;
                Value = value;
            }

            public string Property { get; }
            public string Value { get; }
        }

        public PropertyGrid()
        {
            this.Outlined = true;
        }

        [Parameter]
        public object Object { get; set; }

        [Parameter]
        public string Title { get; set; }

        private List<PropertyAndValue> GetAsKeyValuePairs()
        {
            var obj = this.Object;
            if (obj == null)
            {
                return null;
            }
            var result = new List<PropertyAndValue>();
            var properties = ReflectionHelper.GetSortedPublicProperties(this.Object.GetType());
            foreach (var property in properties)
            {
                if (!CoreReflectionHelper.IsPrimitiveOrExtendedPrimitive(property.PropertyType))
                {
                    continue;
                }
                var name = StringHelper.BuildDisplayName(property.Name);
                var value = property.GetValue(obj);
                result.Add(new PropertyAndValue(name, Convert.ToString(value)));
            }

            return result;
        }
    }
}