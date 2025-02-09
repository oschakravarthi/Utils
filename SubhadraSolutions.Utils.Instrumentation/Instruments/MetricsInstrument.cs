//using SubhadraSolutions.Utils.Instrumentation.Helpers;
//using SubhadraSolutions.Utils.Diagnostics.Metrics;
//using System.Reflection;
//using System.Reflection.Emit;

//namespace SubhadraSolutions.Utils.Instrumentation.Instruments
//{
//    public class MetricsInstrument
//    {
//        private static readonly FieldInfo ILField;
//        static MetricsInstrument()
//        {
//            var t = typeof(Type).Assembly.GetTypes().Where(x => x.Name == "RuntimeMethodBody").First();
//            ILField = t.GetRuntimeFields().Where(x=>x.Name=="_IL").First();
//        }
//        private readonly IMetricsTracker metricsTracker;
//        public MetricsInstrument(IMetricsTracker metricsTracker)
//        {
//            this.metricsTracker = metricsTracker;
//            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
//        }

//        private void CurrentDomain_AssemblyLoad(object? sender, AssemblyLoadEventArgs args)
//        {
//            foreach (var type in args.LoadedAssembly.GetTypes())
//            {
//                if (!type.IsClass && type.IsAbstract)
//                {
//                    continue;
//                }
//                InstrumentType(type);
//                //if (typeof(IMetricsProvider).IsAssignableFrom(type))
//                //{
//                //    if (this.metricsTracker.HasMetrics(type))
//                //    {
//                //        InstrumentType(type);
//                //    }
//                //}
//            }
//        }

//        private void InstrumentType(Type type)
//        {
//            foreach(var method in type.GetMethods())
//            {
//                if (!method.IsAbstract)
//                {
//                    if (method.GetMethodBody() != null)
//                    {
//                        Instrument(method, type);
//                    }
//                }
//            }
//            //var constructor = type.GetConstructors().FirstOrDefault();
//            //if (constructor != null)
//            //{
//            //    Instrument(constructor, type);
//            //}
//        }

//        private void Instrument(MethodBase constructor, Type type)
//        {
//            var msilReader = new MsilReader(constructor);
//            var ils = new List<MsilInstruction>();
//            while(msilReader.Read())
//            {
//                ils.Add(msilReader.Current);
//            }
//            return;
//            var body = constructor.GetMethodBody();
//            var x = body.GetILAsByteArray();

//            var actualParameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();
//            var parameterTypes = new List<Type>();
//            parameterTypes.Add(type);
//            parameterTypes.AddRange(actualParameterTypes);

//            var dm = new DynamicMethod("ctor", null, parameterTypes.ToArray(), type);
//            ILGenerator ilGen = dm.GetILGenerator();

//            for (int i = 0; i < parameterTypes.Count; i++)
//            {
//                ilGen.Emit(OpCodes.Ldarg, i);
//            }
//            //ilGen.Emit(OpCodes.Calli, constructor);

//            //ilGen.Emit(OpCodes.Newobj, constructor);

//            //Label exBlock = ilGen.BeginExceptionBlock();

//            //for (int i = 0; i < parameterTypes.Count; i++)
//            //{
//            //    ilGen.Emit(OpCodes.Ldarg, i);
//            //}
//            //ilGen.Emit(OpCodes.Callvirt, constructor);

//            //ilGen.Emit(OpCodes.Leave, exBlock);

//            //ilGen.BeginFinallyBlock();

//            //ilGen.Emit(OpCodes.Call, this._getInstanceMethod);
//            //ilGen.Emit(OpCodes.Call, ElapsedTicksMethod);
//            //ilGen.Emit(OpCodes.Ldloc_0);
//            //ilGen.Emit(OpCodes.Sub);
//            //ilGen.Emit(OpCodes.Ldstr, guid.ToString());
//            //ilGen.Emit(OpCodes.Callvirt, this._incrementMethodMetricsMethod);

//            //ilGen.EndExceptionBlock();
//            //ilGen.Emit(OpCodes.Ldarg, 0);
//            ilGen.Emit(OpCodes.Ret);

//            //var del = dm.CreateDelegate(typeof(Delegate));
//            MethodHelper.ReplaceMethod(constructor, dm);

//        }
//    }
//}