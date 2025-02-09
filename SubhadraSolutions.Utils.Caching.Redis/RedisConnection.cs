using StackExchange.Redis;
using SubhadraSolutions.Utils.Abstractions;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Caching.Redis;

/// <summary>
///     Standard <seealso cref="StackExchange.Redis" /> Azure Cache for Redis implementation found
///     <see href="https://docs.microsoft.com/en-us/azure/azure-cache-for-redis/cache-dotnet-how-to-use-azure-redis-cache">here</see>
///     .
///     GitHub:
///     <see
///         href="https://github.com/Azure-Samples/azure-cache-redis-samples/blob/main/quickstart/dotnet/Redistest/RedisConnection.cs">
///         azure-cache-redis-samples
///     </see>
/// </summary>
public class RedisConnection : AbstractDisposable
{
    private const int RetryMaxAttempts = 5;
    private readonly string _connectionString;

    // If errors occur for longer than this threshold, StackExchange.Redis
    // may be failing to reconnect internally, so we'll recreate the
    // ConnectionMultiplexer instance
    private readonly TimeSpan ReconnectErrorThreshold = TimeSpan.FromSeconds(30);

    // StackExchange.Redis will also be trying to reconnect internally,
    // so limit how often we recreate the ConnectionMultiplexer instance
    // in an attempt to reconnect
    private readonly TimeSpan ReconnectMinInterval = TimeSpan.FromSeconds(60);

    private readonly TimeSpan RestartConnectionTimeout = TimeSpan.FromSeconds(15);
    private ConnectionMultiplexer _connection;
    private IDatabase _database;
    private DateTimeOffset _firstErrorTime = DateTimeOffset.MinValue;
    private long _lastReconnectTicks = DateTimeOffset.MinValue.UtcTicks;
    private DateTimeOffset _previousErrorTime = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _reconnectSemaphore = new(1, 1);

    private RedisConnection(string connectionString)
    {
        _connectionString = connectionString;
    }

    public static RedisConnection Initialize(string connectionString)
    {
        var redisConnection = new RedisConnection(connectionString);
        redisConnection.ForceReconnect(true);

        return redisConnection;
    }

    public static async Task<RedisConnection> InitializeAsync(string connectionString)
    {
        var redisConnection = new RedisConnection(connectionString);
        await redisConnection.ForceReconnectAsync(true).ConfigureAwait(false);

        return redisConnection;
    }

    public T BasicRetry<T>(Func<IDatabase, T> func)
    {
        var reconnectRetry = 0;

        while (true)
            try
            {
                return func(_database);
            }
            catch (Exception ex) when (ex is RedisConnectionException or SocketException)
            {
                reconnectRetry++;
                if (reconnectRetry > RetryMaxAttempts)
                {
                    throw;
                }

                try
                {
                    ForceReconnect();
                }
                catch (ObjectDisposedException)
                {
                }
            }
    }

    // In real applications, consider using a framework such as
    // Polly to make it easier to customize the retry approach.
    // For more info, please see: https://github.com/App-vNext/Polly
    public async Task<T> BasicRetryAsync<T>(Func<IDatabase, Task<T>> func)
    {
        var reconnectRetry = 0;

        while (true)
            try
            {
                return await func(_database).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex is RedisConnectionException or SocketException)
            {
                reconnectRetry++;
                if (reconnectRetry > RetryMaxAttempts)
                {
                    throw;
                }

                try
                {
                    await ForceReconnectAsync().ConfigureAwait(false);
                }
                catch (ObjectDisposedException)
                {
                }
            }
    }

    protected override void Dispose(bool disposing)
    {
        try
        {
            _connection?.Dispose();
        }
        catch
        {
        }
    }

    public ConnectionMultiplexer GetConnection()
    {
        return _connection;
    }

    public IServer GetServer()
    {
        var endpoints = GetConnection().GetEndPoints();
        var endpoint = (DnsEndPoint)endpoints[0];
        return GetConnection().GetServer(endpoint.Host, endpoint.Port);
    }

