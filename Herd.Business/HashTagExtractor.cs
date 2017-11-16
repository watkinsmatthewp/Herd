using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Herd.Business
{
    public class HashTagExtractor
    {
        private static readonly Regex HASHTAG_REGEX = new Regex(@"\B#((\w|\d)+)");

        public IEnumerable<string> ExtractHashTags(string input)
        {
            if (!string.IsNullOrWhiteSpace(input))
            {
                foreach (var hashTag in ExtractHashTagsFromText(input))
                {
                    yield return hashTag;
                }
            }
        }

        #region Private helpers

        IEnumerable<string> ExtractHashTagsFromText(string input)
        {
            return HASHTAG_REGEX.Matches(input)
                .Select(m => Sanitize(m.Value))
                .Where(h => !string.IsNullOrWhiteSpace(h))
                .Distinct();
        }

        string Sanitize(string hashTag)
        {
            if (string.IsNullOrWhiteSpace(hashTag))
            {
                return null;
            }
            return new string(hashTag.Where(c => char.IsLetterOrDigit(c)).Select(c => char.ToLowerInvariant(c)).ToArray());
        }

        #endregion
    }
}
