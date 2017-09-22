using Herd.Data.Models;

namespace Herd.Data.Providers
{
    public interface IHerdDataProvider
    {
        // App registration
        HerdAppRegistrationDataModel GetAppRegistration(long id);
        HerdAppRegistrationDataModel GetAppRegistration(string instance);
        HerdAppRegistrationDataModel CreateAppRegistration(HerdAppRegistrationDataModel appRegistration);
        void UpdateAppRegistration(HerdAppRegistrationDataModel appRegistration);

        // Users
        HerdUserAccountDataModel GetUser(long id);
        HerdUserAccountDataModel GetUser(string email);
        HerdUserAccountDataModel CreateUser(HerdUserAccountDataModel user);
        void UpdateUser(HerdUserAccountDataModel user);

        // Profiles
        HerdUserProfileDataModel GetProfile(long id);
        HerdUserProfileDataModel CreateProfile(HerdUserProfileDataModel profile);
        void UpdateProfile(HerdUserProfileDataModel profile);
    }
}