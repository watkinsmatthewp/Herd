namespace Herd.Business.Models.Entities
{
    public class MastodonRelationship
    {
        public string ID { get; set; }
        public bool Following { get; set; }
        public bool FollowedBy { get; set; }
        public bool Blocking { get; set; }
        public bool Muting { get; set; }
        public bool Requested { get; set; }
        public bool DomainBlocking { get; set; }
    }
}