using SubhadraSolutions.Utils.Threading;
using System;

namespace SubhadraSolutions.Utils.Pooling.Internal;

internal class PoolAsyncResult : AsyncResult<object>
{
    internal PoolAsyncResult(int poolId, Type poolType)
    {
        PoolId = poolId;
        PoolType = poolType;
    }

    internal int PoolId { get; }

    internal Type PoolType { get; }
}