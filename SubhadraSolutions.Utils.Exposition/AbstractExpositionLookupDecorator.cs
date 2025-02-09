//
//using Newtonsoft.Json.Linq;

//namespace SubhadraSolutions.Utils.Exposition
//{
//    public abstract class AbstractExpositionLookupDecorator : IExpositionLookup
//    {
//        protected readonly IExpositionLookup actual;

//        protected AbstractExpositionLookupDecorator(IExpositionLookup actual)
//        {
//            this.actual = actual;
//        }

//        public virtual string ApiBaseUrl
//        {
//            get
//            {
//                return actual.ApiBaseUrl;
//            }
//            set
//            {
//                actual.ApiBaseUrl = value;
//            }
//        }

//        public virtual object Execute(RequestInfo requestInfo, JObject actionArguments)
//        {
//            return actual.Execute(requestInfo, actionArguments);
//        }

//        public virtual Type GetReturnType(RequestInfo requestInfo)
//        {
//            return actual.GetReturnType(requestInfo);
//        }

//        public virtual void RegisterMethods(string path, object obj)
//        {
//            actual.RegisterMethods(path, obj);
//        }
//    }
//}