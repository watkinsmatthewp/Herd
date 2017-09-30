using Mastonet;
using Mastonet.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        public async Task<IList<Status>> GetRecentStatuses(int limit = 30) => (await BuildMastodonApiClient().GetHomeTimeline(null, null, 30)).ToArray();
        public Task<Status> CreateNewPost(string text) => BuildMastodonApiClient().PostStatus(text, Visibility.Private);
    }
}