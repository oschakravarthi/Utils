//using SubhadraSolutions.Utils.Server.Contracts;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace SubhadraSolutions.Utils.Server.Caching
//{
//    public abstract class WindowingEnum<T>
//    {
//        protected readonly IEnumerator<T> enumerator;
//        protected readonly DateTime from;
//        protected readonly DateTime upto;
//        protected readonly bool isFirstWindow;
//        protected WindowingEnum(IEnumerator<T> enumerator, DateTime from, DateTime upto, bool isFirstWindow)
//        {
//            this.enumerator = enumerator;
//            this.from = from;
//            this.upto = upto;
//        }
//    }

//    public class WindowingEnumerable<T> : WindowingEnum<T>, IEnumerable<T> where T : ITimed
//    {
//        public WindowingEnumerable(IEnumerator<T> enumerator, DateTime from, DateTime upto, bool isFirstWindow) : base(enumerator, from, upto, isFirstWindow)
//        {
//        }
//        public IEnumerator<T> GetEnumerator()
//        {
//            return new WindowingEnumerator<T>(this.enumerator, this.from, this.upto, this.isFirstWindow);
//        }

//        IEnumerator IEnumerable.GetEnumerator()
//        {
//            return new WindowingEnumerator<T>(this.enumerator, this.from, this.upto, this.isFirstWindow);
//        }
//    }

//    internal class WindowingEnumerator<T> : WindowingEnum<T>, IEnumerator<T> where T : ITimed
//    {
//        private T current;
//        public WindowingEnumerator(IEnumerator<T> enumerator, DateTime from, DateTime upto, bool isFirstWindow) : base(enumerator, from, upto, isFirstWindow)
//        {
//            this.HasReachedEnd = false;

//            if (isFirstWindow)
//            {
//                this.HasReachedEnd = !this.enumerator.MoveNext();
//            }
//        }
//        public bool HasReachedEnd { get; private set; }

//        public T Current
//        {
//            get { return this.current; }
//        }

//        object IEnumerator.Current
//        {
//            get { return this.current; }
//        }

//        public bool MoveNext()
//        {
//            var item = this.enumerator.Current;
//            if (HasReachedEnd)
//            {
//                return false;
//            }
//            if (item.Timestamp >= upto)
//            {
//                return false;
//            }
//            if (item.Timestamp < from)
//            {
//                return false;
//            }
//            this.current = item;
//            this.HasReachedEnd = !this.enumerator.MoveNext();
//            return true;
//        }

//        public void Reset()
//        {
//        }

//        public void Dispose()
//        {
//        }
//    }
//}