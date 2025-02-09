using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Data;

public static class DimensionsAndMeasuresHelper
{
    public static IEnumerable<AggregationType> GetAllowedAttributeTypes(PropertyInfo property,
        AttributesLookup attributeLookup, bool isDimension)
    {
        MeasureAttribute measureAttribute = null;
        if (attributeLookup != null)
        {
            measureAttribute = attributeLookup.GetCustomAttribute<MeasureAttribute>(property, true);
        }
        else
        {
            property.GetCustomAttribute<MeasureAttribute>(true);
        }

        if (isDimension)
        {
            if (measureAttribute == null)
            {
                //measures.Add(new(property.Name, AggregationType.Count));
                yield return AggregationType.DistinctCount;
            }
            else
            {
                var agg = measureAttribute.AggregationTypes;

                //if ((agg & AggregationTypes.Count) != 0)
                //{
                //    measures.Add(new(property.Name, AggregationType.Count));
                //}

                if ((agg & AggregationTypes.DistinctCount) != 0)
                {
                    yield return AggregationType.DistinctCount;
                }
            }
        }
        else
        {
            if (measureAttribute == null)
            {
                foreach (var value in Enum.GetValues(typeof(AggregationType)))
                {
                    yield return (AggregationType)value;
                }
            }
            else
            {
                var agg = measureAttribute.AggregationTypes;

                if ((agg & AggregationTypes.Average) != 0)
                {
                    yield return AggregationType.Average;
                }

                //if ((agg & AggregationTypes.Count) != 0)
                //{
                //    measures.Add(new(property.Name, AggregationType.Count));
                //}

                if ((agg & AggregationTypes.Max) != 0)
                {
                    yield return AggregationType.Max;
                }

                if ((agg & AggregationTypes.Min) != 0)
                {
                    yield return AggregationType.Min;
                }

                if ((agg & AggregationTypes.Sum) != 0)
                {
                    yield return AggregationType.Sum;
                }

                if ((agg & AggregationTypes.DistinctCount) != 0)
                {
                    yield return AggregationType.DistinctCount;
                }
            }
        }
    }

    public static KeyValuePair<List<string>, List<T>> GetCleanedDimensionsAndMeasures<T>(Type entityType,
        Func<PropertyInfo, AggregationType, T> measureInstantiator, bool autoCalculateDimensionsAndMeasures,
        bool includeDistinctCountOfDimensions, IReadOnlyCollection<string> propertiesToExclude = null,
        IReadOnlyCollection<string> knownDimensions = null, IReadOnlyCollection<T> knownMeasures = null,
        AttributesLookup attributesLookup = null) where T : IMeasure
    {
        var pair = GetDimensionsAndMeasures(entityType, measureInstantiator, includeDistinctCountOfDimensions,
            propertiesToExclude, knownDimensions, attributesLookup);
        var autoCalculatedDimensions = false;

        List<string> dimensions = null;
        List<T> measures = null;
        if (knownDimensions == null || knownDimensions.Count == 0)
        {
            autoCalculatedDimensions = true;
            dimensions = pair.Key.Select(p => p.Name).ToList();
            dimensions.Sort();
        }
        else
        {
            dimensions = knownDimensions.ToList();
            dimensions.Sort();
            if (autoCalculateDimensionsAndMeasures)
            {
                var dims = pair.Key;

                foreach (var dim in dims)
                {
                    if (!dimensions.Contains(dim.Name))
                    {
                        dimensions.Add(dim.Name);
                    }
                }
            }
        }

        if (knownMeasures == null || knownMeasures.Count == 0)
        {
            measures = pair.Value.ToList();

            if (!autoCalculatedDimensions)
            {
                for (var i = 0; i < measures.Count; i++)
                    if (dimensions.Contains(measures[i].PropertyName))
                    {
                        measures.RemoveAt(i);
                        i--;
                    }
            }
        }
        else
        {
            measures = knownMeasures.ToList();
            if (autoCalculateDimensionsAndMeasures)
            {
                foreach (var calculatedMeasure in pair.Value)
                {
                    if (!measures.Contains(calculatedMeasure) && !dimensions.Contains(calculatedMeasure.PropertyName))
                    {
                        measures.Add(calculatedMeasure);
                    }
                }
            }
        }

        return new KeyValuePair<List<string>, List<T>>(dimensions, measures);
    }

