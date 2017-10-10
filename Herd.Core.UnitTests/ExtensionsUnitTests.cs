using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Herd.Core.UnitTests
{
    public class ExtensionsUnitTests
    {
        public class TestObject
        {
            public string Item1 { get; set; }
            public int Item2 { get; set; }
        }

        [Fact]
        public void ParseJsonTest()
        {
            var jsonToParse = "{ \"Item1\": \"Hello world\", \"Item2\": 100 }";
            var expectedParsedObject = new TestObject { Item1 = "Hello world", Item2 = 100 };
            var parsedObject = jsonToParse.ParseJson<TestObject>();

            Assert.NotNull(parsedObject);
            Assert.Equal(expectedParsedObject.Item1, parsedObject.Item1);
            Assert.Equal(expectedParsedObject.Item2, parsedObject.Item2);
        }

        [Theory]
        [InlineData(false, "{\"Item1\":\"Hello world\",\"Item2\":100}")]
        [InlineData(true, "{\r\n  \"Item1\": \"Hello world\",\r\n  \"Item2\": 100\r\n}")]
        public void SerializeAsJsonTest(bool indent, string expectedJson)
        {
            var objectToSerialize = new TestObject { Item1 = "Hello world", Item2 = 100 };
            var json = objectToSerialize.SerializeAsJson(indent);

            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void SynchronouslyTest()
        {
            var task = DelayReturn(1);
            var result = task.Synchronously();
            Assert.Equal(1, result);
        }

        [Fact]
        public void HashedStringTest()
        {
            Assert.Equal("ZOyIygCyaOW6GjVnihtTFtIS9PNmskdyMlNKiuyjfzw=", "Hello world".Hashed());
        }

        [Fact]
        public void HashedBytesTest()
        {
            var outputBytes = Encoding.UTF8.GetBytes("Hello world").Hashed();
            Assert.Equal("ZOyIygCyaOW6GjVnihtTFtIS9PNmskdyMlNKiuyjfzw=", Convert.ToBase64String(outputBytes));
        }

        #region Private helpers

        private async Task<int> DelayReturn(int val)
        {
            await Task.Delay(500);
            return val;
        }

        #endregion Private helpers
    }
}