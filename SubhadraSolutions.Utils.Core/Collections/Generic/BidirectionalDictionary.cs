using System.Collections.Generic;

namespace SubhadraSolutions.Utils.Collections.Generic;

public class BidirectionalDictionary<TFirst, TSecond>
{
    public BidirectionalDictionary()
    {
        FirstToSecondDictionary = [];
        SecondToFirstDictionary = [];
    }

    public BidirectionalDictionary(Dictionary<TFirst, TSecond> firstToSecondDictionary)
        : this()
    {
        foreach (var current in firstToSecondDictionary.Keys)
        {
            AddValue(current, firstToSecondDictionary[current]);
        }
    }

    internal Dictionary<TFirst, TSecond> FirstToSecondDictionary { get; set; }

    internal Dictionary<TSecond, TFirst> SecondToFirstDictionary { get; set; }

    public void AddValue(TFirst firstValue, TSecond secondValue)
    {
        FirstToSecondDictionary.Add(firstValue, secondValue);
        if (!SecondToFirstDictionary.ContainsKey(secondValue))
        {
            SecondToFirstDictionary.Add(secondValue, firstValue);
        }
    }

    public virtual bool ExistsInFirst(TFirst value)
    {
        return FirstToSecondDictionary.ContainsKey(value);
    }

    public virtual bool ExistsInSecond(TSecond value)
    {
        return SecondToFirstDictionary.ContainsKey(value);
    }

    public virtual TFirst GetFirstValue(TSecond value)
    {
        if (ExistsInSecond(value))
        {
            return SecondToFirstDictionary[value];
        }

        return default;
    }

    public virtual TSecond GetSecondValue(TFirst value)
    {
        if (ExistsInFirst(value))
        {
            return FirstToSecondDictionary[value];
        }

        return default;
    }
}