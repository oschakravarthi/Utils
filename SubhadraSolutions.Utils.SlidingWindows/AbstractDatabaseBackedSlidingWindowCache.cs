//using SubhadraSolutions.Utils.Server.Caching.Contracts;
//using SubhadraSolutions.Utils.Server.Contracts;
//using SubhadraSolutions.Utils.Server.Data.Contracts;
//using SubhadraSolutions.Utils.Server.Data.Dynamic;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//namespace SubhadraSolutions.Utils.Server.Caching
//{
//    public abstract class AbstractDatabaseBackedSlidingWindowCache<T> : AbstractSlidingWindowDataCache<T>, ISlidingWindowCache<T>, IUnique where T : class, ITimeRanged, new()
//    {
//        private readonly string fromTimestampColumnName;
//        private readonly string uptoTimestampColumnName;
//        protected AbstractDatabaseBackedSlidingWindowCache(IDataProvider<T> dataProvider, TimeSpan seriesDuration, TimeSpan fetchWindowDuration, TimeSpan? historyReplayDuration, string fromTimestampColumnName, string uptoTimestampColumnName)
//            : base(dataProvider, seriesDuration, fetchWindowDuration, historyReplayDuration)
//        {
//            this.fromTimestampColumnName = fromTimestampColumnName;
//            this.uptoTimestampColumnName = fromTimestampColumnName;
//        }

//        protected abstract IDbConnectionFactory GetDbConnectionFactory();
//        protected override void AddItems(IReadOnlyList<T> items)
//        {
//            InsertItems(items);
//        }

//        protected override void InitializeProtected()
//        {
//            base.InitializeProtected();

//            var dbConnectionFactory = GetDbConnectionFactory();
//            DynamicDAO<T>.Initialize(dbConnectionFactory);
//            try
//            {
//                DynamicDAO<T>.CreateTable();
//            }
//            catch (Exception e)
//            {
//            }
//        }

//        private void InsertItems(IEnumerable<T> items)
//        {
//            DynamicDAO<T>.Insert(items);
//        }

//        protected override void PurgeExpiredItems(DateTime upto)
//        {
//            DynamicDAO<T>.Delete(null, null, new ClauseCondition(fromTimestampColumnName, "<", upto));
//        }

//        protected override IQueryable<T> GetAllItemsCore()
//        {
//            var enumerable = DynamicDAO<T>.SelectAll(orderBy: string.Format("{0} desc", this.fromTimestampColumnName));
//            enumerable = Interner<T>.WrapIntern(enumerable);
//            return enumerable.AsQueryable();
//        }

//        public virtual IReadOnlyList<T> GetData(DateTime from, DateTime upto)
//        {
//            var enumerable = DynamicDAO<T>.Select(null, null, string.Format("{0} desc", this.fromTimestampColumnName), new ClauseCondition(fromTimestampColumnName, ">=", from), new ClauseCondition(uptoTimestampColumnName, "<", upto));
//            return enumerable.ToList().AsReadOnly();
//        }

//        public override IEnumerable<Window<T>> GetWindows(TimeSpan windowDuration, uint windowsToSkip, uint maxWindowsToFetch)
//        {
//            var from = this.MaxTimestamp - (windowDuration * windowsToSkip);
//            var upto = from + (windowDuration * maxWindowsToFetch);
//            var enumerable = DynamicDAO<T>.Select(null, null, string.Format("{0} desc", this.fromTimestampColumnName), new ClauseCondition(fromTimestampColumnName, ">=", from), new ClauseCondition(uptoTimestampColumnName, "<", upto));
//            return WindowingHelper.SplitIntoWindows<T>(enumerable, windowDuration);
//        }
//    }
//}