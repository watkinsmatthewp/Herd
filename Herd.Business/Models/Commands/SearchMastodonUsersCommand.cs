namespace Herd.Business.Models.Commands
{
    public class SearchMastodonUsersCommand : Command
    {
        public int MaxCount { get; set; } = 30;
        public long? UserID { get; set; }
        public string Name { get; set; }
        public long? FollowsUserID { get; set; }
        public long? FollowedByUserID { get; set; }

        public bool IncludeFollowers { get; set; }
        public bool IncludeFollowing { get; set; }
        public bool IncludeFollowsActiveUser { get; set; }
        public bool IncludeFollowedByActiveUser { get; set; }

        public bool IsGlobalSearch => !UserID.HasValue
            && string.IsNullOrWhiteSpace(Name)
            && !FollowsUserID.HasValue
            && !FollowedByUserID.HasValue;
    }
}