using Mastonet;
using Mastonet.Entities;
using System.Collections.ObjectModel;

namespace Herd.Business
{
    public partial class MastodonApiWrapper : IMastodonApiWrapper
    {
        private ObservableCollection<Status> home;
        #region Public methods
        public async void StartHomeStream()
        {
            var client = BuildMastodonApiClient();
            var home = await client.GetHomeTimeline();
            Home = new ObservableCollection<Status>(home);
            var streaming = client.GetUserStreaming();

            // Register events
            //streaming.OnUpdate += HomeStream_OnUpdate;

            // Start streaming
            streaming.Start();
        }

        public ObservableCollection<Status> Home
        {
            get { return home; }
            set
            {
                if (home != value)
                {
                    home = value;
                    //OnPropertyChanged();
                }
            }
        }

        #endregion

        #region Private methods        
        private void HomeStream_OnUpdate(object sender, StreamUpdateEventArgs e)
        {
            Home.Insert(0, e.Status);
        }
    #endregion
}
}