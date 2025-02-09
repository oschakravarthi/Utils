// -----------------------------------------------------------------------
// <copyright file="KustoEntitiesEnumBase.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Kusto.Data.Common;

namespace SubhadraSolutions.Utils.Kusto.Server;

public abstract class KustoEntitiesEnumBase
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="KustoEntitiesEnumBase{T}" /> class.
    /// </summary>
    /// <param name="cslQueryProvider">The cslQueryProvider<see cref="ICslQueryProvider" />.</param>
    /// <param name="query">The query<see cref="string" />.</param>
    protected KustoEntitiesEnumBase(ICslQueryProvider cslQueryProvider, string query)
    {
        CslQueryProvider = cslQueryProvider;
        Query = query;
    }

    /// <summary>
    ///     Gets the CslQueryProvider.
    /// </summary>
    protected ICslQueryProvider CslQueryProvider { get; }

    /// <summary>
    ///     Gets the Query.
    /// </summary>
    protected string Query { get; }
}