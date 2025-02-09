using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubhadraSolutions.Utils.Threading.Tasks;

public static class TaskHelper
{
    public static Task<object> GetObjectFromValueTask(object valueTask)
    {
        return GetObjectFromTask<object>(valueTask);
    }

    public static async Task<T> GetObjectFromTask<T>(object taskObject)
    {
        Task task;
        var taskType = taskObject.GetType();
        if (IsValueTaskType(taskType))
        {
            var asTaskMethod = taskType.GetMethod("AsTask");
            task = (Task)asTaskMethod.Invoke(taskObject, Array.Empty<object>());
        }
        else
        {
            task = (Task)taskObject;
        }

        await task.ConfigureAwait(false);
        var getResultProperty = taskType.GetProperty("Result");
        var data = getResultProperty.GetValue(taskObject);
        return (T)data;
    }

    public static T GetResult<T>(Task<T> task)
    {
        task.Wait();
        return task.Result;
    }

    public static object GetResultFromTask(Task task)
    {
        var getResultProperty = task.GetType().GetProperty("Result");
        task.Wait();
        object result = null;
        if (getResultProperty != null)
        {
            result = getResultProperty.GetValue(task);
        }

        return result;
    }

    //public static async Task<T> InvokeAsync<T>(this MethodInfo methodInfo, object obj, params object[] parameters)
    //{
    //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
    //    await awaitable;
    //    return (T)awaitable.GetAwaiter().GetResult();
    //}

    //public static async Task InvokeAsync(this MethodInfo methodInfo, object obj, params object[] parameters)
    //{
    //    dynamic awaitable = methodInfo.Invoke(obj, parameters);
    //    await awaitable;
    //}

    public static bool IsValueTask(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        return IsValueTaskType(obj.GetType());
    }

    public static bool IsValueTaskType(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ValueTask<>);
    }

    public static void ExecuteTasks<T>(IEnumerable<Task<T>> tasks, Action<T> resultProcessor,
        int degreeOfParallelism)
    {
        var set = new List<Task<T>>();
        foreach (var task in tasks)
        {
            set.Add(task);
            if (set.Count >= degreeOfParallelism)
            {
                var index = Task.WaitAny(set.ToArray());
                var result = set[index].Result;
                set.RemoveAt(index);
                resultProcessor(result);
            }
        }

        while (set.Count > 0)
        {
            var index = Task.WaitAny(set.ToArray());
            var result = set[index].Result;
            set.RemoveAt(index);
            resultProcessor(result);
        }
    }

    public static void ExecuteTasks<T, TInput>(IEnumerable<Tuple<Task<T>, TInput>> tasksAndInputs,
        Action<T, TInput> resultProcessor, int degreeOfParallelism)
    {
        var set = new List<Tuple<Task<T>, TInput>>();
        foreach (var tuple in tasksAndInputs)
        {
            set.Add(tuple);
            if (set.Count >= degreeOfParallelism)
            {
                var index = Task.WaitAny(set.Select(x => x.Item1).ToArray());
                var completedTuple = set[index];
                var result = completedTuple.Item1.Result;
                set.RemoveAt(index);
                resultProcessor(result, completedTuple.Item2);
            }
        }

        while (set.Count > 0)
        {
            var index = Task.WaitAny(set.Select(x => x.Item1).ToArray());
            var completedTuple = set[index];
            var result = completedTuple.Item1.Result;
            set.RemoveAt(index);
            resultProcessor(result, completedTuple.Item2);
        }
    }
}