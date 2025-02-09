using System;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class LinkedQueue<T>
{
    private Node<T> _head;
    private Node<T> _tail;

    public int Count { get; private set; }

    public T Dequeue()
    {
        if (_head == null)
        {
            throw new Exception("Queue is Empty");
        }

        var _result = _head.Data;
        _head = _head.Next;
        Count--;
        return _result;
    }

    public void Enqueue(T data)
    {
        var _newNode = new Node<T>(data);
        if (_head == null)
        {
            _head = _newNode;
            _tail = _head;
        }
        else
        {
            _tail.Next = _newNode;
            _tail = _tail.Next;
        }

        Count++;
    }
}