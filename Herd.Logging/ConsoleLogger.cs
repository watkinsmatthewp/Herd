using System;
using System.Collections.Generic;

namespace Herd.Logging
{
    public class ConsoleLogger : ILogger
    {
        private ILogFormatter _formatter;

        public ConsoleLogger(ILogFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Log(Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            Console.WriteLine(_formatter.GetLogLine(id, logLevel, message, contextParameters));
        }
    }
}