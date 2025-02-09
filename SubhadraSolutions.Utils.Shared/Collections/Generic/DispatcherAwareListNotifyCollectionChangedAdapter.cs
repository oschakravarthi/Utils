using SubhadraSolutions.Utils.Threading;
using SubhadraSolutions.Utils.Validation;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class DispatcherAwareListNotifyCollectionChangedAdapter<T> : ListNotifyCollectionChangedAdapter<T>
{
    private readonly IDispatcherProvider dispatcherProvider;

    public DispatcherAwareListNotifyCollectionChangedAdapter(IList<T> adaptedList,
        IDispatcherProvider dispatcherProvider)
        : base(adaptedList)
    {
        Guard.ArgumentShouldNotBeNull(dispatcherProvider, nameof(dispatcherProvider));
        this.dispatcherProvider = dispatcherProvider;
    }

    public override event NotifyCollectionChangedEventHandler CollectionChanged
    {
        add
        {
            IDispatcher dispatcher = null;
            if (dispatcherProvider != null)
            {
                dispatcher = dispatcherProvider.GetDispatcher(value.Target);
            }

            if (dispatcher == null)
            {
                collectionChanged += value;
            }
            else
            {
                collectionChanged += (sender, e) => dispatcher.Dispatch(() => value(sender, e));
            }
        }

        remove => collectionChanged -= value;
    }

    public override event PropertyChangedEventHandler PropertyChanged
    {
        add
        {
            IDispatcher dispatcher = null;
            if (dispatcherProvider != null)
            {
                dispatcher = dispatcherProvider.GetDispatcher(value.Target);
            }

            if (dispatcher == null)
            {
                propertyChanged += value;
            }
            else
            {
                propertyChanged += (sender, e) => dispatcher.Dispatch(() => value(sender, e));
            }
        }

        remove => propertyChanged -= value;
    }

    private event NotifyCollectionChangedEventHandler collectionChanged;

    private event PropertyChangedEventHandler propertyChanged;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        GeneralHelper.SafeInvoke(collectionChanged, this, e);
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        GeneralHelper.SafeInvoke(propertyChanged, this, e);
    }
}