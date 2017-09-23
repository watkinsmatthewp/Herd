namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {

        #region Public methods
        public void StartStream()
        {
            var client = BuildMastodonApiClient();
            var streaming = client.GetUserStreaming();

            // Register events
            //streaming.OnUpdate += HomeStream_OnUpdate;

            // Start streaming
            streaming.Start();
        }
        #endregion

        #region Private methods
        #endregion
    }
}