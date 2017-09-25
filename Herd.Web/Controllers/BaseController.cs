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
using Herd.Data.Providers;

namespace Herd.Web.Controllers
{
    public class BaseController : Controller
    {
        protected const string USER_COOKIE_NAME = "HERD_USER_ID";

        protected Lazy<string> _requestedURL;
        protected Lazy<HerdUserAccountDataModel> _activeUser;
        protected Lazy<HerdAppRegistrationDataModel> _appRegistration;
        protected Lazy<IMastodonApiWrapper> _mastodonApiWrapper;
        protected Lazy<IHerdApp> _app;

        protected string RequestedURL => _requestedURL.Value;
        protected HerdUserAccountDataModel ActiveUser => _activeUser.Value;
        protected HerdAppRegistrationDataModel AppRegistration => _appRegistration.Value;
        protected IHerdApp App => _app.Value;

        public BaseController()
        {
            _requestedURL = new Lazy<string>(LoadRequestedURL);
            _activeUser = new Lazy<HerdUserAccountDataModel>(LoadActiveUserFromCookie);
            _appRegistration = new Lazy<HerdAppRegistrationDataModel>(LoadAppRegistrationFromActiveUser);
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(LoadMastodonApiWrapperFromAppRegistration);
            _app = new Lazy<IHerdApp>(LoadHerdApp);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Authentication check
            if (RequiresAuthentication(context) && User == null)
            {
                // Oh no you don't!
                context.Result = NotAuthorized();
                return;
            }

            // Parrot back the auth cookie
            SetActiveUserCookie(ActiveUser);

            // Set the ViewData collection to use in views
            ViewData["RequestedURL"] = RequestedURL;
            ViewData["ActiveUser"] = ActiveUser;
        }

        protected void SetAppRegistration(HerdAppRegistrationDataModel appRegistration)
        {
            _appRegistration = new Lazy<HerdAppRegistrationDataModel>(appRegistration);
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

        #region Private helper methods

        private bool RequiresAuthentication(ActionExecutingContext context)
        {
            var controllerActionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (controllerActionDescriptor == null)
            {
                return true;
            }
            return !controllerActionDescriptor.MethodInfo.GetCustomAttributes(typeof(AuthenticationNotRequiredAttribute), true).Any();
        }

        private string LoadRequestedURL() => $"{Request.Scheme}://{Request.Host}{Request.Path}{Request.QueryString}";

        private HerdUserAccountDataModel LoadActiveUserFromCookie()
        {
            var userCookieComponents = Request.Cookies[USER_COOKIE_NAME]?.Split('|');
            if (userCookieComponents?.Length == 2 && !string.IsNullOrWhiteSpace(userCookieComponents[1]) && long.TryParse(userCookieComponents[0], out long userID))
            {
                var userByID = HerdWebApp.Instance.DataProvider.GetUser(userID);
                if (userByID != null && userID.ToString().Hashed(userByID.Security.SaltKey) == userCookieComponents[1])
                {
                    return userByID;
                }
            }
            return null;
        }

        private HerdAppRegistrationDataModel LoadAppRegistrationFromActiveUser()
        {
            if (ActiveUser?.MastodonConnection?.AppRegistrationID > 0)
            {
                return HerdWebApp.Instance.DataProvider.GetAppRegistration(ActiveUser.MastodonConnection.AppRegistrationID);
            }
            return null;
        }

        private IMastodonApiWrapper LoadMastodonApiWrapperFromAppRegistration()
        {
            if (_appRegistration.Value == null)
            {
                return new MastodonApiWrapper();
            }
            if (string.IsNullOrWhiteSpace(ActiveUser.MastodonConnection?.ApiAccessToken))
            {
                return new MastodonApiWrapper(_appRegistration.Value);
            }
            return new MastodonApiWrapper(_appRegistration.Value, ActiveUser.MastodonConnection);
        }

        private IHerdApp LoadHerdApp() => new HerdApp(HerdWebApp.Instance.DataProvider, _mastodonApiWrapper.Value);

        #endregion
    }
}