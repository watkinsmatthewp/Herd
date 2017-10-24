using Xunit;

namespace Herd.Logging.UnitTests
{
    public class FileLoggerTests
    {
        [Fact]
        public void TestLog()
        {
            //var outputHandler = new OutputHandler();
            //var fileLogger = new FileLogger(Path.Combine(Path.GetTempPath(), "HerdLogs"), new DefaultLogFormatter());

            //Shim fileLoggerShim = Shim.Replace(() => fileLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(),
            //                                                        Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
            //    delegate (FileLogger @this, Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
            //    {
            //        outputHandler.AddFileOutput(new DefaultLogFormatter().GetLogLine(id, logLevel, message, contextParameters));
            //    }
            //);

            //PoseContext.Isolate(() =>
            //{
            //    fileLogger.Log(Guid.NewGuid(), LogLevel.Info, "file-messageText", new Dictionary<string, string>());
            //    Assert.Contains("file-messageText", outputHandler.FileOutput);
            //}, fileLoggerShim);
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