using System;

namespace SubhadraSolutions.Utils.Contracts
{
    public interface IExceptioned
    {
        Exception Exception { get; }
    }
}