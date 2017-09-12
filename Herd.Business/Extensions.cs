using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business
{
    public static class Extensions
    {
        public static bool IsAuthenticated(this IMastodonApiWrapper apiWrapper) => !string.IsNullOrWhiteSpace(apiWrapper.UserApiToken);
    }
}
