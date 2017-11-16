using Herd.Data.Models;
using Herd.Data.Providers;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Herd.Business
{
    public class HashTagRelevanceManager : IHashTagRelevanceManager
    {
        HashTagRelevanceManagerConfiguration _config;
        IDataProvider _data;
        object _lock = new object();
        int _registrationsSinceLastFlush;
        Timer _flushTimer;
        SortedSet<HashTag> _topHashTagsList;
        public SortedSet<HashTag> TopHashTagsList => _topHashTagsList ?? (_topHashTagsList = GetTopHashTags());
        public bool Dirty => _registrationsSinceLastFlush > 0;
        public DateTime LastFlushUTC { get; private set; } = DateTime.UtcNow;

        public HashTagRelevanceManager(HashTagRelevanceManagerConfiguration config, IDataProvider data)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _data = data ?? throw new ArgumentNullException(nameof(data));
        }

        public void StartTimeIntervalFlushTimer()
        {
            if (!_config.TimeFlushInterval.HasValue)
            {
                throw new Exception("Config says we won't time");
            }
            if (_flushTimer != null)
            {
                throw new Exception("Timer already started");
            }

            _flushTimer = new Timer(_config.TimeFlushInterval.Value.TotalMilliseconds) { AutoReset = true };
            _flushTimer.Elapsed += OnFlushTimerTick;
            _flushTimer.Start();
        }

        public void RegisterHashTagUse(string sanitizedHashTag)
        {
            lock (_lock)
            {
                var found = false;
                foreach (var hashTag in TopHashTagsList)
                {
                    hashTag.Score *= _config.ExponentialDecayFactor;
                    if (hashTag.Name == sanitizedHashTag)
                    {
                        found = true;
                        hashTag.Score += _config.PointsPerPost;
                    }
                }
                if (!found)
                {
                    TopHashTagsList.Add(new HashTag
                    {
                        Name = sanitizedHashTag,
                        Score = _config.PointsPerPost
                    });
                    if (TopHashTagsList.Count > _config.HashTagsToRetain)
                    {
                        TopHashTagsList.Remove(TopHashTagsList.Min);
                    }
                }
                _registrationsSinceLastFlush++;
                if (_registrationsSinceLastFlush >= _config.DataFlushPostInterval)
                {
                    Flush();
                }
            }
        }

        #region Private helpers

        SortedSet<HashTag> GetTopHashTags() => _data.GetTopHashTagsList(1).HashTags;

        void OnFlushTimerTick(object sender, ElapsedEventArgs e)
        {
            lock (_lock)
            {
                if (Dirty && DateTime.UtcNow-LastFlushUTC > _config.TimeFlushInterval)
                {
                    Flush();
                }
            }
        }

        void Flush()
        {
            LastFlushUTC = DateTime.UtcNow;
            _registrationsSinceLastFlush = 0;
            _data.UpdateTopHashTagsList(new TopHashTagsList
            {
                HashTags = TopHashTagsList,
                ID = 1
            });
        }

        #endregion
    }
}
