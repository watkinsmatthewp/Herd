using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Logging
{
    public class HerdDefaultLogFormatter : IHerdLogFormatter
    {
        public string GetLogLine(Guid? id, HerdLogLevel logLevel, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null)
        {
            var sb = new StringBuilder();
            sb.AppendLine("{");
            sb.AppendLine($"  [ID] = {id}");
            sb.AppendLine($"  [LEVEL] = {logLevel.ToString().ToUpperInvariant()}");
            sb.AppendLine($"  [TIME] = {DateTime.UtcNow}");
            sb.AppendLine($"  [MESSAGE] = {message}");

            if (contextParameters != null)
            {
                foreach (var contextParameter in contextParameters)
                {
                    sb.AppendLine($"  [{contextParameter.Key}] = {contextParameter.Value}");
                }
            }

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("--------------------------------------------------------------------------------");
            sb.AppendLine();

            return sb.ToString();
        }
    }
}