using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business
{
    public class HashTagRelevanceManagerConfiguration
    {
        /// <summary>
        /// How many points to add to each hash tag's relevance score when it is mentioned in a post
        /// Default: 100
        /// </summary>
        public int PointsPerPost { get; set; } = 100;

        /// <summary>
        /// How many hashtagged posts to allow between writes the to the data storage provider
        /// Default: 50
        /// </summary>
        public int DataFlushPostInterval { get; set; } = 50;

        /// <summary>
        /// Max time to allow between writes to the data storage provider
        /// Default 10 minutes
        /// </summary>
        public TimeSpan? TimeFlushInterval { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Number of hashtags to retain in memory and in the data storage provider
        /// Default: 30
        /// </summary>
        public int HashTagsToRetain { get; set; } = 30;

        /// <summary>
        /// How much to decay the relevance score on each update.
        /// Higher = less decay. Lower = more decay.
        /// Default: .75m
        /// </summary>
        public decimal ExponentialDecayFactor { get; set; } = .75m;
    }
}
