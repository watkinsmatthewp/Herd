using Moq;
using Herd.Logging;
using Pose;
using System;
using Xunit;
using System.Collections.Generic;
using System.IO;

namespace Herd.Logging.UnitTests
{
    public class MultiLoggerTests
    {
        [Fact]
        public void TestLogWithLoggers()
        {
            var multiLogger = new MultiLogger();
            var mockConsoleLogger = new Mock<ILogger>();
            var mockFileLogger = new Mock<ILogger>();

            Assert.Empty(multiLogger.Loggers);
            multiLogger.Loggers.Add(mockConsoleLogger.Object);
            Assert.Single(multiLogger.Loggers);
            multiLogger.Loggers.Add(mockFileLogger.Object);
            Assert.Equal(2, multiLogger.Loggers.Count);

            var id = new Guid();
            var logLevel = LogLevel.Info;
            var message = "Test message";
            var dict = new Dictionary<string, string>();
            multiLogger.Log(id, logLevel, message, dict);

            // Assert
            mockConsoleLogger.Verify(mc => mc.Log(id, logLevel, message, dict), Times.Once());
            mockFileLogger.Verify(mc => mc.Log(id, logLevel, message, dict), Times.Once());
        }

        [Fact]
        public void TestLogWithNoLoggers()
        {
            var multiLogger = new MultiLogger();

            Assert.Empty(multiLogger.Loggers);

            var id = new Guid();
            var logLevel = LogLevel.Info;
            var message = "Test message";
            var dict = new Dictionary<string, string>();
            multiLogger.Log(id, logLevel, message, dict);

            // Assert
            // This shouldn't call anything
        }

        [Fact]
        public void TestExceptionThrown()
        {
            var multiLogger = new MultiLogger();
            var mockConsoleLogger = new Mock<ILogger>();
            var mockFileLogger = new Mock<ILogger>();

            Assert.Empty(multiLogger.Loggers);
            multiLogger.Loggers.Add(mockConsoleLogger.Object);
            Assert.Single(multiLogger.Loggers);
            multiLogger.Loggers.Add(mockFileLogger.Object);
            Assert.Equal(2, multiLogger.Loggers.Count);

            var id = new Guid();
            var logLevel = LogLevel.Info;
            var message = "Test message";
            var dict = new Dictionary<string, string>();

            // Setup mocks
            mockConsoleLogger.Setup(mcl => mcl.Log(id, logLevel, message, dict)).Throws(new IOException());
            mockFileLogger.Setup(mfl => mfl.Log(id, logLevel, message, dict));

            // Exception should be caught and process continue
            multiLogger.Log(id, logLevel, message, dict);

            
            // Assert
            mockConsoleLogger.Verify(mc => mc.Log(id, logLevel, message, dict), Times.Once());
            mockFileLogger.Verify(mc => mc.Log(id, logLevel, message, dict), Times.Once());
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
