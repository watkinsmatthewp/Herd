using Herd.Business;
using Herd.Data.Providers;
using Herd.Logging;
using System;
using System.Diagnostics;
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

        private Lazy<IHashTagRelevanceManager> _hashTagRelevanceManager;
        public IHashTagRelevanceManager HashTagRelevanceManager => _hashTagRelevanceManager.Value;

        private HerdWebApp()
        {
            _logger = new Lazy<ILogger>(BuildLogger);
            _dataProvider = new Lazy<IDataProvider>(BuildDataProvider);
            _hashTagRelevanceManager = new Lazy<IHashTagRelevanceManager>(BuildHashTagRelevanceManager);
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

        private IHashTagRelevanceManager BuildHashTagRelevanceManager() => new HashTagRelevanceManager(BuildHashTagRelevanceManagerConfiguration(), DataProvider);

        private HashTagRelevanceManagerConfiguration BuildHashTagRelevanceManagerConfiguration()
        {
            var config = new HashTagRelevanceManagerConfiguration();
            if (Debugger.IsAttached)
            {
                config.TimeFlushInterval = null;
                config.DataFlushPostInterval = 2;
            }
            return config;
        }
    }
}