using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace SubhadraSolutions.Utils.Reflection.Helpers;

public static class MethodHelper
{
    private static readonly ConcurrentBag<Tuple<MethodBase, MethodBase>> _references = [];

    private static readonly MethodInfo getMethodDescriptorInfoMethod =
        typeof(DynamicMethod).GetMethod("GetMethodDescriptor", BindingFlags.NonPublic | BindingFlags.Instance);

    public static IntPtr GetMethodAddress(MethodBase method)
    {
        if (method is DynamicMethod dynamicMethod)
        {
            return GetDynamicMethodAddress(dynamicMethod);
        }

        RuntimeHelpers.PrepareMethod(method.MethodHandle);
        return method.MethodHandle.GetFunctionPointer();
    }

    public static void ReplaceMethod(MethodBase oldMethod, MethodBase newMethod)
    {
        if (!MethodSignaturesEqual(newMethod, oldMethod))
        {
            throw new ArgumentException(@"The method signatures are not the same.", nameof(newMethod));
        }

        _references.Add(new Tuple<MethodBase, MethodBase>(newMethod, oldMethod));

        var oldMethodAdr = GetMethodAddressRef(oldMethod);
        var newMethodAdr = GetMethodAddress(newMethod);
        ReplaceMethod(oldMethodAdr, newMethodAdr);
    }

    private static IntPtr GetDynamicMethodAddress(DynamicMethod method)
    {
        var handle = GetDynamicMethodRuntimeHandle(method);
        RuntimeHelpers.PrepareMethod(handle);
        return handle.GetFunctionPointer();
    }

    private static RuntimeMethodHandle GetDynamicMethodRuntimeHandle(DynamicMethod method)
    {
        var handle = (RuntimeMethodHandle)getMethodDescriptorInfoMethod.Invoke(method, null);
        return handle;
    }

    private static IntPtr GetMethodAddress20SP2(MethodBase method)
    {
        unsafe
        {
            return new IntPtr((int*)method.MethodHandle.Value.ToPointer() + 2);
        }
    }

    private static IntPtr GetMethodAddressRef(MethodBase srcMethod)
    {
        if (srcMethod is DynamicMethod dynamicMethod)
        {
            return GetDynamicMethodAddress(dynamicMethod);
        }

        RuntimeHelpers.PrepareMethod(srcMethod.MethodHandle);

        var funcPointer = srcMethod.MethodHandle.GetFunctionPointer();
        var addrRef = GetMethodAddress20SP2(srcMethod);
        if (IsAddressValueMatch(addrRef, funcPointer))
        {
            return addrRef;
        }

        unsafe
        {
            var methodDesc = (ulong*)srcMethod.MethodHandle.Value.ToPointer();
            var index = (int)((*methodDesc >> 32) & 0xFF);
            var step = IntPtr.Size;
            var classStart = (byte*)srcMethod.DeclaringType.TypeHandle.Value.ToPointer();
            classStart += 10 * step;
            var address = classStart + index * step;
            addrRef = new IntPtr(address);
            if (IsAddressValueMatch(addrRef, funcPointer))
            {
                return addrRef;
            }

            var error =
                $"Method Injection Error: The address {addrRef} 's value {*(IntPtr*)addrRef} doesn't match expected value: {funcPointer}";
            throw new InvalidOperationException(error);
        }
    }

    private static Type GetMethodReturnType(MethodBase method)
    {
        var methodInfo = method as MethodInfo;
        if (methodInfo == null)
        {
            throw new ArgumentException(@"Unsupported MethodBase : " + method.GetType().Name, nameof(method));
        }

        return methodInfo.ReturnType;
    }

    private static RuntimeMethodHandle GetMethodRuntimeHandle(MethodBase method)
    {
        if (!(method is DynamicMethod))
        {
            return method.MethodHandle;
        }

        RuntimeMethodHandle handle;
        if (Environment.Version.Major == 4)
        {
            var getMethodDescriptorInfo = typeof(DynamicMethod).GetMethod("GetMethodDescriptor",
                BindingFlags.NonPublic | BindingFlags.Instance);
            handle = (RuntimeMethodHandle)getMethodDescriptorInfo.Invoke(method, null);
        }
        else
        {
            var fieldInfo = typeof(DynamicMethod).GetField("m_method", BindingFlags.NonPublic | BindingFlags.Instance);
            handle = (RuntimeMethodHandle)fieldInfo.GetValue(method);
        }

        return handle;
    }

    private static bool IsAddressValueMatch(IntPtr address, IntPtr value)
    {
        unsafe
        {
            var realValue = *(IntPtr*)address;
            return realValue == value;
        }
    }

    private static bool MethodSignaturesEqual(MethodBase x, MethodBase y)
    {
        Type returnX = GetMethodReturnType(x), returnY = GetMethodReturnType(y);
        if (returnX != returnY)
        {
            return false;
        }

        ParameterInfo[] xParams = x.GetParameters(), yParams = y.GetParameters();
        if (xParams.Length != yParams.Length)
        {
            if ((x.IsStatic && !x.IsStatic && xParams.Length == yParams.Length + 1 &&
                 xParams[0].ParameterType == x.DeclaringType) || (x.IsStatic && !y.IsStatic &&
                                                                  yParams.Length == xParams.Length + 1 &&
                                                                  yParams[0].ParameterType == y.DeclaringType))
            {
                var min = x.IsStatic ? yParams : xParams;
                var max = x.IsStatic ? xParams : yParams;
                return !min.Where((t, i) => t.ParameterType != max[i + 1].ParameterType).Any();
            }
        }

        return true;
    }

    private static void ReplaceMethod(IntPtr oldMethodAdr, IntPtr srcAdr)
    {
        unsafe
        {
            if (IntPtr.Size == 8)
            {
                var d = (ulong*)oldMethodAdr.ToPointer();
                var newAddress = (ulong)srcAdr.ToInt64();
                *d = newAddress;
            }
            else
            {
                var d = (uint*)oldMethodAdr.ToPointer();
                *d = (uint)srcAdr.ToInt32();
            }
        }
    }
}