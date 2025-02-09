using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class LinkedStack<T>
{
    private Node<T> first;

    public int Count { get; private set; }

    public bool IsEmpty => first == null;

    public T Peek()
    {
        if (IsEmpty)
        {
            throw new Exception("Stack is empty");
        }

        return first.Data;
    }

    public T Pop()
    {
        if (IsEmpty)
        {
            throw new Exception("Stack is empty");
        }

        var ell = first.Data;
        first = first.Next;
        Count--;
        return ell;
    }

    public void Push(T obj)
    {
        if (first == null)
        {
            first = new Node<T>(obj);
        }
        else
        {
            first = new Node<T>(obj, first);
        }

        Count++;
    }
}