using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.MastodonWrappers
{
    public class Account
    {
        public string AccountName { get; set; }
        public string AvatarUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public string HeaderUrl{ get; set; }
        public int Id  { get; set; }
        public bool Locked { get; set; }
        public string Note { get; set; }
        public string ProfileUrl { get; set; }
        public int StatusesCount { get; set; }
        public string UserName { get; set; }
    }
}
