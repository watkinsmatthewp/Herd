﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

// Add a comment (this should trigger a build)
// Another comment, another build

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

        public static void Synchronously(this Task task)
        {
            if (!task.IsCompleted)
            {
                task.Wait();
            }
        }

        public static T Synchronously<T>(this Task<T> task)
        {
            return task.ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public static string Hashed(this string originalValue)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(originalValue).Hashed());
        }

        public static byte[] Hashed(this byte[] originalValue)
        {
            return SHA256.Create().ComputeHash(originalValue);
        }

        public static bool Contains(this string containingText, string searchString, StringComparison stringComparison)
        {
            return containingText.IndexOf(searchString, stringComparison) >= 0;
        }

        public static T With<T>(this T obj, Action<T> doWork) => obj.Then(doWork);

        public static T Then<T>(this T obj, Action<T> doWork)
        {
            doWork(obj);
            return obj;
        }

        public static bool None<T>(this IEnumerable<T> collection) => !collection.Any();

        public static bool None<T>(this IEnumerable<T> collection, Func<T, bool> predicate) => !collection.Any(predicate);
    }
}