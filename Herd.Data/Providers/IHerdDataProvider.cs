using Herd.Data.Models;

namespace Herd.Data.Providers
{
    public interface IHerdDataProvider
    {
        HerdUserDataModel GetUser(long id);
        HerdUserDataModel GetUser(string email);
        HerdUserDataModel CreateUser(HerdUserDataModel user);
        void UpdateUser(HerdUserDataModel user);

        HerdAppRegistrationDataModel GetAppRegistration(long id);
        HerdAppRegistrationDataModel GetAppRegistration(string instance);
        HerdAppRegistrationDataModel CreateAppRegistration(HerdAppRegistrationDataModel registration);
        void UpdateAppRegistration(HerdAppRegistrationDataModel registration);
    }
}