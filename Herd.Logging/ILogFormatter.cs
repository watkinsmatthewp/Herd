using System;
using System.Collections.Generic;

namespace Herd.Logging
{
    public interface ILogFormatter
    {
        string GetLogLine(Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null);
    }
}