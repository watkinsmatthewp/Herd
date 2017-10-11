using Herd.Data.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Herd.Web.Code
{
    public static class ControllerExtensions
    {
        public static long? GetActiveUserIdFromSession(this Controller controller)
        {
            var claim = controller.User?.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                return null;
            }
            return long.TryParse(claim.Value, out var userID) ? userID : null as long?;
        }

        public static Task SetActiveUserInSession(this Controller controller, UserAccount user)
        {
            return controller.SetActiveUserInSession(user.ID);
        }

        public static async Task SetActiveUserInSession(this Controller controller, long userID)
        {
            var identity = new ClaimsIdentity(Startup.COOKIE_AUTH_SCHEME_NAME, ClaimTypes.Name, ClaimTypes.Role);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userID.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, userID.ToString()));
            var principal = new ClaimsPrincipal(identity);
            await controller.HttpContext.SignInAsync(Startup.COOKIE_AUTH_SCHEME_NAME, principal, new AuthenticationProperties
            {
                IsPersistent = true
            });
        }

        public static Task ClearActiveUserFromSession(this Controller controller)
        {
            return controller.HttpContext.SignOutAsync(Startup.COOKIE_AUTH_SCHEME_NAME);
        }
    }
}