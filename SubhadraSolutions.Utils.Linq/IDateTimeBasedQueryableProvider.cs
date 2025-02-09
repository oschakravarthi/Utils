using System;
using System.Linq;

namespace SubhadraSolutions.Utils.Linq;

public interface IDateTimeBasedQueryableProvider : IQueryableProvider
{
    DateTime MaxDate { get; }
    DateTime MinDate { get; }

    IQueryable GetQueryable(DateTime from, DateTime upto);
}