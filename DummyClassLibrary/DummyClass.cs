using System.Threading;

namespace DummyClassLibrary;

public class DummyClass
{
    public static void DummyMethod()
    {
        Thread.Sleep(100);
        //throw new Exception("Some exception");
    }
}