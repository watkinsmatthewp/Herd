using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Logging
{
    public class HerdConsoleLogger : IHerdLogger
    {
        private IHerdLogFormatter _formatter;

        public HerdConsoleLogger(IHerdLogFormatter formatter)
        {
            _formatter = formatter;
        }

        public void Log(Guid? id, HerdLogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            Console.WriteLine(_formatter.GetLogLine(id, logLevel, message, contextParameters));
        }
    }
}
