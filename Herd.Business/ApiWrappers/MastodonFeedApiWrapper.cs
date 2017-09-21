namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        public async Task<object> DoStuff()
        {
            var timeline = await ApiClient.GetHomeTimeline();
            throw new NotImplementedException();
        }
    }
}