using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business
{
    public interface IHashTagRelevanceManager
    {
        SortedSet<HashTag> TopHashTagsList { get; }
        bool Dirty { get; }
        DateTime LastFlushUTC { get; }

        void StartTimeIntervalFlushTimer();
        void RegisterHashTagUse(string sanitizedHashTag);
    }
}
