using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Data.Models
{
    public interface IHerdDataModel
    {
        long ID { get; }
    }

    public abstract class HerdDataModel : IHerdDataModel
    {
        public long ID { get; set; }
    }
}
