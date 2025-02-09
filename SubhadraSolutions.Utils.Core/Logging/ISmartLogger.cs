using Microsoft.Extensions.Logging;
using System;

namespace SubhadraSolutions.Utils.Logging;

public interface ISmartLogger : ILogger
{
    event EventHandler<GenericEventArgs<Exception>> OnExceptionLogged;
}