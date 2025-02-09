using SubhadraSolutions.Utils.Reflection;
using SubhadraSolutions.Utils.Reflection.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace SubhadraSolutions.Utils.Instrumentation.Instruments;

public static class NotifyPropertyChangedInstrument<T>
{
    private static readonly object SyncLock = new();
    private static volatile bool instrumented;
    private static ReadOnlyCollection<PropertyChangedEventArgs> propertyChangedEventArgsList;

    private static ReadOnlyCollection<PropertyChangingEventArgs> propertyChangingEventArgsList;

    static NotifyPropertyChangedInstrument()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        List<PropertyChangedEventArgs> changedEventArgsList = null;
        if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)))
        {
            changedEventArgsList = [];
        }

        List<PropertyChangingEventArgs> changingEventArgsList = null;
        if (typeof(INotifyPropertyChanging).IsAssignableFrom(typeof(T)))
        {
            changingEventArgsList = [];
        }

        if (changingEventArgsList == null && changedEventArgsList == null)
        {
            return;
        }

        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                changedEventArgsList?.Add(new PropertyChangedEventArgs(property.Name));

                changingEventArgsList?.Add(new PropertyChangingEventArgs(property.Name));
            }
        }

        if (changedEventArgsList != null)
        {
            propertyChangedEventArgsList = changedEventArgsList.AsReadOnly();
        }

        if (changingEventArgsList != null)
        {
            propertyChangingEventArgsList = changingEventArgsList.AsReadOnly();
        }
    }

    public static void Instrument()
    {
        if (!instrumented)
        {
            lock (SyncLock)
            {
                if (!instrumented)
                {
                    instrumented = true;
                    InstrumentPropertyChange();
                }
            }
        }
    }

    private static void InjectCallActualMethod(ILGenerator ilGen, MethodInfo setMethod)
    {
        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Ldarg_1);
        ilGen.Emit(OpCodes.Callvirt, setMethod);
    }

    private static void InjectRaiseEvent(ILGenerator ilGen, int propertyIndex, FieldInfo eventField,
        int eventFieldLocalIndex, FieldInfo eventArgsField, MethodInfo indexerGetterMethod)
    {
        ilGen.Emit(OpCodes.Ldarg_0);
        ilGen.Emit(OpCodes.Ldfld, eventField);
        ilGen.Emit(OpCodes.Stloc, eventFieldLocalIndex);

        ilGen.CreateArray(typeof(object), 2, 0);

        ilGen.ReplaceArrayElement(OpCodes.Ldloc_0, 0, OpCodes.Ldarg_0);
        var index = propertyIndex;
        ilGen.ReplaceArrayElementOfReferenceType(OpCodes.Ldloc_0, 1, () =>
        {
            ilGen.Emit(OpCodes.Ldsfld, eventArgsField);
            ilGen.Emit(OpCodes.Ldc_I4, index);
            ilGen.Emit(OpCodes.Callvirt, indexerGetterMethod);
        });

        ilGen.Emit(OpCodes.Ldloc, eventFieldLocalIndex);
        ilGen.Emit(OpCodes.Ldloc_0);
        ilGen.Emit(OpCodes.Call, EventHelper.SafeInvokeMethod);
    }

    private static bool InstrumentPropertyChange()
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var propertyChangedEventField =
            typeof(T).GetField("PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
        var propertyChangingEventField =
            typeof(T).GetField("PropertyChanging", BindingFlags.Instance | BindingFlags.NonPublic);
        var injectForPropertyChanged = propertyChangedEventArgsList != null && propertyChangedEventField != null;
        var injectForPropertyChanging = propertyChangingEventArgsList != null && propertyChangingEventField != null;

        if (!injectForPropertyChanged && !injectForPropertyChanging)
        {
            return false;
        }

        var changingEventArgsField =
            typeof(NotifyPropertyChangedInstrument<T>).GetField(nameof(propertyChangingEventArgsList),
                BindingFlags.NonPublic | BindingFlags.Static);
        var changedEventArgsField =
            typeof(NotifyPropertyChangedInstrument<T>).GetField(nameof(propertyChangedEventArgsList),
                BindingFlags.NonPublic | BindingFlags.Static);

        var changingEventArgsIndexerGetterMethod = typeof(ReadOnlyCollection<PropertyChangingEventArgs>).GetProperties()
            .First(p => p.GetIndexParameters().Length == 1).GetMethod;
        var changedEventArgsIndexerGetterMethod = typeof(ReadOnlyCollection<PropertyChangedEventArgs>).GetProperties()
            .First(p => p.GetIndexParameters().Length == 1).GetMethod;

        var propertyIndex = -1;
        foreach (var property in properties)
        {
            if (property.CanWrite)
            {
                propertyIndex++;
                var setMethod = property.SetMethod;
                var dynamicMethod = new DynamicMethod("NotifyPropertyChange_" + property.Name, setMethod.ReturnType,
                    [typeof(T), property.PropertyType], typeof(T));

                var ilGen = dynamicMethod.GetILGenerator();
                ilGen.DeclareLocal(typeof(object[]));
                if (injectForPropertyChanging)
                {
                    ilGen.DeclareLocal(typeof(PropertyChangingEventHandler));
                }

                if (injectForPropertyChanged)
                {
                    ilGen.DeclareLocal(typeof(PropertyChangedEventHandler));
                }

                if (injectForPropertyChanging)
                {
                    InjectRaiseEvent(ilGen, propertyIndex, propertyChangingEventField, 1, changingEventArgsField,
                        changingEventArgsIndexerGetterMethod);
                }

                InjectCallActualMethod(ilGen, setMethod);

                if (injectForPropertyChanged)
                {
                    InjectRaiseEvent(ilGen, propertyIndex, propertyChangedEventField, injectForPropertyChanging ? 2 : 1,
                        changedEventArgsField, changedEventArgsIndexerGetterMethod);
                }

                ilGen.Emit(OpCodes.Ret);
                MethodHelper.ReplaceMethod(setMethod, dynamicMethod);
            }
        }

        return true;
    }
}