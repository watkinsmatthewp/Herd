using Herd.Data.Models;
using Xunit;

namespace Herd.Data.UnitTests
{
    public class ExtensionsTests
    {
        [Fact]
        public void GetEntityNameTest()
        {
            var userAccount = new UserAccount();
            Assert.Equal("UserAccount", userAccount.GetEntityName());
        }
    }
}
