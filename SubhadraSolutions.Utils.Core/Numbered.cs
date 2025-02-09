namespace SubhadraSolutions.Utils;

public class Numbered<T>
{
    public Numbered(int number, T info)
    {
        Number = number;
        Info = info;
    }

    public T Info { get; private set; }
    public int Number { get; }
}