using Xunit;

namespace Herd.Logging.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void TestInfo()
        {
            //var consoleLogger = new ConsoleLogger(new DefaultLogFormatter());
            //var logger = new Mock<ConsoleLogger>();
            //var outputHandler = new OutputHandler();

            //Shim consoleLoggerShim = Shim.Replace(() =>
            //    consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(), Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
            //        delegate (ConsoleLogger @this, Guid? id, LogLevel level, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
            //        {
            //            outputHandler.AddConsoleOutput(new DefaultLogFormatter().GetLogLine(id, level, message, contextParameters));
            //        }
            //);

            //PoseContext.Isolate(() =>
            //{
            //    //logger.Object.Info(new Guid(), "new info message", new Dictionary<string, string>(), null);
            //    consoleLogger.Info(new Guid(), "new info message", new Dictionary<string, string>(), null);
            //}, consoleLoggerShim);
            //Assert.Contains("new info message", outputHandler.ConsoleOutput);
        }

        [Fact]
        public void TestDebug()
        {
            //var logger = new ConsoleLogger(new DefaultLogFormatter());
            //var outputHandler = new OutputHandler();
            //var consoleLogger = new ConsoleLogger(new DefaultLogFormatter());

            //Shim consoleLoggerShim = Shim.Replace(() => consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(),
            //                                                              Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
            //    delegate (ConsoleLogger @this, Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
            //    {
            //        outputHandler.AddConsoleOutput(new DefaultLogFormatter().GetLogLine(id, logLevel, message, contextParameters));
            //    }
            //);

            //PoseContext.Isolate(() =>
            //{
            //    consoleLogger.Debug(new Guid(), "new debug message", new Dictionary<string, string>(), null);
            //    Assert.Contains("new debug message", outputHandler.ConsoleOutput);
            //}, consoleLoggerShim);
        }

        [Fact]
        public void TestError()
        {
            //var logger = new ConsoleLogger(new DefaultLogFormatter());
            //var outputHandler = new OutputHandler();
            //var consoleLogger = new ConsoleLogger(new DefaultLogFormatter());

            //Shim consoleLoggerShim = Shim.Replace(() => consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(),
            //                                                              Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
            //    delegate (ConsoleLogger @this, Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
            //    {
            //        outputHandler.AddConsoleOutput(new DefaultLogFormatter().GetLogLine(id, logLevel, message, contextParameters));
            //    }
            //);

            //PoseContext.Isolate(() =>
            //{
            //    consoleLogger.Error(new Guid(), "new error message", new Dictionary<string, string>(), null);
            //    Assert.Contains("new error message", outputHandler.ConsoleOutput);
            //}, consoleLoggerShim);
        }

        [Fact]
        public void LogWithException()
        {
            //var logger = new ConsoleLogger(new DefaultLogFormatter());
            //var outputHandler = new OutputHandler();
            //var consoleLogger = new ConsoleLogger(new DefaultLogFormatter());

            //Shim consoleLoggerShim = Shim.Replace(() => consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(),
            //                                                              Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
            //    delegate (ConsoleLogger @this, Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
            //    {
            //        outputHandler.AddConsoleOutput(new DefaultLogFormatter().GetLogLine(id, logLevel, message, contextParameters));
            //    }
            //);

            //PoseContext.Isolate(() =>
            //{
            //    consoleLogger.Error(new Guid(), "new error message", new Dictionary<string, string>(), new Exception("whoops"));
            //    Assert.Contains("new error message", outputHandler.ConsoleOutput);
            //}, consoleLoggerShim);
        }

        #region Private

        public class OutputHandler
        {
            public string FileOutput { get; set; } = string.Empty;
            public string ConsoleOutput { get; set; } = string.Empty;

            public void AddFileOutput(string output)
            {
                this.FileOutput += output;
            }

            public void AddConsoleOutput(string output)
            {
                this.ConsoleOutput += output;
            }
        }

        #endregion Private

    }
}