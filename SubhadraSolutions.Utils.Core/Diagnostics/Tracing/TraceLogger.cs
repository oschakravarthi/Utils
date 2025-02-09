#define TRACE

using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace SubhadraSolutions.Utils.Diagnostics.Tracing;

public static class TraceLogger
{
    public static Dictionary<string, string> LogfileTable = [];

    private static readonly Dictionary<string, TraceListenerEntry> _RegisteredListeners = [];

    private static readonly TimeSpan LongTraceLogDuration = TimeSpan.FromSeconds(10.0);

    private static ThreadLocal<TraceListener> _ListenerForCurrentThread = new(true);

    private static string _logfileDirectory;

    public static TraceListener ListenerForCurrentThread
    {
        get => _ListenerForCurrentThread.Value;
        set { _ListenerForCurrentThread = new ThreadLocal<TraceListener>(() => value, true); }
    }

    public static string LogfileDirectory
    {
        get
        {
            if (_logfileDirectory == null)
            {
                //TODO:
                //_logfileDirectory = ConfigurationManager.AppSettings["TraceHelper.LogfileDirectory"];
                if (!string.IsNullOrEmpty(_logfileDirectory))
                {
                    LogfileDirectory = _logfileDirectory;
                }
                else
                {
                    _logfileDirectory = string.Empty;
                }
            }

            return _logfileDirectory;
        }
        set
        {
            _logfileDirectory = value;
            if (!string.IsNullOrEmpty(_logfileDirectory))
            {
                _logfileDirectory = _logfileDirectory.Trim(Path.DirectorySeparatorChar) +
                                    Path.DirectorySeparatorChar;
            }
        }
    }

    public static void DeleteLog(string channelName)
    {
        string logfileName = null;
        LogfileTable.ThreadSafeRead(delegate { LogfileTable.TryGetValue(channelName, out logfileName); });
        if (!string.IsNullOrEmpty(logfileName))
        {
            File.Delete(logfileName);
        }
    }

    public static void Flush(TraceChannelId channelId)
    {
        var channelName = (string)channelId;
        TraceListenerEntry listenerEntry = null;
        var shouldFlush = false;
        _RegisteredListeners.ThreadSafeRead(delegate
        {
            if (_RegisteredListeners.TryGetValue(channelName, out listenerEntry))
            {
                shouldFlush = true;
            }
        });
        if (shouldFlush)
        {
            listenerEntry.Listener.Flush();
        }
    }

    public static TraceLogFile GetLog(string channelName)
    {
        TraceLogFile result = null;
        string logfileName = null;
        LogfileTable.ThreadSafeRead(delegate { LogfileTable.TryGetValue(channelName, out logfileName); });
        if (!string.IsNullOrEmpty(logfileName))
        {
            result = new TraceLogFile(logfileName);
        }

        return result;
    }

    public static TraceChannelId StartChannel(string channelName)
    {
        TextWriterTraceListener listener = null;
        var wasCreated = false;
        _RegisteredListeners.ThreadSafeWrite(delegate
        {
            if (_RegisteredListeners.TryGetValue(channelName, out var listenerEntry))
            {
                Interlocked.Increment(ref listenerEntry.RefCount);
            }
            else
            {
                var now = DateTime.Now;
                var text = GenerateChannelFilename(channelName, now);
                listener = new TextWriterTraceListener(text, channelName);
                ListenerForCurrentThread = listener;
                listener.Filter = new SourceFilter(channelName);
                listener.TraceOutputOptions =
                    TraceOptions.DateTime | TraceOptions.ProcessId | TraceOptions.ThreadId;
                listenerEntry = new TraceListenerEntry(listener, text, now);
                _RegisteredListeners[channelName] = listenerEntry;
                if (LogfileTable.TryGetValue(channelName, out var value))
                {
                    try
                    {
                        File.Delete(value);
                    }
                    catch (Exception)
                    {
                    }
                }

                LogfileTable[channelName] = text;
                wasCreated = true;
            }
        });
        if (wasCreated)
        {
            Trace.Listeners.Add(listener);
            TraceInformation((TraceChannelId)channelName, $"{DateTime.Now}: Started log: {channelName}");
        }

        return (TraceChannelId)channelName;
    }

