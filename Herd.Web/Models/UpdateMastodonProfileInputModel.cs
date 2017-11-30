using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Herd.Web.Models
{
    public class UpdateMastodonProfileInputModel
    {
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public IFormFile AvatarImage { get; set; }
        public IFormFile HeaderImage { get; set; }
    }
}
