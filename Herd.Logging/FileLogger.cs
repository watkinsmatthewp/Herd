using System;
using System.Collections.Generic;
using System.IO;

namespace Herd.Logging
{
    public class FileLogger : ILogger
    {
        private string _directoryPath = null;
        private bool _enforcedDirCreation = false;
        private ILogFormatter _formatter;

        public FileLogger(string directoryPath, ILogFormatter formatter)
        {
            _directoryPath = directoryPath;
            _formatter = formatter;
        }

        public void Log(Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            if (!_enforcedDirCreation && !Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
            var filePath = Path.Combine(_directoryPath, $"{DateTime.UtcNow.ToString("yyyy-MM-dd")}.log");
            File.AppendAllLines(filePath, new string[]
            {
                _formatter.GetLogLine(id, logLevel, message, contextParameters)
            });
        }
    }
}