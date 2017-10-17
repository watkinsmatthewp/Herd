using Herd.Data.Models;

namespace Herd.Data.Providers
{
    public interface IDataProvider
    {
        // App registration
        Registration GetAppRegistration(int id);

        Registration GetAppRegistration(string instance);

        Registration CreateAppRegistration(Registration appRegistration);

        void UpdateAppRegistration(Registration appRegistration);

        // Users
        UserAccount GetUser(int id);

        UserAccount GetUser(string email);

        UserAccount CreateUser(UserAccount user);

        void UpdateUser(UserAccount user);
    }
}