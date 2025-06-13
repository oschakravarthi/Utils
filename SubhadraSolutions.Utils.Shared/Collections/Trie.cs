using System;
using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Shared.Collections
{
    
    public class Trie<T>
    {
        private class TrieNode
        {
            public Dictionary<char, TrieNode> Children = new();
            public List<T> Values = new(); // store multiple addresses
            public bool IsEndOfKey = false;
        }

        private readonly TrieNode root = new();

        public Trie()
        {
        }
        public void Add(string name, T value)
        {
            var node = root;
            foreach (char ch in name.ToLowerInvariant()) // case-insensitive
            {
                if (!node.Children.ContainsKey(ch))
                    node.Children[ch] = new TrieNode();
                node = node.Children[ch];
            }

            node.IsEndOfKey = true;
            node.Values.Add(value);
        }

        // Find all names starting with a prefix
        public Dictionary<string, List<T>> SearchByPrefix(string prefix)
        {
            var results = new Dictionary<string, List<T>>();
            var node = root;

            foreach (char ch in prefix.ToLowerInvariant())
            {
                if (!node.Children.ContainsKey(ch))
                    return results; // prefix not found
                node = node.Children[ch];
            }

            DFS(node, prefix.ToLowerInvariant(), results);
            return results;
        }

        // DFS traversal to collect all names with this prefix
        private void DFS(TrieNode node, string current, Dictionary<string, List<T>> results)
        {
            if (node.IsEndOfKey)
            {
                results[current] = new List<T>(node.Values);
            }

            foreach (var pair in node.Children)
            {
                DFS(pair.Value, current + pair.Key, results);
            }
        }
    }

}