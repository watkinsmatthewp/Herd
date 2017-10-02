using Herd.Data.Models;
using Herd.Data.Providers;
using System;
using Xunit;

namespace Herd.Data.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestAppRegistration()
        {
            HerdFileDataProvider provider = new HerdFileDataProvider();
            HerdAppRegistrationDataModel registration = new HerdAppRegistrationDataModel();
            registration.MastodonAppRegistrationID = 1;
            registration.ClientId = "05";
            registration.ClientSecret = "sneaky";
            registration.Instance = "social";

            provider.CreateAppRegistration(registration);

            HerdAppRegistrationDataModel result = provider.GetAppRegistration(registration.ID);

            Assert.Equal(registration.ClientId, result.ClientId);
        }
    }
}
