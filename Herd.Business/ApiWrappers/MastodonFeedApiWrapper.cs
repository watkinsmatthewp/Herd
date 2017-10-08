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
        public Task<Status> CreateNewPost(string message, Visibility visibility, int? replyStatusId = null, IEnumerable<int> mediaIds = null, bool sensitive = false, string spoilerText = null) => BuildMastodonApiClient().PostStatus(message, visibility, replyStatusId, mediaIds, sensitive, spoilerText);
    }
}