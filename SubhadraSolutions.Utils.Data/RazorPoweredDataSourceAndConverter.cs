//using Microsoft.AspNetCore.Razor.Language;
//using SubhadraSolutions.Utils.Data;
//using System;
//using System.Collections.Concurrent;
//using System.Linq;
//using System.Reflection;

//namespace SubhadraSolutions.Utils.Server.Data
//{
//    public class RazorPoweredDataSourceAndConverter : IDataSource<string>, IDataConverter<string>
//    {
//        private static readonly Action<IRazorEngineCompilationOptionsBuilder> builderAction = PopulateRazorEngineCompilationOptionsBuilder;
//        private static readonly ConcurrentDictionary<Type, IRazorEngineCompiledTemplate> compiledTemplates = new();
//        private static readonly MethodInfo compileGenericMethod = typeof(RazorEngine).GetMethods().Where(m => m.Name == "Compile" && m.IsGenericMethod).First();
//        private static readonly IRazorEngine razorEngine = new RazorEngine();

//        private IRazorEngineCompiledTemplate compiledTemplateForNullType = null;
//        public string Template { get; set; }

//        public static IRazorEngineCompiledTemplate CompileTemplate(Type modelType, string template)
//        {
//            if (modelType == null)
//            {
//                return razorEngine.Compile(template, builderAction);
//            }
//            var method = compileGenericMethod.MakeGenericMethod(modelType);
//            return (IRazorEngineCompiledTemplate)method.Invoke(razorEngine, new object[] { template, builderAction });
//        }

//        public string Convert(object input)
//        {
//            var inputType = input == null ? null : input.GetType();

//            IRazorEngineCompiledTemplate compiledTemplate = GetCompiledTemplate(inputType);
//            var output = compiledTemplate.Run(input);
//            return output;
//        }

//        public string GetData()
//        {
//            IRazorEngineCompiledTemplate compiledTemplate = GetCompiledTemplate(null);
//            var output = compiledTemplate.Run();
//            return output;
//        }

//        private static void PopulateRazorEngineCompilationOptionsBuilder(IRazorEngineCompilationOptionsBuilder builder)
//        {
//            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
//            foreach (var assembly in assemblies)
//            {
//                if (!assembly.FullName.StartsWith("Microsoft.HCC.") && !assembly.FullName.StartsWith("System."))
//                {
//                    continue;
//                }
//                builder.AddAssemblyReference(assembly);
//            }
//        }

//        private IRazorEngineCompiledTemplate GetCompiledTemplate(Type inputType)
//        {
//            IRazorEngineCompiledTemplate compiledTemplate = null;
//            if (inputType != null)
//            {
//                compiledTemplate = compiledTemplates.GetOrAdd(inputType, (t) => CompileTemplate(t, this.Template));
//            }
//            else
//            {
//                if (compiledTemplateForNullType == null)
//                {
//                    lock (razorEngine)
//                    {
//                        if (compiledTemplateForNullType == null)
//                        {
//                            this.compiledTemplateForNullType = CompileTemplate(null, this.Template);
//                        }
//                    }
//                }
//                compiledTemplate = compiledTemplateForNullType;
//            }

//            return compiledTemplate;
//        }
//    }
//}