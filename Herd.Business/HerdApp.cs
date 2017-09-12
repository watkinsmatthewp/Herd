using Herd.Data.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Business
{
    public class HerdApp
    {
        public static HerdApp Instance { get; } = new HerdApp();

        public IHerdDataProvider Data { get; } = new HerdFileDataProvider();

        private HerdApp()
        {

        }
    }
}
