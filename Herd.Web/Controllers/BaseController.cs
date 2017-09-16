using Herd.Business;
using Herd.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Herd.Web.Controllers
{
    public class BaseController : Controller
    {
        public string RequestedURL => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        #region response messages
        public string SuccessMessage
        {
            get => TempData["SuccessMessage"] as string;
            set => TempData["SuccessMessage"] = value;
        }

        public string InfoMessage
        {
            get => TempData["InfoMessage"] as string;
            set => TempData["InfoMessage"] = value;
        }

        public string ErrorMessage
        {
            get => TempData["ErrorMessage"] as string;
            set => TempData["ErrorMessage"] = value;
        }
        #endregion

        // TODO: Get this from a cookie or User.Identity
        private static long? _activeUserID = null;
        public static long ActiveUserId => _activeUserID ?? (_activeUserID = 1).Value;

        private static HerdUserDataModel _activeUser = null;
        public static HerdUserDataModel ActiveUser => _activeUser ?? (_activeUser = HerdApp.Instance.Data.GetUser(ActiveUserId));

        // TODO: Lazy-load
        private static IMastodonApiWrapper _mastodonApiWrapper = null;
        public static IMastodonApiWrapper MastodonApiWrapper => _mastodonApiWrapper ?? (_mastodonApiWrapper = new MastodonApiWrapper(ActiveUser.MastodonInstanceHost, ActiveUser.ApiAccessToken));

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewData["RequestedURL"] = RequestedURL;
            ViewData["ActiveUserID"] = ActiveUserId;
            ViewData["ActiveUser"] = ActiveUser;
        }
    }
}
