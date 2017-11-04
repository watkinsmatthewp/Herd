using Herd.Business.Models.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Web.Models
{
    public class NewMastodonPostInputModel
    {
        public string Message { get; set; }
        public MastodonPostVisibility Visibility { get; set; }
        public string ReplyStatusId { get; set; }
        public bool Sensitive { get; set; }
        public string SpoilerText { get; set; }
    }
}
