using Microsoft.AspNetCore.Components;
using MudBlazor.Docs.Services;
using System;

namespace MudBlazor.Docs.Components;

public partial class QueuedContent : ComponentBase, IDisposable
{
    [Inject] private IRenderQueueService RenderQueue { get; set; }

    [Parameter] public RenderFragment ChildContent { get; set; }
    [Parameter] public bool RenderImmediately { get; set; }

    public bool IsDisposed { get; private set; }

    public bool IsRendered { get; private set; }

    public void Dispose()
    {
        IsDisposed = true;
        Rendered = null;
        if (!IsRendered)
        {
            Disposed?.Invoke(this);
        }

        Disposed = null;
    }

    public event Action<QueuedContent> Rendered;

    public event Action<QueuedContent> Disposed;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (!RenderImmediately)
        {
            RenderQueue.Enqueue(this);
        }
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
        if (IsDisposed)
        {
            return;
        }

        if (IsRendered && Rendered != null)
        {
            Rendered?.Invoke(this);
            Rendered = null;
        }
    }

    public void Render()
    {
        if (IsDisposed)
        {
            return;
        }

        IsRendered = true;
        InvokeAsync(StateHasChanged);
    }
}