    public static void StopChannel(TraceChannelId channelId, bool shouldDelete = false,
        TimeSpan? longAgeThatOverridesDelete = null)
    {
        var channelName = (string)channelId;
        string fileName = null;
        TraceListenerEntry listenerEntry = null;
        var shouldRemove = false;
        _RegisteredListeners.ThreadSafeWrite(delegate
        {
            if (_RegisteredListeners.TryGetValue(channelName, out listenerEntry))
            {
                if (Interlocked.Decrement(ref listenerEntry.RefCount) == 0)
                {
                    _RegisteredListeners.Remove(channelName);
                    ListenerForCurrentThread = null;
                    shouldRemove = true;
                }

                fileName = listenerEntry.FileName;
            }
        });
        if (shouldRemove)
        {
            var now = DateTime.Now;
            TraceInformation((TraceChannelId)channelName, $"{now}: Stopped log: {channelName}");
            if (shouldDelete && longAgeThatOverridesDelete.HasValue)
            {
                var value = now - listenerEntry.Created;
                var timeSpan = longAgeThatOverridesDelete;
                shouldDelete = value > timeSpan;
            }

            if (!shouldDelete)
            {
                listenerEntry.Listener.Flush();
            }

            Trace.Listeners.Remove(listenerEntry.Listener);
            if (shouldDelete)
            {
                LogfileTable.Remove(channelName);
                listenerEntry.Listener.Writer.Close();
                listenerEntry.Listener.Writer.Dispose();
                listenerEntry.Listener.Writer = null;
                listenerEntry.Listener.Close();
                listenerEntry.Listener.Dispose();
                listenerEntry.Listener = null;
                File.Delete(fileName);
            }
        }
    }

    public static void TraceError(string format, params object[] args)
    {
        TraceError(null, format, args);
    }

    public static void TraceError(TraceChannelId channelId, string format, params object[] args)
    {
        var text = args == null ? format : string.Format(format, args);
        var traceListenerForChannel = GetTraceListenerForChannel(channelId);
        if (traceListenerForChannel != null)
        {
            traceListenerForChannel.WriteLine("*!* Error: " + text);
        }
        else
        {
            Trace.TraceError(text);
        }
    }

    public static void TraceInformation(string format, params object[] args)
    {
        TraceInformation(null, format, args);
    }

    public static void TraceInformation(TraceChannelId channelId, string format, params object[] args)
    {
        var text = args == null ? format : string.Format(format, args);
        var traceListenerForChannel = GetTraceListenerForChannel(channelId);
        if (traceListenerForChannel != null)
        {
            traceListenerForChannel.WriteLine("--- Information: " + text);
        }
        else
        {
            Trace.TraceInformation(text);
        }
    }

    public static void TraceWarning(string format, params object[] args)
    {
        TraceWarning(null, format, args);
    }

    public static void TraceWarning(TraceChannelId channelId, string format, params object[] args)
    {
        var text = args == null ? format : string.Format(format, args);
        var traceListenerForChannel = GetTraceListenerForChannel(channelId);
        if (traceListenerForChannel != null)
        {
            traceListenerForChannel.WriteLine("*** Warning: " + text);
        }
        else
        {
            Trace.TraceWarning(text);
        }
    }

    private static string GenerateChannelFilename(string channelName, DateTime? now = null)
    {
        var path = LogfileDirectory.Trim(Path.DirectorySeparatorChar);
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        return LogfileDirectory + channelName + (now ?? DateTime.Now).ToString("_yyyyMMdd_HHmmss") + ".log";
    }

    private static TraceListener GetTraceListenerForChannel(TraceChannelId channelId)
    {
        var channelName = (string)channelId;
        TraceListenerEntry listenerEntry = null;
        TraceListener result = Trace.Listeners.OfType<DefaultTraceListener>().FirstOrDefault();
        if (!string.IsNullOrEmpty(channelName) &&
            _RegisteredListeners.ThreadSafeRead(locked =>
                _RegisteredListeners.TryGetValue(channelName, out listenerEntry)) && listenerEntry?.Listener != null)
        {
            result = listenerEntry.Listener;
        }
        else if (ListenerForCurrentThread != null)
        {
            result = ListenerForCurrentThread;
        }

        return result;
    }

    private class TraceListenerEntry
    {
        internal readonly DateTime Created;
        internal readonly string FileName;
        internal TextWriterTraceListener Listener;
        internal int RefCount;

        internal TraceListenerEntry(TextWriterTraceListener listener, string fileName, DateTime? timeCreated = null)
        {
            Listener = listener;
            FileName = fileName;
            Created = timeCreated ?? DateTime.Now;
            RefCount = 1;
        }

        private TraceListenerEntry()
        {
        }
    }
}