using Herd.Data.Models;
using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Herd.Data
{
    public static class Extensions
    {
        private static readonly Regex MODEL_NAME_TOKENIZER = new Regex("Herd([a-zA-Z]+)DataModel");
        private static ConcurrentDictionary<Type, string> _knownModelNames = new ConcurrentDictionary<Type, string>();

        public static string GetEntityName<T>(this T objectModel) where T : IHerdDataModel
        {
            return typeof(T).GetEntityName();
        }

        public static string GetEntityName(this Type objectModelType)
        {
            if (_knownModelNames.ContainsKey(objectModelType))
            {
                return _knownModelNames[objectModelType];
            }
            if (objectModelType.IsAssignableFrom(typeof(HerdDataModel)))
            {
                throw new ArgumentException($"{objectModelType.Name} is not a {nameof(HerdDataModel)} object");
            }
            var matches = MODEL_NAME_TOKENIZER.Matches(objectModelType.Name);
            var entityName = matches[0].Groups[1].Value;
            return _knownModelNames[objectModelType] = entityName;
        }
    }
}