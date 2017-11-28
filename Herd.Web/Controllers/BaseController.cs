using Herd.Business;
using Herd.Business.ApiWrappers;
using Herd.Data.Models;
using Herd.Web.Code;
using Herd.Web.CustomAttributes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;

namespace Herd.Web.Controllers
{
    public class BaseController : Controller
    {
        protected Lazy<string> _requestedURL;
        protected Lazy<UserAccount> _activeUser;
        protected Lazy<Registration> _appRegistration;
        protected Lazy<IMastodonApiWrapper> _mastodonApiWrapper;
        protected Lazy<IHerdApp> _app;

        protected string RequestedURL => _requestedURL.Value;
        protected UserAccount ActiveUser => _activeUser.Value;
        protected Registration AppRegistration => _appRegistration.Value;
        protected IHerdApp App => _app.Value;

        public BaseController()
        {
            _requestedURL = new Lazy<string>(LoadRequestedURL);
            _activeUser = new Lazy<UserAccount>(LoadActiveUserFromSession);
            _appRegistration = new Lazy<Registration>(LoadAppRegistrationFromActiveUser);
            _mastodonApiWrapper = new Lazy<IMastodonApiWrapper>(LoadMastodonApiWrapperFromAppRegistration);
            _app = new Lazy<IHerdApp>(LoadHerdApp);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Authentication check
            if (RequiresAuthentication(context) && ActiveUser == null)
            {
                // Oh no you don't!
                context.Result = NotAuthorized();
                return;
            }
        }

        protected void SetAppRegistration(Registration appRegistration)
        {
            _appRegistration = new Lazy<Registration>(appRegistration);
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

        private UserAccount LoadActiveUserFromSession()
        {
            var userID = this.GetActiveUserIdFromSession();
            return userID.HasValue ? HerdWebApp.Instance.DataProvider.GetUser(userID.Value) : null;
        }

        private Registration LoadAppRegistrationFromActiveUser()
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

        private IHerdApp LoadHerdApp() => new HerdApp(HerdWebApp.Instance.DataProvider, HerdWebApp.Instance.HashTagRelevanceManager, _mastodonApiWrapper.Value, HerdWebApp.Instance.Logger);

        #endregion Private helper methods
    }
}