    public static KeyValuePair<HashSet<PropertyInfo>, HashSet<T>> GetDimensionsAndMeasures<T>(Type entityType,
        Func<PropertyInfo, AggregationType, T> measureInstantiator, bool includeDistinctCountOfDimensions,
        IReadOnlyCollection<string> propertiesToExclude = null, IReadOnlyCollection<string> knownDimensions = null,
        AttributesLookup attributesLookup = null)
    {
        if (attributesLookup == null)
        {
            attributesLookup = new AttributesLookup(entityType);
        }

        var pair = GetDimensionsAndMeasuresProperties(entityType, propertiesToExclude, knownDimensions,
            attributesLookup);
        var dimensions = pair.Key;
        var measures = new HashSet<T>();

        CopyMeasures(pair.Value, measureInstantiator, attributesLookup, measures, false);
        if (includeDistinctCountOfDimensions)
        {
            CopyMeasures(pair.Key, measureInstantiator, attributesLookup, measures, true);
        }

        return new KeyValuePair<HashSet<PropertyInfo>, HashSet<T>>(dimensions, measures);
    }

    public static KeyValuePair<HashSet<PropertyInfo>, HashSet<PropertyInfo>> GetDimensionsAndMeasuresProperties(
        Type entityType, IReadOnlyCollection<string> propertiesToExclude = null,
        IReadOnlyCollection<string> knownDimensions = null, AttributesLookup attributesLookup = null)
    {
        if (entityType == null)
        {
        }

        var dimensions = new HashSet<PropertyInfo>();
        var measures = new HashSet<PropertyInfo>();
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.PropertyType.IsPrimitiveOrExtendedPrimitive());
        var hashSet = properties.ToHashSet();
        if (attributesLookup == null)
        {
            attributesLookup = new AttributesLookup(entityType);
        }

        // PropertyInfo[] properties = entityType.GetProperties();
        foreach (var property in properties)
        {
            var linkedPropertyAttributes = attributesLookup
                .GetCustomAttributes(property, typeof(LinkedPropertyAttribute), true).ToList();

            for (var i = 0; i < linkedPropertyAttributes.Count; i++)
            {
                var linkedPropertyAttribute = (LinkedPropertyAttribute)linkedPropertyAttributes[i];
                var linkedPropertyName = linkedPropertyAttribute.LinkedPropertyName;
                var linkedProperty = entityType.GetProperty(linkedPropertyName);
                hashSet.Remove(linkedProperty);
            }
        }

        foreach (var property in hashSet)
        {
            var propertyName = property.Name;
            if (propertiesToExclude?.Contains(propertyName) == true)
            {
                continue;
            }

            if (knownDimensions?.Contains(propertyName) == true)
            {
                dimensions.Add(property);
                continue;
            }

            if (property.PropertyType.IsNumericType())
            {
                if (!attributesLookup.IsDefined<NotMeasureAttribute>(property, true))
                {
                    measures.Add(property);
                    continue;
                }
            }

            if (!attributesLookup.IsDefined<NotDimensionAttribute>(property, true))
            {
                dimensions.Add(property);
            }
        }

        return new KeyValuePair<HashSet<PropertyInfo>, HashSet<PropertyInfo>>(dimensions, measures);
    }

    private static void CopyMeasures<T>(IEnumerable<PropertyInfo> properties,
        Func<PropertyInfo, AggregationType, T> measureInstantiator, AttributesLookup attributesLookup,
        ICollection<T> target, bool isDimension)
    {
        foreach (var property in properties)
        {
            var allowedAggregateTypes = GetAllowedAttributeTypes(property, attributesLookup, isDimension);
            foreach (var allowedAggregateType in allowedAggregateTypes)
            {
                target.Add(measureInstantiator(property, allowedAggregateType));
            }
        }
    }
}