using Herd.Data.Models;
using Xunit;

namespace Herd.Data.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void GetEntityNameTest()
        {
            Assert.Equal("UserAccount", new UserAccount().GetEntityName());
            Assert.Equal("Registration", new Registration().GetEntityName());
        }
    }
}