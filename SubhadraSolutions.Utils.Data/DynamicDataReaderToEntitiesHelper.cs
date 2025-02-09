using SubhadraSolutions.Utils.CodeContracts.Annotations;
using SubhadraSolutions.Utils.Collections;
using SubhadraSolutions.Utils.Data.Annotations;
using SubhadraSolutions.Utils.Reflection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Data;

public static class DynamicDataReaderToEntitiesHelper<T>
{
    private static readonly ConcurrentDictionary<MapperLookupItem, Func<IDataRecord, T>> lookup = new();

    public static Func<IDataRecord, T> BuildMapper(IDataRecord record, AttributesLookup attributesLookup = null)
    {
        var lookupItem = new MapperLookupItem(record);
        return lookup.GetOrAdd(lookupItem, key => BuildMapperCore(record, attributesLookup));
    }

    public static IEnumerable<T> MapToEntities(IDataReader reader)
    {
        var mapper = BuildMapper(reader);

        while (reader.Read()) yield return mapper(reader);
    }

    [DynamicallyInvoked]
    public static List<T> MapToEntitiesList(IDataReader reader)
    {
        var enumerable = MapToEntities(reader);
        return new List<T>(enumerable);
    }

    [ILEmitter]
    private static Func<IDataRecord, T> BuildCreateEntityFromRecordDynamicMethod(IDataRecord record,
        ConstructorInfo constructor, int[] ordinals)
    {
        var tto = typeof(T);

        var dm = new DynamicMethod("createEntityFromRecordDynamicMethod" + GeneralHelper.Identity, tto,
            [typeof(IDataRecord)], typeof(DynamicDataReaderToEntitiesHelper<T>));
        var ilGen = dm.GetILGenerator();
        var parameters = constructor.GetParameters();
        var paremetersCount = parameters.Length;

        var parameterLocals = new LocalBuilder[paremetersCount];

        for (var i = 0; i < paremetersCount; i++)
        {
            var parameterType = parameters[i].ParameterType;
            var parameterLocal = ilGen.DeclareLocal(parameterType);

            parameterLocals[i] = parameterLocal;
        }

        for (var i = 0; i < paremetersCount; i++)
        {
            var isNullableType = false;
            var parameterLocal = parameterLocals[i];
            EmitHelper.InitializeLocal(parameterLocal, ilGen);

            var parameter = parameters[i];
            var parameterType = parameter.ParameterType;

            if (parameterType.IsGenericType && parameterType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullableType = true;
                parameterType = parameterType.GetGenericArguments()[0];
            }

            var labelInner = ilGen.DefineLabel();

            var ordinal = ordinals[i];

            if (ordinal < 0)
            {
                ilGen.Emit(OpCodes.Br, labelInner);
            }
            else
            {
                var fieldType = record.GetFieldType(ordinal);
                EmitILForDbNullCheck(ordinal, ilGen, labelInner);
                if (isNullableType)
                {
                    ilGen.Emit(OpCodes.Ldloca, parameterLocal);
                }

                EmitILForReadingFromReaderAndConvertingToTheRequiredType(ordinal, fieldType, parameterType, ilGen);
                if (isNullableType)
                {
                    ilGen.Emit(OpCodes.Call, parameterLocal.LocalType.GetConstructors()[0]);
                }
                else
                {
                    ilGen.Emit(OpCodes.Stloc, parameterLocal);
                }

                ilGen.Emit(OpCodes.Br, labelInner);

                ilGen.MarkLabel(labelInner);
            }
        }

        var entityLocal = ilGen.DeclareLocal(tto);
        for (var i = 0; i < paremetersCount; i++) ilGen.Emit(OpCodes.Ldloc, parameterLocals[i]);
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Stloc, entityLocal);

