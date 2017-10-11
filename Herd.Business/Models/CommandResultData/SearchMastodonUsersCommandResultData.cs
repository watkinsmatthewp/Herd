using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.CommandResultData
{
    public class SearchMastodonUsersCommandResultData : CommandResultData
    {
        public List<UserSearchResult> Users { get; set; }

        public class UserSearchResult
        {
            public int MastodonUserID { get; set; }
            public string MastodonUserName { get; set; }
            public string MastodonDisplayName { get; set; }
            public string MastodonProfileImageURL { get; set; }
            public string MastodonHeaderImageURL { get; set; }
            public string MastodonShortBio { get; set; }
            public bool FollowsCurrentUser { get; set; }
            public bool CurrentUserIsFollowing { get; set; }
        }
    }
}
