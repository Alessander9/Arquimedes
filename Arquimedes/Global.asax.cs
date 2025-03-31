using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Arquimedes
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
          
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            // Verificar si la sesión está activa
            if (HttpContext.Current.Session != null && HttpContext.Current.Session["Usuario"] == null)
            {
                // Obtener el path actual de la solicitud
                string currentPath = HttpContext.Current.Request.Url.AbsolutePath.ToLower();

                // Lista de rutas permitidas sin autenticación
                var allowedPaths = new[]
                {
            "/login/login",         // Página de inicio de sesión
            "/login/logout",        // Cerrar sesión
            "/access/unauthorized", // Página de acceso no autorizado
            "/home/index",          // Permitir libre acceso al Index
            "/"                     // Permitir el acceso inicial a la raíz del sitio
        };

                // Verificar si la ruta actual no está en las permitidas
                if (!allowedPaths.Any(path => currentPath.Equals(path) || currentPath.StartsWith(path)))
                {
                    // Redirigir al login
                    HttpContext.Current.Response.Redirect("~/Login/Login");
                }
            }
        }

    }



}

