using Herd.Business.Models.MastodonWrappers;

namespace Herd.Business.Models.CommandResultData
{
    public class HerdAppGetStatusCommandResultData : HerdAppCommandResultData
    {
        public Status Status { get; set; }
        public StatusContext StatusContext { get; set; }
    }
}