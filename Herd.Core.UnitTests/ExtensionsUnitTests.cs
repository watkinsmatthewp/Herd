using System;
using System.Collections.Generic;
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

            var voidTask = DelayReturn();
            voidTask.Synchronously();

            // Not sure what to test after this, if voidTask is complete?
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

        [Fact]
        public void ContainsTest()
        {
            Assert.True("hello world".Contains("HELLO", StringComparison.InvariantCultureIgnoreCase));
            Assert.False("hello world".Contains("HELLO", StringComparison.InvariantCulture));
        }

        [Fact]
        public void NoneTest()
        {
            Assert.True(new List<long>().None());
            Assert.False(new List<long> { 1, 2, 3 }.None());

            Assert.True(new List<Object>().None());
            Assert.False(new List<Object> { new Object(), new Object(), new Object() }.None());
        }

        [Fact]
        public void NoneWithPredicateTest()
        {
            Assert.True(new List<long>().None(i => i < 20));
            Assert.False(new List<long> { 1, 2, 3 }.None(i => i < 20));
        }

        [Fact]
        public void ThenTest()
        {
            MockObject mockObject = new MockObject { ID = 1, Touched = false };
            Assert.True(mockObject.Then(mo => mo.Touched = true).Touched);
        }

        #region Private helpers

        public class MockObject
        {
            public int ID { get; set; }
            public Boolean Touched { get; set; }
        }

        private async Task<int> DelayReturn(int val)
        {
            await Task.Delay(500);
            return val;
        }

        private async Task DelayReturn()
        {
            await Task.Delay(5000);
            return;
        }

        #endregion Private helpers
    }
}