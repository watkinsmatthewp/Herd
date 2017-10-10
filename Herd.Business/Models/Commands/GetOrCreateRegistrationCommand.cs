namespace Herd.Business.Models.Commands
{
    public class GetOrCreateRegistrationCommand : HerdAppCommand
    {
        public string Instance { get; set; }
    }
}