    private void ForceReconnect(bool initializing = false)
    {
        var previousTicks = Interlocked.Read(ref _lastReconnectTicks);
        var previousReconnectTime = new DateTimeOffset(previousTicks, TimeSpan.Zero);
        var elapsedSinceLastReconnect = DateTimeOffset.UtcNow - previousReconnectTime;

        // We want to limit how often we perform this top-level reconnect, so we check how long it's been since our last attempt.
        if (elapsedSinceLastReconnect < ReconnectMinInterval)
        {
            return;
        }

        try
        {
            _reconnectSemaphore.Wait(RestartConnectionTimeout);
        }
        catch
        {
            // If we fail to enter the semaphore, then it is possible that another thread has already done so.
            // ForceReconnectAsync() can be retried while connectivity problems persist.
            return;
        }

        try
        {
            var utcNow = DateTimeOffset.UtcNow;
            elapsedSinceLastReconnect = utcNow - previousReconnectTime;

            if (_firstErrorTime == DateTimeOffset.MinValue && !initializing)
            {
                // We haven't seen an error since last reconnect, so set initial values.
                _firstErrorTime = utcNow;
                _previousErrorTime = utcNow;
                return;
            }

            if (elapsedSinceLastReconnect <
                ReconnectMinInterval)
            {
                return; // Some other thread made it through the check and the lock, so nothing to do.
            }

            var elapsedSinceFirstError = utcNow - _firstErrorTime;
            var elapsedSinceMostRecentError = utcNow - _previousErrorTime;

            var shouldReconnect =
                elapsedSinceFirstError >=
                ReconnectErrorThreshold // Make sure we gave the multiplexer enough time to reconnect on its own if it could.
                && elapsedSinceMostRecentError <=
                ReconnectErrorThreshold; // Make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).

            // Update the previousErrorTime timestamp to be now (e.g. this reconnect request).
            _previousErrorTime = utcNow;

            if (!shouldReconnect && !initializing)
            {
                return;
            }

            _firstErrorTime = DateTimeOffset.MinValue;
            _previousErrorTime = DateTimeOffset.MinValue;

            if (_connection != null)
            {
                var oldConnection = _connection;
                try
                {
                    oldConnection?.Close();
                }
                catch (Exception)
                {
                    // Ignore any errors from the oldConnection
                }
            }

            Interlocked.Exchange(ref _connection, null);
            var newConnection = ConnectionMultiplexer.Connect(_connectionString);
            Interlocked.Exchange(ref _connection, newConnection);

            Interlocked.Exchange(ref _lastReconnectTicks, utcNow.UtcTicks);
            var newDatabase = _connection.GetDatabase();
            Interlocked.Exchange(ref _database, newDatabase);
        }
        finally
        {
            _reconnectSemaphore.Release();
        }
    }

    /// <summary>
    ///     Force a new ConnectionMultiplexer to be created.
    ///     NOTES:
    ///     1. Users of the ConnectionMultiplexer MUST handle ObjectDisposedExceptions, which can now happen as a result of
    ///     calling ForceReconnectAsync().
    ///     2. Call ForceReconnectAsync() for RedisConnectionExceptions and RedisSocketExceptions. You can also call it for
    ///     RedisTimeoutExceptions,
    ///     but only if you're using generous ReconnectMinInterval and ReconnectErrorThreshold. Otherwise, establishing new
    ///     connections can cause
    ///     a cascade failure on a server that's timing out because it's already overloaded.
    ///     3. The code will:
    ///     a. wait to reconnect for at least the "ReconnectErrorThreshold" time of repeated errors before actually
    ///     reconnecting
    ///     b. not reconnect more frequently than configured in "ReconnectMinInterval"
    /// </summary>
    /// <param name="initializing">Should only be true when ForceReconnect is running at startup.</param>
    private async Task ForceReconnectAsync(bool initializing = false)
    {
        var previousTicks = Interlocked.Read(ref _lastReconnectTicks);
        var previousReconnectTime = new DateTimeOffset(previousTicks, TimeSpan.Zero);
        var elapsedSinceLastReconnect = DateTimeOffset.UtcNow - previousReconnectTime;

        // We want to limit how often we perform this top-level reconnect, so we check how long it's been since our last attempt.
        if (elapsedSinceLastReconnect < ReconnectMinInterval)
        {
            return;
        }

        try
        {
            await _reconnectSemaphore.WaitAsync(RestartConnectionTimeout).ConfigureAwait(false);
        }
        catch
        {
            // If we fail to enter the semaphore, then it is possible that another thread has already done so.
            // ForceReconnectAsync() can be retried while connectivity problems persist.
            return;
        }

        try
        {
            var utcNow = DateTimeOffset.UtcNow;
            elapsedSinceLastReconnect = utcNow - previousReconnectTime;

            if (_firstErrorTime == DateTimeOffset.MinValue && !initializing)
            {
                // We haven't seen an error since last reconnect, so set initial values.
                _firstErrorTime = utcNow;
                _previousErrorTime = utcNow;
                return;
            }

            if (elapsedSinceLastReconnect <
                ReconnectMinInterval)
            {
                return; // Some other thread made it through the check and the lock, so nothing to do.
            }

            var elapsedSinceFirstError = utcNow - _firstErrorTime;
            var elapsedSinceMostRecentError = utcNow - _previousErrorTime;

            var shouldReconnect =
                elapsedSinceFirstError >=
                ReconnectErrorThreshold // Make sure we gave the multiplexer enough time to reconnect on its own if it could.
                && elapsedSinceMostRecentError <=
                ReconnectErrorThreshold; // Make sure we aren't working on stale data (e.g. if there was a gap in errors, don't reconnect yet).

            // Update the previousErrorTime timestamp to be now (e.g. this reconnect request).
            _previousErrorTime = utcNow;

            if (!shouldReconnect && !initializing)
            {
                return;
            }

            _firstErrorTime = DateTimeOffset.MinValue;
            _previousErrorTime = DateTimeOffset.MinValue;

            if (_connection != null)
            {
                var oldConnection = _connection;
                try
                {
                    await oldConnection?.CloseAsync();
                }
                catch (Exception)
                {
                    // Ignore any errors from the oldConnection
                }
            }

            Interlocked.Exchange(ref _connection, null);
            var newConnection = await ConnectionMultiplexer.ConnectAsync(_connectionString).ConfigureAwait(false);
            Interlocked.Exchange(ref _connection, newConnection);

            Interlocked.Exchange(ref _lastReconnectTicks, utcNow.UtcTicks);
            var newDatabase = _connection.GetDatabase();
            Interlocked.Exchange(ref _database, newDatabase);
        }
        finally
        {
            _reconnectSemaphore.Release();
        }
    }
}