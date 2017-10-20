using Pose;
using System;
using Xunit;
using System.Collections.Generic;

namespace Herd.Logging.UnitTests
{
    public class ConsoleLoggerTests
    {

        [Fact]
        public void TestLog()
        {
            var outputHandler = new OutputHandler();
            var consoleLogger = new ConsoleLogger(new DefaultLogFormatter());
            Shim consoleLoggerShim = Shim.Replace(() => consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(),
                                                                          Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
                delegate (ConsoleLogger @this, Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters)
                {
                    outputHandler.AddConsoleOutput(new DefaultLogFormatter().GetLogLine(id, logLevel, message, contextParameters));
                }
            );

            PoseContext.Isolate(() =>
            {
                consoleLogger.Log(Guid.NewGuid(), LogLevel.Info, "console-messageText", new Dictionary<string, string>());
                Assert.Contains("console-messageText", outputHandler.ConsoleOutput);
            }, consoleLoggerShim);
            
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
