using System;
using System.Collections.Generic;
using System.Text;

namespace Herd.Business.Models.Commands
{
    public static class CommandExtensions
    {
        public static bool HasPagingOptions(this IPagedCommand command) => command.PagingOptions.HasPagingOptions();

        public static bool HasPagingOptions(this PagingOptions pagingOptions) =>
            (pagingOptions?.Limit).HasValue
            || !string.IsNullOrWhiteSpace(pagingOptions?.MaxID)
            || !string.IsNullOrWhiteSpace(pagingOptions?.SinceID);
    }
}
