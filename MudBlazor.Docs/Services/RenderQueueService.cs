// Copyright (c) MudBlazor 2021
// MudBlazor licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using MudBlazor.Docs.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MudBlazor.Docs.Services;

public interface IRenderQueueService
{
    int Capacity { get; }

    void Clear();

    void Enqueue(QueuedContent component);

    Task WaitUntilEmpty();
}

public class RenderQueueService : IRenderQueueService
{
    private readonly Queue<QueuedContent> _queue = new();
    private TaskCompletionSource _tcs;

    public int Capacity { get; init; } = 3;

    public void Clear()
    {
        lock (_queue)
        {
            _queue.Clear();
            _tcs?.TrySetResult();
            _tcs = null;
        }
    }

    public Task WaitUntilEmpty()
    {
        lock (_queue)
        {
            if (_queue.Count == 0)
            {
                return Task.CompletedTask;
            }

            if (_tcs == null)
            {
                _tcs = new TaskCompletionSource();
            }

            return _tcs.Task;
        }
    }

    void IRenderQueueService.Enqueue(QueuedContent component)
    {
        var renderImmediately = false;
        lock (_queue)
        {
            renderImmediately = _queue.Count == 0;
            _queue.Enqueue(component);
            component.Rendered += OnComponentRendered;
            component.Disposed += OnComponentDisposed;
        }

        if (renderImmediately)
        {
            component.Render();
        }
    }

    private void OnComponentDisposed(QueuedContent component)
    {
        //return RenderNext();
    }

    private void OnComponentRendered(QueuedContent component)
    {
        //return RenderNext();
    }

    private async Task RenderNext()
    {
        QueuedContent componentToRender = null;
        lock (_queue)
        {
            while (_queue.Count > 0)
            {
                var component = _queue.Dequeue();
                if (component.IsDisposed || component.IsRendered)
                {
                    component.Rendered -= OnComponentRendered;
                    component.Disposed -= OnComponentDisposed;
                    continue;
                }

                componentToRender = component;
                break;
            }

            if (componentToRender == null)
            {
                _tcs?.TrySetResult();
                _tcs = null;
                return;
            }
        }

        await Task.Delay(1).ConfigureAwait(false);
        componentToRender.Render();
    }
}