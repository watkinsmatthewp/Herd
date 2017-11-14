using Herd.Business.Models.CommandResultData;
using Mastonet.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Herd.Business.ApiWrappers
{
    public class PagedList
    {
        public PageInformation PageInformation { get; set; }
    }

    public class PagedList<T> : PagedList
    {
        public IList<T> Elements { get; set; }

        public static PagedList<T> Create<I, T>(MastodonList<I> mastonetList, Func<I, T> convert) => new PagedList<T>
        {
            PageInformation = new PageInformation
            {
                EarlierPageMaxID = mastonetList.NextPageMaxId.ToString(),
                NewerPageSinceID = mastonetList.PreviousPageSinceId.ToString()
            },
            Elements = mastonetList.Select(e => convert(e)).ToArray()
        };
    }
}
