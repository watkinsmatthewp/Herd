﻿using Herd.Core;
using System;
using System.Collections;
using System.Linq;

namespace Herd.Business.Extensions
{
    public static class Extensions
    {
        public static string Hashed(this string passwordPlainText, int saltKey)
        {
            var random = new Random(saltKey);
            var mungedPassword = new string($"{passwordPlainText}{saltKey}".OrderBy(c => random.Next()).ToArray());
            return mungedPassword.Hashed();
        }
    }
}