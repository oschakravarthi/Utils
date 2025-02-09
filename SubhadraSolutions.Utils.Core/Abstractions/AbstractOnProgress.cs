using System;

namespace SubhadraSolutions.Utils.Abstractions;

public abstract class AbstractOnProgress
{
    public event EventHandler<GenericEventArgs<string>> OnProgress;

    protected void NotifyOnProgress(string status = null)
    {
        var subscribers = OnProgress;
        if (subscribers != null)
        {
            subscribers(this, new GenericEventArgs<string>(status));
        }
    }
}