namespace Herd.Business.Models.Commands
{
    public class SearchMastodonUsersCommand : Command
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string FollowsUserID { get; set; }
        public string FollowedByUserID { get; set; }

        public bool IncludeFollowers { get; set; }
        public bool IncludeFollowing { get; set; }
        public bool IncludeFollowsActiveUser { get; set; }
        public bool IncludeFollowedByActiveUser { get; set; }

        public PagingOptions PagingOptions { get; set; }

        public bool IsGlobalSearch => string.IsNullOrWhiteSpace(UserID)
            && string.IsNullOrWhiteSpace(Name)
            && string.IsNullOrWhiteSpace(FollowsUserID)
            && string.IsNullOrWhiteSpace(FollowedByUserID);
    }
}