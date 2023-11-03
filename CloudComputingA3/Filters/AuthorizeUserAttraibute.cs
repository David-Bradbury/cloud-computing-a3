using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;


namespace CloudComputingA3.Filters
{
    public class AuthorizeUserAttribute : Attribute, IAuthorizationFilter
    {
        // Ensure users are correctly logged in and if not redirects to home page.
        public void OnAuthorization(AuthorizationFilterContext context)
        {

            var userID = context.HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(userID))
            {
                context.Result = new RedirectToActionResult("Index", "Home", null);
            }
        }
    }
}
