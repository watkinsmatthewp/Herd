namespace Herd.Business.Models
{
    public class PagingOptions
    {
        public string MaxID { get; set; }
        public string SinceID { get; set; }
        public int? Limit { get; set; } = 30;
    }
}