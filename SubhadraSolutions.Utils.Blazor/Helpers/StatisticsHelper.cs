//using Microsoft.RAM.Blazor.Components;
//using SubhadraSolutions.Utils;
//using SubhadraSolutions.Utils.Data.Annotations;
//using SubhadraSolutions.Utils.Reflection;
//using System;
//using System.Collections.Generic;
//using System.ComponentModel.DataAnnotations;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Microsoft.RAM.Blazor.Helpers
//{
//    public static class StatisticsHelper
//    {
//        public static IEnumerable<StatisticsInfo> GetStatistics(object o, AttributesLookup attributesLookup, object[] formatArgs)
//        {
//            var type = o.GetType();
//            if(attributesLookup==null)
//            {
//                attributesLookup = new AttributesLookup(type);
//            }
//            var properties = ReflectionHelper.GetSortedPublicProperties(type, attributesLookup);
//            foreach (var property in properties)
//            {
//                var kpiAttribute = attributesLookup.GetCustomAttribute<KPIAttribute>(property, true);
//                if (kpiAttribute == null)
//                {
//                    continue;
//                }

//                var value = property.GetValue(o, null);
//                var displayAttribute = attributesLookup.GetCustomAttribute<DisplayAttribute>(property, true);
//                var info = new StatisticsInfo
//                {
//                    IsPercentage = kpiAttribute.IsPercentage,
//                    IsPositiveValuePositive = kpiAttribute.IsPositiveValuePositive,
//                    Value = value
//                };
//                if (displayAttribute == null)
//                {
//                    info.Title = property.Name.BuildDisplayName();
//                }
//                else
//                {
//                    info.Title = displayAttribute.Name;
//                    info.Subtitle = displayAttribute.ShortName;
//                    info.Documentation = displayAttribute.Description;
//                }
//            }
//        }
//    }
//}