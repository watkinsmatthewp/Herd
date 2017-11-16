using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace Herd.Business.UnitTests
{
    public class HashTagExtractorTests
    {
        [Fact]
        public void TestExtractor()
        {
            var input = "You should see #One as 'one' and #TWO but listed as #two and #this-butnothis not that#there.";
            var hashTags = new HashTagExtractor().ExtractHashTags(input).ToArray();
            Assert.Equal(3, hashTags.Length);
            Assert.Equal("one", hashTags[0]);
            Assert.Equal("two", hashTags[1]);
            Assert.Equal("this", hashTags[2]);
        }
    }
}
