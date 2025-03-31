using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Arquimedes.Controllers

{
    public class AccessController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        [HttpGet]
        public ActionResult Unauthorized()
        {
            return View("Unauthorized");
        }

        // Método estático para validar el acceso a roles
        public static bool ValidarAcceso(HttpSessionStateBase session, string rolRequerido)
        {
            // Validar si la sesión contiene el rol del usuario
            if (session["Rol"] != null)
            {
                string rolUsuario = session["Rol"].ToString();
                return string.Equals(rolUsuario, rolRequerido, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}