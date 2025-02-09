using SubhadraSolutions.Utils.Threading;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class ListNotifyCollectionChangedAdapter<T>(IList<T> adaptedList) : AbstractListDecorator<T>(adaptedList),
    INotifyCollectionChanged,
    INotifyPropertyChanged
{
    private readonly IAtomicOperationSupported adaptedListAsAtomicOperationSupported = adaptedList as IAtomicOperationSupported;

    public override T this[int index]
    {
        set
        {
            var action = new Action(delegate
            {
                var oldValue = base[index];
                base[index] = value;
                OnPropertyChanged(new PropertyChangedEventArgs("Item[]"));
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                    oldValue, value, index));
            });
            PerformAction(action);
        }
    }

    public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

    public virtual event PropertyChangedEventHandler PropertyChanged;

    public override void Add(T item)
    {
        var action = new Action(delegate
        {
            base.Add(item);
            var index = base.Count - 1;

            OnCountChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        });
        PerformAction(action);
    }

    public override void Clear()
    {
        var action = new Action(delegate
        {
            base.Clear();
            OnCountChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        });
        PerformAction(action);
    }

    public override void Insert(int index, T item)
    {
        var action = new Action(delegate
        {
            base.Insert(index, item);
            OnCountChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        });
        PerformAction(action);
    }

    public override bool Remove(T item)
    {
        var index = IndexOf(item);
        if (index > -1)
        {
            var action = new Action(delegate
            {
                var returnValue = base.Remove(item);
                if (returnValue)
                {
                    OnCountChanged();
                    OnCollectionChanged(
                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
                }
            });
            PerformAction(action);
            return true;
        }

        return false;
    }

    public override void RemoveAt(int index)
    {
        var action = new Action(delegate
        {
            var item = base[index];
            base.RemoveAt(index);
            OnCollectionChanged(
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            OnCountChanged();
        });
        PerformAction(action);
    }

    protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        var collectionChanged = CollectionChanged;
        if (collectionChanged != null)
        {
            collectionChanged(this, e);
        }
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        var propertyChanged = PropertyChanged;
        if (propertyChanged != null)
        {
            propertyChanged(this, e);
        }
    }

    private void OnCountChanged()
    {
        var countChangedEventArgs = new PropertyChangedEventArgs(nameof(Count));
        var itemArrayChangedEventArgs = new PropertyChangedEventArgs("Item[]");
        OnPropertyChanged(countChangedEventArgs);
        OnPropertyChanged(itemArrayChangedEventArgs);
    }

    private void PerformAction(Action action)
    {
        if (adaptedListAsAtomicOperationSupported == null)
        {
            action();
        }
        else
        {
            adaptedListAsAtomicOperationSupported.PerformAtomicOperation(action);
        }
    }
}