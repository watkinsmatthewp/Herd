using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Logging
{
    public class HerdMultiLogger : IHerdLogger
    {
        public List<IHerdLogger> Loggers { get; private set; } = new List<IHerdLogger>();

        public void Log(Guid? id, HerdLogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            foreach (var logger in Loggers)
            {
                try
                {
                    logger.Log(id, logLevel, message, contextParameters);
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
