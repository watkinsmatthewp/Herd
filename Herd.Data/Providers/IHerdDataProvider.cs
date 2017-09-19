using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Providers
{
    public interface IHerdDataProvider
    {
        HerdUserDataModel GetUser(long id);
        HerdUserDataModel GetUser(string username, string instance);
        HerdUserDataModel CreateUser(HerdUserDataModel user);
        void UpdateUser(HerdUserDataModel user);

        HerdAppRegistrationDataModel GetAppRegistration(long id);
        HerdAppRegistrationDataModel GetAppRegistration(string instance);
        HerdAppRegistrationDataModel CreateAppRegistration(HerdAppRegistrationDataModel registration);
        void UpdateAppRegistration(HerdAppRegistrationDataModel registration);
    }
}
