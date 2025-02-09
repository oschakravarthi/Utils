//using SubhadraSolutions.Utils.Server.Contracts;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SubhadraSolutions.Utils.Server.Caching
//{
//    public class TimeRangedLinkedList<T> : LinkedList<T> where T : ITimeRanged
//    {
//        private ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();
//        public IEnumerable<T> GetWindow(DateTime from, DateTime upto)
//        {
//            var enumerable = GetAllItems();

//            foreach (var item in enumerable)
//            {
//                var tsUpto = item.GetUptoTimestamp();
//                if (tsUpto >= upto)
//                {
//                    continue;
//                }

//                var tsFrom = item.GetFromTimestamp();
//                if (tsFrom < from)
//                {
//                    yield break;
//                }
//                yield return item;
//            }
//        }

//        public void PurgeExpiredItems(DateTime upto)
//        {
//            int count = 0;

//            try
//            {
//                slimLock.EnterWriteLock();
//                if (this.Count > 0)
//                {
//                    var temp = this.Last;

//                    while (temp != null)
//                    {
//                        //var tsFrom = temp.Value.GetFromTimestamp();
//                        var tsUpto = temp.Value.GetUptoTimestamp();

//                        if (tsUpto < upto)
//                        {
//                            var prev = temp.Previous;
//                            this.Remove(temp);
//                            temp = prev;
//                            count++;
//                        }
//                        else
//                        {
//                            return;
//                        }
//                    }
//                }

//            }
//            finally
//            {
//                slimLock.ExitWriteLock();
//                Console.WriteLine("Purged: {0}", count);
//            }
//        }

//        public IEnumerable<T> GetAllItems()
//        {
//            var first = this.First;
//            if (first == null)
//            {
//                yield break;
//            }

//            slimLock.EnterReadLock();
//            var last = this.Last;
//            while (first != last)
//            {
//                yield return first.Value;
//                first = first.Next;
//            }
//            yield return first.Value;
//            slimLock.ExitReadLock();
//        }

//        public void AddFirst(IReadOnlyList<T> items)
//        {
//            if(items.Count < 1)
//            {
//                return;
//            }
//            try
//            {
//                this.slimLock.EnterWriteLock();

//                var previous = this.First;
//                for (int i = items.Count - 1; i >= 0; i--)
//                {
//                    if (previous == null)
//                    {
//                        previous = this.AddFirst(items[i]);
//                    }
//                    else
//                    {
//                        if (previous.Value.GetUptoTimestamp() <= items[i].GetFromTimestamp())
//                        {
//                            previous = this.AddFirst(items[i]);
//                        }
//                        else
//                        {
//                            if (i != items.Count - 1)
//                            {
//                                throw new Exception("LL");
//                            }
//                            else
//                            {
//                                previous = this.AddFirst(items[i]);
//                            }
//                        }
//                    }
//                }
//            }
//            finally
//            {
//                this.slimLock.ExitWriteLock();
//            }
//        }

//        //public void AddFirst(IReadOnlyList<T> items)
//        //{
//        //    for (int i = items.Count - 1; i >= 0; i--)
//        //    {
//        //        this.AddFirst(items[i]);
//        //    }
//        //}
//    }
//}