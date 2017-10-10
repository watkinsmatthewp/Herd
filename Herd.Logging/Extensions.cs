using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Logging
{
    public static class Extensions
    {
        public static void Info(this IHerdLogger logger, Guid? id, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null, Exception e = null)
        {
            logger.Log(id, HerdLogLevel.Info, message, GetContextParameters(contextParameters, GetContextParameters(e)));
        }

        public static void Debug(this IHerdLogger logger, Guid? id, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null, Exception e = null)
        {
            logger.Log(id, HerdLogLevel.Debug, message, GetContextParameters(contextParameters, GetContextParameters(e)));
        }

        public static void Error(this IHerdLogger logger, Guid? id, string message, IEnumerable<KeyValuePair<string, string>> contextParameters = null, Exception e = null)
        {
            logger.Log(id, HerdLogLevel.Error, message, GetContextParameters(contextParameters, GetContextParameters(e)));
        }

        #region Private helpers

        private static IEnumerable<KeyValuePair<string, string>> GetContextParameters(IEnumerable<KeyValuePair<string, string>> baseContextParameters, IEnumerable<KeyValuePair<string, string>> extraContextParameters)
        {
            if (baseContextParameters != null)
            {
                foreach (var contextParameter in baseContextParameters)
                {
                    yield return contextParameter;
                }
            }
            if (extraContextParameters != null)
            {
                foreach (var contextParameter in extraContextParameters)
                {
                    yield return contextParameter;
                }
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> GetContextParameters(Exception e)
        {
            yield return Kvp("EXCEPTION", GetLowestlevelException(e));
        }

        private static Exception GetLowestlevelException(Exception e)
        {
            return (e as AggregateException)?.InnerExceptions?.Count == 1 ? GetLowestlevelException(e.InnerException) : e;
        }

        private static KeyValuePair<string, string> Kvp(string key, object value)
        {
            return new KeyValuePair<string, string>(key, value.ToString());
        }

        #endregion        
    }
}
