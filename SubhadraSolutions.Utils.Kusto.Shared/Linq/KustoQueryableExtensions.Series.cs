using SubhadraSolutions.Utils.Linq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace SubhadraSolutions.Utils.Kusto.Shared.Linq;

public static partial class KustoQueryableExtensions
{
    // public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, double>> aggregator, Expression<Func<TSource, DateTime>> onSelector, TimeSpan step, Expression<Func<TSource, TBy>> bySelector, Expression<Func<SeriesOutputRecord<TBy, double, DateTime>, TResult>> merger)
    // {
    //    var method = GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        aggregator,
    //        onSelector,
    //        Expression.Constant(step),
    //        bySelector,
    //        merger
    //    }));
    // }

    // public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, long>> aggregator, Expression<Func<TSource, DateTime>> onSelector, TimeSpan step, Expression<Func<TSource, TBy>> bySelector, Expression<Func<SeriesOutputRecord<TBy, long, DateTime>, TResult>> merger)
    // {
    //    var method = GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        aggregator,
    //        onSelector,
    //        Expression.Constant(step),
    //        bySelector,
    //        merger
    //    }));
    // }

    public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source,
        Expression<Func<IQueryable<TSource>, long>> aggregator, Expression<Func<TSource, DateTime>> onSelector,
        TimeSpan step, Expression<Func<TSource, TBy>> bySelector,
        Expression<Func<SeriesOutputRecord<TBy, int, DateTime>, TResult>> merger)
    {
        var method =
            LinqFakeMethods.GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, source.Expression, aggregator,
            onSelector, Expression.Constant(step), bySelector, merger));
    }

    // public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, double>> aggregator, Expression<Func<TSource, double>> onSelector, TimeSpan step, Expression<Func<TSource, TBy>> bySelector, Expression<Func<SeriesOutputRecord<TBy, double, double>, TResult>> merger)
    // {
    //    var method = GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        aggregator,
    //        onSelector,
    //        Expression.Constant(step),
    //        bySelector,
    //        merger
    //    }));
    // }

    // public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, long>> aggregator, Expression<Func<TSource, long>> onSelector, TimeSpan step, Expression<Func<TSource, TBy>> bySelector, Expression<Func<SeriesOutputRecord<TBy, long, int>, TResult>> merger)
    // {
    //    var method = GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        aggregator,
    //        onSelector,
    //        Expression.Constant(step),
    //        bySelector,
    //        merger
    //    }));
    // }

    // public static IQueryable<TResult> MakeSeries<TSource, TBy, TResult>(this IQueryable<TSource> source, Expression<Func<IQueryable<TSource>, int>> aggregator, Expression<Func<TSource, int>> onSelector, TimeSpan step, Expression<Func<TSource, TBy>> bySelector, Expression<Func<SeriesOutputRecord<TBy, int, int>, TResult>> merger)
    // {
    //    var method = GetMethodInfo(MakeSeries, source, aggregator, onSelector, step, bySelector, merger);
    //    return source.Provider.CreateQuery<TResult>(Expression.Call(null, method, new Expression[]
    //    {
    //        source.Expression,
    //        aggregator,
    //        onSelector,
    //        Expression.Constant(step),
    //        bySelector,
    //        merger
    //    }));
    // }

    public static IQueryable<TResult> SeriesFitLine<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<double>>> aggregateSelector,
        Expression<Func<TSource, SeriesFitLineOutputRecord<double>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesFitLine, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesFitLine<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<long>>> aggregateSelector,
        Expression<Func<TSource, SeriesFitLineOutputRecord<long>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesFitLine, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesFitLine<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<int>>> aggregateSelector,
        Expression<Func<TSource, SeriesFitLineOutputRecord<int>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesFitLine, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesOutliers<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<double>>> aggregateSelector,
        Expression<Func<TSource, OutliersOutputRecord<int>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesOutliers, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesOutliers<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<long>>> aggregateSelector,
        Expression<Func<TSource, OutliersOutputRecord<int>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesOutliers, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesOutliers<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<int>>> aggregateSelector,
        Expression<Func<TSource, OutliersOutputRecord<int>, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesOutliers, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesStats<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<double>>> aggregateSelector,
        Expression<Func<TSource, SeriesStatsOutputRecord, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesStats, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesStats<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<long>>> aggregateSelector,
        Expression<Func<TSource, SeriesStatsOutputRecord, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesStats, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }

    public static IQueryable<TResult> SeriesStats<TSource, TResult>(this IQueryable<TSource> source,
        Expression<Func<TSource, PackedList<int>>> aggregateSelector,
        Expression<Func<TSource, SeriesStatsOutputRecord, TResult>> merger)
    {
        var method = LinqFakeMethods.GetMethodInfo(SeriesStats, source, aggregateSelector, merger);

        return source.Provider.CreateQuery<TResult>(Expression.Call(null, method,
        [
            source.Expression,
            aggregateSelector,
            merger
        ]));
    }
}

