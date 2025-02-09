using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class Tree<T>
{
    internal TreeNodeCollection<T> nodes;

    internal TreeNode<T> root;

    public Tree(T rootvalue)
    {
        root = new TreeNode<T>(this, rootvalue);
    }

    public Tree(TreeNode<T> rootNode)
    {
        root = rootNode;
        SetTreeRecursively(rootNode, this);
    }

    public TreeNodeCollection<T> Nodes
    {
        get { return nodes ??= new TreeNodeCollection<T>(root); }
    }

    public TreeNode<T> Root => root;

    public TreeNode<T> Find(T value, IEqualityComparer<T> comparer)
    {
        return FindRecursively(value, root, comparer);
    }

    private TreeNode<T> FindRecursively(T value, TreeNode<T> node, IEqualityComparer<T> comparer)
    {
        if (comparer.Equals(node.Value, value))
        {
            return node;
        }

        for (var i = 0; i < node.childCount; i++)
        {
            var result = FindRecursively(value, node.children[i], comparer);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    private void SetTreeRecursively(TreeNode<T> node, Tree<T> tree)
    {
        node.tree = tree;

        for (var i = 0; i < node.childCount; i++) SetTreeRecursively(node.children[i], tree);
    }
}