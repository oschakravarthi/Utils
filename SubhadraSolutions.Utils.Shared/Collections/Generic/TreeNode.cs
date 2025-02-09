using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class TreeNode<T>
{
    internal TreeNode<T>[] children;
    internal int index, childCount;
    private TreeNodeCollection<T> nodes;
    internal bool nodesCleared;
    internal TreeNode<T> parent;
    internal Tree<T> tree;

    public TreeNode(T value)
    {
        Value = value;
    }

    public TreeNode(T value, TreeNode<T>[] children)
        : this(value)
    {
        Nodes.AddRange(children);
    }

    internal TreeNode(Tree<T> tree, T value)
        : this()
    {
        this.tree = tree;
        Value = value;
    }

    private TreeNode()
    {
    }

    public TreeNode<T> FirstNode
    {
        get
        {
            if (childCount == 0)
            {
                return null;
            }

            return children[0];
        }
    }

    public int Index => index;

    public TreeNode<T> LastNode
    {
        get
        {
            if (childCount == 0)
            {
                return null;
            }

            return children[childCount - 1];
        }
    }

    public int Level
    {
        get
        {
            if (Parent == null)
            {
                return 0;
            }

            return Parent.Level + 1;
        }
    }

    public TreeNode<T> NextNode
    {
        get
        {
            if (index + 1 < parent.Nodes.Count)
            {
                return parent.Nodes[index + 1];
            }

            return null;
        }
    }

    public TreeNodeCollection<T> Nodes
    {
        get { return nodes ??= new TreeNodeCollection<T>(this); }
    }

    public TreeNode<T> Parent
    {
        get
        {
            var t = Tree;

            if (t != null && parent == t.root)
            {
                return null;
            }

            return parent;
        }
    }

    public TreeNode<T> PrevNode
    {
        get
        {
            var num = index;
            var fixedIndex = parent.Nodes.FixedIndex;
            if (fixedIndex > 0)
            {
                num = fixedIndex;
            }

            if (num > 0 && num <= parent.Nodes.Count)
            {
                return parent.Nodes[num - 1];
            }

            return null;
        }
    }

    public Tree<T> Tree
    {
        get { return tree ??= FindTree(); }
    }

    public T Value { get; }

    public TreeNode<T> AddChild(T value)
    {
        var node = new TreeNode<T>(tree, value);
        Nodes.Add(node);
        return node;
    }

    public int GetNodeCount(bool includeSubTrees)
    {
        var num = childCount;

        if (includeSubTrees)
        {
            for (var i = 0; i < childCount; i++)
                num += children[i].GetNodeCount(true);
        }

        return num;
    }

    public void Remove()
    {
        Remove(true);
    }

    internal void Clear()
    {
        _ = Tree;

        try
        {
            while (childCount > 0) children[childCount - 1].Remove(true);

            children = null;
        }
        finally
        {
            nodesCleared = true;
        }
    }

    internal void EnsureCapacity(int num)
    {
        var num2 = num;
        if (num2 < 4)
        {
            num2 = 4;
        }

        if (children == null)
        {
            children = new TreeNode<T>[num2];
        }
        else if (childCount + num > children.Length)
        {
            var num3 = childCount + num;
            if (num == 1)
            {
                num3 = childCount * 2;
            }

            var destinationArray = new TreeNode<T>[num3];
            Array.Copy(children, 0, destinationArray, 0, childCount);
            children = destinationArray;
        }
    }

    internal Tree<T> FindTree()
    {
        var treeNode = this;

        while (treeNode.parent != null) treeNode = treeNode.parent;

        return treeNode.tree;
    }

    internal void InsertNodeAt(int index, TreeNode<T> node)
    {
        EnsureCapacity(1);
        node.parent = this;
        node.index = index;

        for (var num = childCount; num > index; num--) (children[num] = children[num - 1]).index = num;

        children[index] = node;
        childCount++;
    }

    internal void Remove(bool notify)
    {
        for (var i = 0; i < childCount; i++) children[i].Remove(false);

        if (notify && parent != null)
        {
            for (var j = index; j < parent.childCount - 1; j++) (parent.children[j] = parent.children[j + 1]).index = j;

            parent.children[parent.childCount - 1] = null;
            parent.childCount--;
            parent = null;
        }

        tree = null;
    }
}