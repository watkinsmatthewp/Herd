using Moq;
using Herd.Logging;
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
            //(Guid? id, LogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
            ConsoleLogger consoleLogger = new ConsoleLogger(new DefaultLogFormatter());
            Shim consoleLoggerShim = Shim.Replace(() => consoleLogger.Log(Is.A<Guid?>(), Is.A<LogLevel>(), Is.A<string>(), Is.A<IEnumerable<KeyValuePair<string, string>>>())).With(
                delegate (ConsoleLogger @this) {
                    return true;
                });

            PoseContext.Isolate(() =>
            {
                // All code that executes within this block
                // is isolated and shimmed methods are replaced
                consoleLogger.Log(Guid.NewGuid(), LogLevel.Info, "messageText", new Dictionary<string, string>());
                Assert.True(true);
            }, consoleLoggerShim);
        }
    }
}
