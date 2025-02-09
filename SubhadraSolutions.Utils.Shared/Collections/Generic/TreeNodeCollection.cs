using SubhadraSolutions.Utils.Contracts;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class TreeNodeCollection<T> : IList<TreeNode<T>>, IClearable
{
    private readonly TreeNode<T> owner;

    internal TreeNodeCollection(TreeNode<T> owner)
    {
        this.owner = owner;
    }

    internal int FixedIndex { get; set; } = -1;

    public virtual TreeNode<T> this[T key, IEqualityComparer<T> comparer]
    {
        get
        {
            var index = IndexOfKey(key, comparer);
            if (IsValidIndex(index))
            {
                return this[index];
            }

            return null;
        }
    }

    public int Count => owner.childCount;
    public bool IsReadOnly => false;

    public virtual TreeNode<T> this[int index]
    {
        get
        {
            if (index < 0 || index >= owner.childCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            return owner.children[index];
        }
        set
        {
            if (index < 0 || index >= owner.childCount)
            {
                throw new ArgumentOutOfRangeException(nameof(index), "Argument not in valid range");
            }

            value.parent = owner;
            value.index = index;
            owner.children[index] = value;
        }
    }

    public void Add(TreeNode<T> node)
    {
        AddInternal(node, 0);
    }

    public virtual void Clear()
    {
        owner.Clear();
    }

    public bool Contains(TreeNode<T> node)
    {
        return IndexOf(node) != -1;
    }

    public void CopyTo(TreeNode<T>[] array, int arrayIndex)
    {
        if (owner.childCount > 0)
        {
            Array.Copy(owner.children, 0, array, arrayIndex, owner.childCount);
        }
    }

    public IEnumerator<TreeNode<T>> GetEnumerator()
    {
        return new ArraySubsetEnumerator<TreeNode<T>>(owner.children, owner.childCount);
    }

    public int IndexOf(TreeNode<T> node)
    {
        for (var i = 0; i < Count; i++)
            if (this[i] == node)
            {
                return i;
            }

        return -1;
    }

    public virtual void Insert(int index, TreeNode<T> node)
    {
        _ = owner.Tree;
        if (index < 0)
        {
            index = 0;
        }

        if (index > owner.childCount)
        {
            index = owner.childCount;
        }

        owner.InsertNodeAt(index, node);
    }

    public bool Remove(TreeNode<T> node)
    {
        node.Remove();
        return true;
    }

    public virtual void RemoveAt(int index)
    {
        this[index].Remove();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public virtual TreeNode<T> Add(T value)
    {
        var treeNode = new TreeNode<T>(value);
        Add(treeNode);
        return treeNode;
    }

    public virtual void AddRange(IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            Add(value);
        }
    }

    public virtual void AddRange(TreeNode<T>[] nodes)
    {
        if (nodes == null)
        {
            throw new ArgumentNullException(nameof(nodes));
        }

        if (nodes.Length != 0)
        {
            _ = owner.Tree;
            owner.Nodes.FixedIndex = owner.childCount;
            owner.EnsureCapacity(nodes.Length);

            for (var num = nodes.Length - 1; num >= 0; num--) AddInternal(nodes[num], num);

            owner.Nodes.FixedIndex = -1;
        }
    }

    public virtual int IndexOfKey(T key, IEqualityComparer<T> comparer)
    {
        for (var i = 0; i < Count; i++)
            if (comparer.Equals(this[i].Value, key))
            {
                return i;
            }

        return -1;
    }

    public virtual TreeNode<T> Insert(int index, T value)
    {
        var treeNode = new TreeNode<T>(value);
        Insert(index, treeNode);
        return treeNode;
    }

    public virtual void RemoveByKey(T key, IEqualityComparer<T> comparer)
    {
        var index = IndexOfKey(key, comparer);
        if (IsValidIndex(index))
        {
            RemoveAt(index);
        }
    }

    private int AddInternal(TreeNode<T> node, int delta)
    {
        if (node == null)
        {
            throw new ArgumentNullException(nameof(node));
        }

        _ = owner.Tree;
        node.parent = owner;
        var num = owner.Nodes.FixedIndex;

        if (num != -1)
        {
            node.index = num + delta;
        }
        else
        {
            owner.EnsureCapacity(1);
            node.index = owner.childCount;
        }

        owner.children[node.index] = node;
        owner.childCount++;
        return node.index;
    }

    private bool IsValidIndex(int index)
    {
        if (index >= 0)
        {
            return index < Count;
        }

        return false;
    }
}