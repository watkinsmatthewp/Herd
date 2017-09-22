using Herd.Business;
using Herd.Data.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Herd.Core;
using Microsoft.AspNetCore.Mvc.Controllers;
using Herd.Web.CustomAttributes;
using Herd.Business.Models.Commands;

namespace Herd.Web.Controllers
{
    public class BaseController : Controller
    {
        protected const string USER_COOKIE_NAME = "HERD_USER_ID";

        protected Lazy<string> _requestedURL;
        protected Lazy<HerdUserAccountDataModel> _activeUser;
        protected Lazy<bool> _isAuthenticated;
        protected Lazy<IMastodonApiWrapper> _mastodonApiWrapper = null;

        protected string RequestedURL => _requestedURL.Value;
        protected HerdUserAccountDataModel ActiveUser => _activeUser.Value;
        protected bool IsAuthenticated => _isAuthenticated.Value;
        protected IMastodonApiWrapper MastodonApiWrapper => _mastodonApiWrapper.Value;

        protected virtual bool RequiresAuthentication(ActionExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                return true;
            }
            return !controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AuthenticationNotRequiredAttribute), true).Any();
        }

        public BaseController()
        {
            _requestedURL = new Lazy<string>(LoadRequestedURL);
            _activeUser = new Lazy<HerdUserAccountDataModel>(LoadActiveUserFromCookie);
            _isAuthenticated = new Lazy<bool>(LoadIsAuthenticated);
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(LoadMastodonApiWrapper);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Authentication check
            if (IsAuthenticated)
            {
                // Parrot back the auth cookie
                SetActiveUserCookie(ActiveUser);
            }
            else if (RequiresAuthentication(context))
            {
                // Oh no you don't!
                context.Result = NotAuthorized();
                return;
            }

            // Set the ViewData collection to use in views
            ViewData["RequestedURL"] = RequestedURL;
            ViewData["ActiveUser"] = ActiveUser;
        }

        protected void SetActiveUserCookie(HerdUserAccountDataModel user)
        {
            if (user == null)
            {
                ClearActiveUserCookie();
            }
            else
            {
                Response.Cookies.Append(USER_COOKIE_NAME, $"{user.ID}|{user.ID.ToString().Hashed(user.Security.SaltKey)}");
            }
        }

        protected void ClearActiveUserCookie()
        {
            Response.Cookies.Delete(USER_COOKIE_NAME);
        }

        protected virtual IActionResult NotAuthorized()
        {
            Response.StatusCode = 401;
            return RedirectToAction("Index", "Home");
        }

        protected JsonResult ApiJson(object o, bool serializeNulls = false, bool indent = false)
        {
            return Json(o, new Newtonsoft.Json.JsonSerializerSettings
            {
                Formatting = indent ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None,
                NullValueHandling = serializeNulls ? Newtonsoft.Json.NullValueHandling.Include : Newtonsoft.Json.NullValueHandling.Ignore
            });
        }

        private bool LoadIsAuthenticated() => ActiveUser != null;
        private bool LoadRequireAuthentication() => (Request.HttpContext.Items["AllowAnonymous"] as bool?) != true;
        private string LoadRequestedURL() => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        private HerdUserAccountDataModel LoadActiveUserFromCookie()
        {
            var userCookieComponents = Request.Cookies[USER_COOKIE_NAME]?.Split('|');
            if (userCookieComponents?.Length == 2 && !string.IsNullOrWhiteSpace(userCookieComponents[1]) && long.TryParse(userCookieComponents[0], out long userID))
            {
                var userByID = HerdApp.Instance.GetUser(new HerdAppGetUserCommand { UserID = userID }).Data?.User;
                if (userByID != null && userID.ToString().Hashed(userByID.Security.SaltKey) == userCookieComponents[1])
                {
                    return userByID;
                }
            }
            return null;
        }

        private IMastodonApiWrapper LoadMastodonApiWrapper()
        {
            // See if there is an app registration for this instance in the DB
            var appRegistration = HerdApp.Instance.Data.GetAppRegistration(ActiveUser.MastodonConnection.Instance);
            if (appRegistration == null)
            {
                // Nope, we have to register
                appRegistration = HerdApp.Instance.Data.CreateAppRegistration(new MastodonApiWrapper(ActiveUser.MastodonConnection.Instance).RegisterApp().Synchronously());
            }
            return new MastodonApiWrapper(appRegistration, ActiveUser.MastodonConnection.ApiAccessToken);
        }
    }
}