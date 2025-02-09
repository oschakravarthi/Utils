using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace SubhadraSolutions.Utils.Collections.Specialized;

/// <summary>
///     Provides a dictionary for use with data binding.
/// </summary>
/// <typeparam name="TKey">Specifies the type of the keys in this collection.</typeparam>
/// <typeparam name="TValue">Specifies the type of the values in this collection.</typeparam>
[DebuggerDisplay("Count={Count}")]
[Serializable]
public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>,
    INotifyCollectionChanged, INotifyPropertyChanged
{
    private readonly IDictionary<TKey, TValue> dictionary;

    /// <summary>
    ///     Initializes an instance of the class.
    /// </summary>
    public ObservableDictionary()
        : this(new ConcurrentDictionary<TKey, TValue>())
    {
    }

    /// <summary>
    ///     Initializes an instance of the class using another dictionary as
    ///     the key/value store.
    /// </summary>
    public ObservableDictionary(IDictionary<TKey, TValue> dictionary)
    {
        this.dictionary = dictionary;
    }

    /// <summary>
    ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the
    ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    /// <returns>
    ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the keys of the object that implements
    ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </returns>
    public ICollection<TKey> Keys => dictionary.Keys;

    /// <summary>
    ///     Gets an <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the
    ///     <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    /// <returns>
    ///     An <see cref="T:System.Collections.Generic.ICollection`1" /> containing the values in the object that
    ///     implements <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </returns>
    public ICollection<TValue> Values => dictionary.Values;

    public int Count => dictionary.Count;

    public bool IsReadOnly => dictionary.IsReadOnly;

    /// <summary>
    ///     Gets or sets the element with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns></returns>
    public TValue this[TKey key]
    {
        get => dictionary[key];
        set => UpdateWithNotification(key, value);
    }

    /// <summary>
    ///     Adds an element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    /// <param name="key">The object to use as the key of the element to add.</param>
    /// <param name="value">The object to use as the value of the element to add.</param>
    public void Add(TKey key, TValue value)
    {
        AddWithNotification(key, value);
    }

    /// <summary>
    ///     Determines whether the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the
    ///     specified key.
    /// </summary>
    /// <param name="key">The key to locate in the <see cref="T:System.Collections.Generic.IDictionary`2" />.</param>
    /// <returns>
    ///     true if the <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element with the key; otherwise,
    ///     false.
    /// </returns>
    public bool ContainsKey(TKey key)
    {
        return dictionary.ContainsKey(key);
    }

    /// <summary>
    ///     Removes the element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </summary>
    /// <param name="key">The key of the element to remove.</param>
    /// <returns>
    ///     true if the element is successfully removed; otherwise, false.  This method also returns false if
    ///     <paramref name="key" /> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2" />.
    /// </returns>
    public bool Remove(TKey key)
    {
        return RemoveWithNotification(key);
    }

    /// <summary>
    ///     Gets the value associated with the specified key.
    /// </summary>
    /// <param name="key">The key whose value to get.</param>
    /// <param name="value">
    ///     When this method returns, the value associated with the specified key, if the key is found;
    ///     otherwise, the default value for the type of the <paramref name="value" /> parameter. This parameter is passed
    ///     uninitialized.
    /// </param>
    /// <returns>
    ///     true if the object that implements <see cref="T:System.Collections.Generic.IDictionary`2" /> contains an element
    ///     with the specified key; otherwise, false.
    /// </returns>
    public bool TryGetValue(TKey key, out TValue value)
    {
        return dictionary.TryGetValue(key, out value);
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        AddWithNotification(item);
    }

    public void Clear()
    {
        dictionary.Clear();

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        dictionary.CopyTo(array, arrayIndex);
    }

    IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return dictionary.GetEnumerator();
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        return RemoveWithNotification(item.Key);
    }

    /// <summary>Event raised when the collection changes.</summary>
    public event NotifyCollectionChangedEventHandler CollectionChanged = (sender, args) => { };

    /// <summary>Event raised when a property on the collection changes.</summary>
    public event PropertyChangedEventHandler PropertyChanged = (sender, args) => { };

    /// <summary>
    ///     Allows derived classes to raise custom property changed events.
    /// </summary>
    protected void RaisePropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }

    private void AddWithNotification(KeyValuePair<TKey, TValue> item)
    {
        AddWithNotification(item.Key, item.Value);
    }

    private void AddWithNotification(TKey key, TValue value)
    {
        dictionary[key] = value;

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
            new KeyValuePair<TKey, TValue>(key, value)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
    }

    private bool RemoveWithNotification(TKey key)
    {
        if (dictionary.TryGetValue(key, out var value) && dictionary.Remove(key))
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                new KeyValuePair<TKey, TValue>(key, value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Count"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Keys"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));

            return true;
        }

        return false;
    }

    private void UpdateWithNotification(TKey key, TValue value)
    {
        if (dictionary.TryGetValue(key, out var existing))
        {
            dictionary[key] = value;

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                new KeyValuePair<TKey, TValue>(key, value),
                new KeyValuePair<TKey, TValue>(key, existing)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Values"));
        }
        else
        {
            AddWithNotification(key, value);
        }
    }
}