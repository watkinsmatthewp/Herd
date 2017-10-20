using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Herd.Logging.UnitTests
{
    public class DefaultLogFormatterTests
    {
        [Fact]
        public void GetLogLineTestNoParams()
        {
            DefaultLogFormatter dlf = new DefaultLogFormatter();
            Guid id = new Guid();
            string message = "info-message";

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  [ID] = {id}");
            sb.AppendLine($"  [LEVEL] = {LogLevel.Info.ToString().ToUpperInvariant()}");
            sb.AppendLine($"  [TIME] = {DateTime.UtcNow}");
            sb.AppendLine($"  [MESSAGE] = {message}");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();

            string expected = sb.ToString();
            string actual = dlf.GetLogLine(id, LogLevel.Info, "info-message", null);
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GetLogLineTestWithParams()
        {
            DefaultLogFormatter dlf = new DefaultLogFormatter();

            Guid id = new Guid();
            string message = "info-message";
            var contextParameters = new Dictionary<string, string>
            {
                { "isDev", "true" },
                { "isProduction", "false" }
            };

            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  [ID] = {id}");
            sb.AppendLine($"  [LEVEL] = {LogLevel.Info.ToString().ToUpperInvariant()}");
            sb.AppendLine($"  [TIME] = {DateTime.UtcNow}");
            sb.AppendLine($"  [MESSAGE] = {message}");
            foreach (var contextParameter in contextParameters)
            {
                sb.AppendLine($"  [{contextParameter.Key}] = {contextParameter.Value}");
            }
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();

            string expected = sb.ToString();
            string actual = dlf.GetLogLine(id, LogLevel.Info, "info-message", contextParameters);
            Assert.Equal(expected, actual);
        }

}
}