        ilGen.Emit(OpCodes.Ldloc, entityLocal);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<IDataRecord, T>>();
    }

    [ILEmitter]
    private static Func<IDataRecord, T> BuildCreateEntityFromRecordDynamicMethod(IDataRecord record,
        PropertyInfo[] properties, int[] ordinals, AttributesLookup attributesLookup)
    {
        var tto = typeof(T);

        var propertiesCount = properties.Length;
        var dm = new DynamicMethod("createEntityFromRecordDynamicMethod" + GeneralHelper.Identity, tto,
            [typeof(IDataRecord)], typeof(DynamicDataReaderToEntitiesHelper<T>));
        var ilGen = dm.GetILGenerator();
        var entityLocal = ilGen.DeclareLocal(tto);
        var constructor = tto.GetConstructor(Type.EmptyTypes);
        ilGen.Emit(OpCodes.Newobj, constructor);
        ilGen.Emit(OpCodes.Stloc, entityLocal);

        FieldInfo[] fields = null;
        if (GeneralHelper.IsNetCore())
        {
            fields = new FieldInfo[propertiesCount];
        }

        for (var i = 0; i < propertiesCount; i++)
        {
            LocalBuilder local = null;
            var property = properties[i];
            var propertyType = property.PropertyType;

            if (!property.CanWrite)
            {
                continue;
            }

            if (fields != null)
            {
                fields[i] = tto.GetField($"<{property.Name}>k__BackingField",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            }

            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                local = ilGen.DeclareLocal(propertyType);
                propertyType = propertyType.GetGenericArguments()[0];
            }

            var labelInner = ilGen.DefineLabel();

            var ordinal = ordinals[i];

            if (ordinal > -1)
            {
                var fieldType = record.GetFieldType(ordinal);
                EmitILForDbNullCheck(ordinal, ilGen, labelInner);

                ilGen.Emit(OpCodes.Ldloc, entityLocal);

                if (local != null)
                {
                    ilGen.Emit(OpCodes.Ldloca, local);
                }

                EmitILForReadingFromReaderAndConvertingToTheRequiredType(ordinal, fieldType, propertyType, ilGen);

                if (local != null)
                {
                    ilGen.Emit(OpCodes.Call, local.LocalType.GetConstructors()[0]);
                    ilGen.Emit(OpCodes.Ldloc, local);
                }

                EmitILForSorting(property, ilGen, attributesLookup);

                FieldInfo field = null;
                if (fields != null)
                {
                    field = fields[i];
                }

                if (field != null)
                {
                    ilGen.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    ilGen.Emit(OpCodes.Callvirt, property.SetMethod);
                }

                ilGen.MarkLabel(labelInner);
            }
        }

        ilGen.Emit(OpCodes.Ldloc, entityLocal);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<IDataRecord, T>>();
    }

    [ILEmitter]
    private static Func<IDataRecord, T> BuildCreateScalarEntityFromRecordDynamicMethod(IDataRecord record)
    {
        var toType = typeof(T);
        var dm = new DynamicMethod("createScalarEntityFromRecordDynamicMethod" + GeneralHelper.Identity, toType,
            [typeof(IDataRecord)], typeof(DynamicDataReaderToEntitiesHelper<T>));
        var ilGen = dm.GetILGenerator();

        var entityLocal = ilGen.DeclareLocal(toType);
        EmitHelper.InitializeLocal(entityLocal, ilGen);

        var labelInner = ilGen.DefineLabel();
        const int ordinal = 0;
        var isNullableType = false;
        if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            isNullableType = true;
            toType = toType.GetGenericArguments()[0];
        }

        EmitILForDbNullCheck(ordinal, ilGen, labelInner);

        var fieldType = record.GetFieldType(0);
        if (isNullableType)
        {
            ilGen.Emit(OpCodes.Ldloca, entityLocal);
        }

        EmitILForReadingFromReaderAndConvertingToTheRequiredType(ordinal, fieldType, toType, ilGen);
        if (isNullableType)
        {
            ilGen.Emit(OpCodes.Call, entityLocal.LocalType.GetConstructors()[0]);
        }
        else
        {
            ilGen.Emit(OpCodes.Stloc, entityLocal);
        }

        ilGen.MarkLabel(labelInner);
        ilGen.Emit(OpCodes.Ldloc, entityLocal);
        ilGen.Emit(OpCodes.Ret);
        return dm.CreateDelegate<Func<IDataRecord, T>>();
    }

    private static Func<IDataRecord, T> BuildMapperCore(IDataRecord record, AttributesLookup attributesLookup)
    {
        var toType = typeof(T);

        if (DataRecordHelper.GetReaderMethod(toType) != null)
        {
            return BuildCreateScalarEntityFromRecordDynamicMethod(record);
        }

        if (record.FieldCount == 1)
        {
            var isNullableType = false;
            if (toType.IsGenericType && toType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullableType = true;
                toType = toType.GetGenericArguments()[0];
            }

            if (isNullableType || toType.IsPrimitiveOrExtendedPrimitive() || record.GetFieldType(0) == typeof(object))
            {
                return BuildCreateScalarEntityFromRecordDynamicMethod(record);
            }
        }

        var defaultConstructor = toType.GetConstructor(Type.EmptyTypes);
        int[] ordinals = null;

        if (defaultConstructor == null)
        {
            var constructor = toType.GetConstructors()[0];
            var parameters = constructor.GetParameters();
            ordinals = new int[parameters.Length];

            for (var i = 0; i < parameters.Length; i++)
                ordinals[i] = DataRecordHelper.GetOrdinal(record, parameters[i].Name);

            return BuildCreateEntityFromRecordDynamicMethod(record, constructor, ordinals);
        }

        var properties = toType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetIndexParameters().Length == 0).ToArray();
        ordinals = new int[properties.Length];

        var propertiesCount = properties.Length;

        for (var i = 0; i < propertiesCount; i++)
        {
            var property = properties[i];
            var columnName = property.GetPropertyColumnName();

            ordinals[i] = DataRecordHelper.GetOrdinal(record, columnName);
        }

        if (attributesLookup == null)
        {
            attributesLookup = new AttributesLookup(toType);
        }

        return BuildCreateEntityFromRecordDynamicMethod(record, properties, ordinals, attributesLookup);
    }

    [ILEmitter]
    private static void EmitILForDbNullCheck(int ordinal, ILGenerator ilGen, Label labelToGo)
    {
        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Ldc_I4, ordinal);
        ilGen.Emit(OpCodes.Callvirt, DataRecordHelper.IsDbNullMethod);
        ilGen.Emit(OpCodes.Ldc_I4, 0);
        ilGen.Emit(OpCodes.Ceq);
        ilGen.Emit(OpCodes.Brfalse, labelToGo);
    }

    [ILEmitter]
    private static void EmitILForReadingFromReaderAndConvertingToTheRequiredType(int ordinal, Type from, Type to,
        ILGenerator ilGen)
    {
        var cellReadMethod = GetReaderMethod(from);

        if (to.IsEnum && from == typeof(string))
        {
            ilGen.Emit(OpCodes.Ldobj, to);
        }

        ilGen.Emit(OpCodes.Ldarg, 0);
        ilGen.Emit(OpCodes.Ldc_I4, ordinal);

        ilGen.Emit(cellReadMethod.IsStatic ? OpCodes.Call : OpCodes.Callvirt, cellReadMethod);

        if (from != to)
        {
            if (to.IsEnum)
            {
                if (from == typeof(string))
                {
                    ilGen.Emit(OpCodes.Call, SharedReflectionInfo.EnumParseMethod);
                }
            }
            else
            {
                var convertMethod = ConvertHelper.GetConvertMethod(from, to, true);
                ilGen.Emit(OpCodes.Call, convertMethod);
            }
        }
    }

    [ILEmitter]
    private static bool EmitILForSorting(PropertyInfo property, ILGenerator ilGen, AttributesLookup attributesLookup)
    {
        var sortDirectionAttribute = attributesLookup.GetCustomAttribute<SortAttribute>(property, true);
        if (sortDirectionAttribute == null)
        {
            return false;
        }

        var propertyType = property.PropertyType;
        var elementType = propertyType.FindIEnumerable();
        if (elementType == null)
        {
            return false;
        }

        if (!typeof(List<>).MakeGenericType(elementType).IsAssignableFrom(propertyType))
        {
            return false;
        }

        var method = CollectionHelper.SortListInlineTemplateMethod.MakeGenericMethod(elementType);

        ilGen.Emit(OpCodes.Ldc_I4, (int)sortDirectionAttribute.SortOrder);
        ilGen.Emit(method.IsStatic ? OpCodes.Call : OpCodes.Callvirt, method);
        return true;
    }

    private static MethodInfo GetReaderMethod(Type from)
    {
        var method = DataRecordHelper.GetReaderMethod(from);

        if (method == null)
        {
            throw new MissingMethodException($"Could not find a mathod to read type {from} from IDataRecord");
        }

        return method;
    }
}