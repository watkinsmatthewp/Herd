using Herd.Data.Providers;

namespace Herd.Business
{
    interface IHerdApp
    {
        IHerdDataProvider Data { get; }
    }
}