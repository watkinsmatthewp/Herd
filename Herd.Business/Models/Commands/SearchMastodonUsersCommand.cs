namespace Herd.Business.Models.Commands
{
    public class SearchMastodonUsersCommand : Command
    {
        public int MaxCount { get; set; } = 30;
        public int? UserID { get; set; }
        public string Name { get; set; }
        public int? FollowsUserID { get; set; }
        public int? FollowedByUserID { get; set; }

        public bool IncludeFollowers { get; set; }
        public bool IncludeFollowing { get; set; }

        public bool IsGlobalSearch => !UserID.HasValue
            && string.IsNullOrWhiteSpace(Name)
            && !FollowsUserID.HasValue
            && !FollowedByUserID.HasValue;
    }
}