using Herd.Data.Providers;
using Herd.Logging;
using System;
using System.IO;

namespace Herd.Web.Code
{
    public class HerdWebApp
    {
        private static HerdWebApp _instance = new HerdWebApp();
        public static HerdWebApp Instance => _instance;

        private Lazy<ILogger> _logger;
        public ILogger Logger => _logger.Value;

        private Lazy<IDataProvider> _dataProvider;
        public IDataProvider DataProvider => _dataProvider.Value;

        private HerdWebApp()
        {
            _logger = new Lazy<ILogger>(BuildLogger);
            _dataProvider = new Lazy<IDataProvider>(BuildDataProvider);
        }

        private ILogger BuildLogger()
        {
            var formatter = new DefaultLogFormatter();
            var logger = new MultiLogger();
            logger.Loggers.Add(new ConsoleLogger(formatter));
            logger.Loggers.Add(new FileLogger(Path.Combine(Path.GetTempPath(), "HerdLogs"), formatter));
            return logger;
        }

        private IDataProvider BuildDataProvider() => new FileDataProvider();
    }
}