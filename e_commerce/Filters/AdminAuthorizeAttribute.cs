using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace e_commerce.Filters
{
    public class AdminAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Vérifier si l'utilisateur est authentifié
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false; // L'utilisateur n'est pas authentifié
            }
            // Obtenir l'email de l'utilisateur authentifié
            var userEmail = httpContext.User.Identity.Name;
            // Vérifier si l'utilisateur est l'admin (par exemple, par son email)
            if (userEmail == "kellyalphador@gmail.com")
            {
                return true; // L'utilisateur est l'administrateur
            }
            return false; // L'utilisateur n'est pas l'administrateur
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {// Rediriger vers la page de connexion si l'utilisateur n'est pas authentifié
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { controller = "Account", action = "Login" })
                );
            }else{// Rediriger vers une page d'accès non autorisé si l'utilisateur est authentifié mais pas admin
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary(new { controller = "Home", action = "Index" })
                );
            }
        }
    }
}