using System.Collections.Generic;

namespace Herd.Business.Models.Entities
{
    public class MastodonUser
    {
        // Core properties
        public int MastodonUserID { get; set; }

        public string MastodonUserName { get; set; }
        public string MastodonDisplayName { get; set; }
        public string MastodonProfileImageURL { get; set; }
        public string MastodonHeaderImageURL { get; set; }
        public string MastodonShortBio { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int PostCount { get; set; }

        // Extra "context" properties
        public bool? FollowsRelevantUser { get; set; }

        public bool? IsFollowedByRelevantUser { get; set; }
        public List<MastodonUser> Followers { get; set; }
        public List<MastodonUser> Following { get; set; }
    }
}