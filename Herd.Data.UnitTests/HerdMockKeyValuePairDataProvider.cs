using Herd.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Herd.Data.UnitTests
{
    public class HerdMockKeyValuePairDataProvider : HerdKeyValuePairDataProvider
    {
        Dictionary<string, string> _data = new Dictionary<string, string>();

        public HerdMockKeyValuePairDataProvider() : base("ROOT", ".")
        {
        }

        protected override IEnumerable<string> GetAllKeys(string rootKey) => _data.Keys.Where(k => k.StartsWith($"{rootKey}{KeyDelimiter}", StringComparison.OrdinalIgnoreCase));
        protected override string ReadKey(string key) => _data[key];
        protected override void WriteKey(string key, string value) => _data[key] = value;
    }
}
