namespace SubhadraSolutions.Utils.Algorithms;

public struct GroupMergeInfo<T>
{
    public GroupMergeInfo(double proximity, T from, T to)
    {
        From = from;
        To = to;
        Proximity = proximity;
    }

    public T From { get; private set; }
    public double Proximity { get; }
    public T To { get; private set; }
}