using Herd.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Providers
{
    public interface IHerdDataProvider
    {
        HerdUserDataModel GetUser(long id);
        HerdUserDataModel CreateUser(HerdUserDataModel user);
        void UpdateUser(HerdUserDataModel user);
    }
}
