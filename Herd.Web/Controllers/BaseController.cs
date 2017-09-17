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
        private long? _activeUserID => GetLoginToken();
        public long ActiveUserId => (long) _activeUserID;

        private HerdUserDataModel _activeUser = null;
        // public static long ActiveUserId => _activeUserID ?? (_activeUserID = 1).Value; 
        // public HerdUserDataModel ActiveUser => _activeUser ?? (_activeUser = HerdApp.Instance.Data.GetUser(ActiveUserId));
        public HerdUserDataModel ActiveUser => HerdApp.Instance.Data.GetUser(ActiveUserId);
        

        // TODO: Lazy-load
        private IMastodonApiWrapper _mastodonApiWrapper = null;
        public IMastodonApiWrapper MastodonApiWrapper => _mastodonApiWrapper ?? (_mastodonApiWrapper = new MastodonApiWrapper(ActiveUser.MastodonInstanceHost, ActiveUser.ApiAccessToken));

        /**
         * TODO:  Use this in order to tell what user is logged on?
         * WIP
         */
        public long GetLoginToken()
        {
            Request.Headers.TryGetValue("LoginToken", out Microsoft.Extensions.Primitives.StringValues headerValue);
            long id = Convert.ToInt32(headerValue.FirstOrDefault());
            if (id <= 0)
            {
                // user has to login
                return 1;
            }

            return id;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            ViewData["RequestedURL"] = RequestedURL;
            ViewData["ActiveUserID"] = ActiveUserId;
            ViewData["ActiveUser"] = ActiveUser;
        }
    }
}