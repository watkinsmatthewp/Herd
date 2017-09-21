using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Herd.Core
{
    public static class Extensions
    {
        public static T ParseJson<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static string SerializeAsJson<T>(this T objectToSerialize, bool indented = false)
        {
            return JsonConvert.SerializeObject(objectToSerialize, indented ? Formatting.Indented : Formatting.None);
        }

        public static T Synchronously<T>(this Task<T> task)
        {
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            return task.Result;
        }

        public static string Hashed(this string originalValue)
        {
            return Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(originalValue).Hashed());
        }

        public static byte[] Hashed(this byte[] originalValue)
        {
            return SHA256.Create().ComputeHash(originalValue);
        }
    }
}
