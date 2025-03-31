using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class RoleAuthorizeAttribute : ActionFilterAttribute
    {
        private readonly string[] _requiredRoles;

        public RoleAuthorizeAttribute(params string[] requiredRoles)
        {
            _requiredRoles = requiredRoles;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var session = filterContext.HttpContext.Session;
            if (session == null || session["Rol"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "controller", "Access" },
                    { "action", "Unauthorized" }
                    });
                return;
            }

            // Validar si el rol del usuario coincide con alguno de los roles requeridos
            var userRole = session["Rol"].ToString();
            if (!_requiredRoles.Any(role => role.Equals(userRole, StringComparison.OrdinalIgnoreCase)))
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "controller", "Access" },
                    { "action", "Unauthorized" }
                    });
            }
        }
    }

}