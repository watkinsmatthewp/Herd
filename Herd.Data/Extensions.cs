using Herd.Data.Models;
using System;
using System.Collections.Concurrent;

namespace Herd.Data
{
    public static class Extensions
    {
        private static ConcurrentDictionary<Type, string> _knownModelNames = new ConcurrentDictionary<Type, string>();

        public static string GetEntityName<T>(this T objectModel) where T : IDataModel
        {
            return typeof(T).GetEntityName();
        }

        public static string GetEntityName(this Type objectModelType)
        {
            if (_knownModelNames.ContainsKey(objectModelType))
            {
                return _knownModelNames[objectModelType];
            }
            if (objectModelType.IsAssignableFrom(typeof(DataModel)))
            {
                throw new ArgumentException($"{objectModelType.Name} is not a {nameof(DataModel)} object");
            }
            return _knownModelNames[objectModelType] = objectModelType.Name;
        }
    }
}