public class OutliersOutputRecord<T>
{
    public PackedList<T> Outlier { get; set; }
}

public class SeriesFitLineOutputRecord<TAggregate>
{
    public double Interception { get; set; }
    public PackedList<double> LineFit { get; set; }
    public double RSquare { get; set; }
    public double RVariance { get; set; }
    public double Slope { get; set; }
    public double Variance { get; set; }
}

public class SeriesFitLineRecord<TBy, TAggregate, TOn>
{
    public PackedList<TAggregate> Aggregate { get; set; }
    public TBy By { get; set; }
    public double Interception { get; set; }
    public PackedList<double> LineFit { get; set; }
    public PackedList<TOn> On { get; set; }

    public double RSquare { get; set; }
    public double RVariance { get; set; }
    public double Slope { get; set; }
    public double Variance { get; set; }
}

public class SeriesMVExpandRecord<TBy, TAggregate, TOn>
{
    public TAggregate Aggregate { get; set; }
    public TBy By { get; set; }
    public double Interception { get; set; }
    public double LineFit { get; set; }
    public TOn On { get; set; }

    public double Outlier { get; set; }
    public double RSquare { get; set; }
    public double RVariance { get; set; }
    public double SeriesStatsReadsAvg { get; set; }
    public double SeriesStatsReadsMax { get; set; }
    public double SeriesStatsReadsMaxIdx { get; set; }
    public double SeriesStatsReadsMin { get; set; }
    public double SeriesStatsReadsMinIdx { get; set; }
    public double SeriesStatsReadsStDev { get; set; }
    public double SeriesStatsReadsVariance { get; set; }
    public double Slope { get; set; }
    public double Variance { get; set; }
}

public class SeriesOutliersRecord<TBy, TAggregate, TOn>
{
    public PackedList<TAggregate> Aggregates { get; set; }
    public TBy By { get; set; }
    public PackedList<TOn> On { get; set; }

    public PackedList<double> Outlier { get; set; }
}

public class SeriesOutputRecord<TBy, TAggregate, TOn>
{
    public PackedList<TAggregate> Aggregate { get; set; }
    public TBy By { get; set; }
    public PackedList<TOn> On { get; set; }
}

public class SeriesStatsOutputRecord
{
    public double SeriesStatsReadsAvg { get; set; }
    public double SeriesStatsReadsMax { get; set; }
    public double SeriesStatsReadsMaxIdx { get; set; }
    public double SeriesStatsReadsMin { get; set; }
    public double SeriesStatsReadsMinIdx { get; set; }
    public double SeriesStatsReadsStDev { get; set; }
    public double SeriesStatsReadsVariance { get; set; }
}

public class SeriesStatsRecord<TBy, TAggregate, TOn>
{
    public PackedList<TAggregate> Aggregate { get; set; }
    public TBy By { get; set; }
    public double Interception { get; set; }
    public PackedList<double> LineFit { get; set; }
    public PackedList<TOn> On { get; set; }

    public double RSquare { get; set; }
    public double RVariance { get; set; }
    public double SeriesStatsReadsAvg { get; set; }
    public double SeriesStatsReadsMax { get; set; }
    public double SeriesStatsReadsMaxIdx { get; set; }
    public double SeriesStatsReadsMin { get; set; }
    public double SeriesStatsReadsMinIdx { get; set; }
    public double SeriesStatsReadsStDev { get; set; }
    public double SeriesStatsReadsVariance { get; set; }
    public double Slope { get; set; }
    public double Variance { get; set; }
}