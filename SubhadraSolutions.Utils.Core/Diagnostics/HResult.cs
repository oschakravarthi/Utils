using System.ComponentModel;

namespace SubhadraSolutions.Utils.Diagnostics;

public enum HResult
{
    [Description("The operation completed successfully.")]
    S_OK = 0,

    [Description("Access is denied to connect to the Task Scheduler service.")]
    E_ACCESS_DENIED = -2147024891,

    [Description(
        "The application does not have enough memory to complete the operation or the user or password has at least one null and one non-null value.")]
    E_OUTOFMEMORY = -2147024882,

    [Description("The task XML contains a value which is incorrectly formatted or out of range.")]
    SCHED_E_INVALIDVALUE = -2147216616,

    [Description("The task has not yet run.")]
    SCHED_S_TASK_HAS_NOT_RUN = 267011,

    [Description("The task is registered, but not all specified triggers will start the task.")]
    SCHED_S_SOME_TRIGGERS_FAILED = 267035,

    [Description(
        "The task is registered, but may fail to start. Batch logon privilege needs to be enabled for the task principal.")]
    SCHED_S_BATCH_LOGON_PROBLEM = 267036,

    [Description("A generic exception occurred. (Base class for all exceptions in the runtime.)")]
    COR_E_EXCEPTION = -2146233088
}