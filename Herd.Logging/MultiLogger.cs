using System;
using System.Collections.Generic;

namespace Herd.Logging
{
    public class MultiLogger : ILogger
    {
        public List<ILogger> Loggers { get; } = new List<ILogger>();

        public void Log(Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            foreach (var logger in Loggers)
            {
                try
                {
                    logger.Log(id, logLevel, message, contextParameters);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}