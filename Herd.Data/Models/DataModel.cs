namespace Herd.Data.Models
{
    public interface IDataModel
    {
        long ID { get; }
    }

    public abstract class DataModel : IDataModel
    {
        public long ID { get; set; }
    }
}