namespace SubhadraSolutions.Utils.Contracts;

public interface IPreviousAndNext<out T>
{
    T GetNext();

    T GetPrevious();
}