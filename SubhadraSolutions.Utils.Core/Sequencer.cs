namespace SubhadraSolutions.Utils;

public class Sequencer
{
    private int value;

    public int Next
    {
        get
        {
            var toReturn = value;
            value++;
            return toReturn;
        }
    }
}