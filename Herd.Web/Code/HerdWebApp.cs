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

        private Lazy<IHerdLogger> _logger;
        public IHerdLogger Logger => _logger.Value;

        private Lazy<IHerdDataProvider> _dataProvider;
        public IHerdDataProvider DataProvider => _dataProvider.Value;

        private HerdWebApp()
        {
            _logger = new Lazy<IHerdLogger>(BuildLogger);
            _dataProvider = new Lazy<IHerdDataProvider>(BuildDataProvider);
        }

        private IHerdLogger BuildLogger()
        {
            var formatter = new HerdDefaultLogFormatter();
            var logger = new HerdMultiLogger();
            logger.Loggers.Add(new HerdConsoleLogger(formatter));
            logger.Loggers.Add(new HerdFileLogger(Path.Combine(Path.GetTempPath(), "HerdLogs"), formatter));
            return logger;
        }

        private IHerdDataProvider BuildDataProvider() => new HerdFileDataProvider();
    }
}