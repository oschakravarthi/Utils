namespace SubhadraSolutions.Utils.Collections.Generic;

internal class Node<T>(T data)
{
    public Node(T data, Node<T> next)
        : this(data)
    {
        Next = next;
    }

    public T Data { get; set; } = data;
    public Node<T> Next { get; set; }
}