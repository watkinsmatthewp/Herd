namespace Herd.Business.Models.Commands
{
    public class GetStatusCommand : HerdAppCommand
    {
        public int StatusId { get; set; }
    }
}