using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace SubhadraSolutions.Utils.Diagnostics;

public class ActiveCode
{
    private int? _callerLayer;

    private int _currentLayer;

    private StackFrame _stackFrame;

    static ActiveCode()
    {
        DroppedPrefixes = [];
        AddDroppedPrefix(typeof(ActiveCode).Namespace);
    }

    public ActiveCode(int requestedLayer = 0)
    {
        Initialize(requestedLayer);
    }

    public ActiveCode(int requestedLayer, IEnumerable<Type> boundaryTypes)
    {
        Initialize(requestedLayer, boundaryTypes);
    }

    public static ActiveCode Current => new();

    public static List<string> DroppedPrefixes { get; }

    public static ActiveCode Parent => Current.GetParent();

    public Type Class { get; private set; }

    public string ClassName
    {
        get
        {
            var text = ThreadingHelper.SafeDeref(Class.ToString) ?? string.Empty;
            text = text.Replace("+", ".");
            if (DroppedPrefixes.Count > 0)
            {
                foreach (var droppedPrefix in DroppedPrefixes)
                {
                    if (text.StartsWith(droppedPrefix))
                    {
                        text = text.Replace(droppedPrefix, "");
                    }
                }

                return text;
            }

            var array = text.Split('.');
            var num = array.Length - 2;
            if (num < 0)
            {
                num = 0;
            }

            return string.Join(".", array.Skip(num));
        }
    }

    public MethodBase Function { get; private set; }

    public string FunctionName => ThreadingHelper.SafeDeref(() => Function.Name) ?? string.Empty;

    public string Name
    {
        get
        {
            var text = "";
            var memberType = Function.MemberType;
            if (memberType is MemberTypes.Constructor or MemberTypes.Field or MemberTypes.Method)
            {
                text = !FunctionName.Contains("this") ? "()" : "[]";
            }

            var className = ClassName;
            return $"{className}{(className != string.Empty ? "." : "")}{FunctionName}{text}";
        }
    }

    public ActiveCode this[int layersAboveCaller] => new(layersAboveCaller + _currentLayer);

    public static void AddDroppedPrefix(string namespacePrefix)
    {
        if (!string.IsNullOrEmpty(namespacePrefix))
        {
            if (!namespacePrefix.EndsWith("."))
            {
                namespacePrefix += ".";
            }

            if (!DroppedPrefixes.Contains(namespacePrefix))
            {
                DroppedPrefixes.Add(namespacePrefix);
            }
        }
    }

    public static ActiveCode LayerAbove(IEnumerable<Type> types)
    {
        return new ActiveCode(0, types);
    }

    public static void RemoveDroppedPrefix(string namespacePrefix)
    {
        if (!string.IsNullOrEmpty(namespacePrefix))
        {
            if (!namespacePrefix.EndsWith("."))
            {
                namespacePrefix += ".";
            }

            if (DroppedPrefixes.Contains(namespacePrefix))
            {
                DroppedPrefixes.Remove(namespacePrefix);
            }
        }
    }

    public ActiveCode GetParent()
    {
        return this[1];
    }

    public void Initialize(int requestedLayer = 0, IEnumerable<Type> boundaryTypes = null)
    {
        var list = new List<Type> { GetType() };
        list.AddRange(boundaryTypes ?? new List<Type>());
        if (!_callerLayer.HasValue)
        {
            _callerLayer = -1;
            do
            {
                _callerLayer++;
                SetLayer(_callerLayer.Value);
            } while (list.Contains(Class));

            if (requestedLayer != 0)
            {
                SetLayer(requestedLayer + _callerLayer.Value);
            }
        }
        else
        {
            SetLayer(requestedLayer + _callerLayer.Value);
        }
    }

    private void SetLayer(int newLayer)
    {
        _currentLayer = newLayer;
        _stackFrame = new StackFrame(newLayer, true);
        Function = _stackFrame.GetMethod() ?? DefaultFunction.Instance;
        Class = Function.DeclaringType;
    }

    public class DefaultClass : TypeDelegator
    {
        public static DefaultClass Instance = new();

        public override string FullName => "<beyond stack: no class>";
        public override string Name => "<no class>";
    }

    public class DefaultFunction : MethodBase
    {
        public static DefaultFunction Instance = new();

        public override MethodAttributes Attributes => MethodAttributes.Private;
        public override Type DeclaringType => DefaultClass.Instance;
        public override MemberTypes MemberType => MemberTypes.Method;
        public override RuntimeMethodHandle MethodHandle => default;
        public override string Name => "<no function>";
        public override Type ReflectedType => DeclaringType;

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return Array.Empty<object>();
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return Array.Empty<object>();
        }

        public override MethodImplAttributes GetMethodImplementationFlags()
        {
            return MethodImplAttributes.IL;
        }

        public override ParameterInfo[] GetParameters()
        {
            return Array.Empty<ParameterInfo>();
        }

        public override object Invoke(object obj, BindingFlags invokeAttr, Binder binder, object[] parameters,
            CultureInfo culture)
        {
            throw new InvalidOperationException(GetType().Name +
                                                ": Placeholder object, not intended to be used directly.");
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return false;
        }
    }
}