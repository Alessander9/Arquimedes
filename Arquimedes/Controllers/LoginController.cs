using Arquimedes.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Arquimedes.Controllers
{
    public class LoginController : Controller
    {

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // GET: Login
        

        public ActionResult Login()
        {
            // Si existe una sesión activa, redirigir al Dashboard según el rol
            if (Session["Usuario"] != null && Session["Rol"] != null)
            {
                string rolUsuario = Session["Rol"].ToString();

                // Redirigir según el rol ya autenticado
                switch (rolUsuario)
                {
                    case "Administrador":
                        return RedirectToAction("VistaAdministrador", "Administrador");
                    case "Estudiante":
                        return RedirectToAction("VistaEstudiante", "Estudiante");
                    case "Academico":
                        return RedirectToAction("Dashboard", "Academico");
                    case "Profesor":
                        return RedirectToAction("Dashboard", "Profesor");
                    case "Asistente":
                        return RedirectToAction("Dashboard", "Asistente");
                }
            }

            // Si no hay sesión activa, mostrar la vista de login
            return View();
        }




        [HttpPost]
        public ActionResult Login(string nombreUsuario, string contrasenia, string rolUsuario)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    using (SqlCommand cmd = new SqlCommand("ValidarCredenciales", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@NombreUsuario", nombreUsuario);
                        cmd.Parameters.AddWithValue("@Contrasenia", contrasenia);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string rolUsuarioDb = reader["rolUsuario"].ToString().Trim();
                                string idRelacionado = reader["idRelacionado"].ToString();

                                // Depuración
                                System.Diagnostics.Debug.WriteLine("Rol desde la base de datos: " + rolUsuarioDb);
                                System.Diagnostics.Debug.WriteLine("Rol seleccionado por el usuario: " + rolUsuario);

                                // Validar el rol
                                if (!string.Equals(rolUsuarioDb, rolUsuario, StringComparison.OrdinalIgnoreCase))
                                {
                                    ViewBag.Message = "El rol seleccionado no coincide con el rol asignado al usuario.";
                                    return View();
                                }

                                // Crear sesión
                                Session["Usuario"] = nombreUsuario;
                                Session["Rol"] = rolUsuarioDb;
                                Session["IdRelacionado"] = idRelacionado;

                                // Redirigir según el rol
                                switch (rolUsuarioDb)
                                {
                                    case "Administrador":
                                        return RedirectToAction("VistaAdministrador", "Administrador");
                                    case "Estudiante":
                                        return RedirectToAction("VistaEstudiante", "Usuario");
                                    case "Academico":
                                        return RedirectToAction("VistaAcademico", "Academico");
                                    case "Profesor":
                                        return RedirectToAction("VistaProfesor", "Profesor");
                                    case "Asistente":
                                        return RedirectToAction("VistaAsistente", "Asistente");
                                    default:
                                        throw new Exception("Rol desconocido.");
                                }
                            }
                            else
                            {
                                ViewBag.Message = "Credenciales inválidas. Por favor, inténtalo de nuevo.";
                                return View();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Error al intentar iniciar sesión: " + ex.Message;
                return View();
            }
        }


        // Redirigir a la vista correspondiente según el rol
        private ActionResult RedirectToRoleDashboard(string rolUsuario)
        {
            if (rolUsuario.Equals("Administrador", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("VistaAdministrador", "Usuario  ");
            else if (rolUsuario.Equals("Estudiante", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("VistaEstudiante", "Usuario");
            else if (rolUsuario.Equals("Academico", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("VistaAcademico", "Academico");
            else if (rolUsuario.Equals("Profesor", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("VistaProfesor", "Profesor");
            else if (rolUsuario.Equals("Asistente", StringComparison.OrdinalIgnoreCase))
                return RedirectToAction("VistaAsistente", "Asistente");
            else
                throw new Exception("Rol desconocido.");
        }

        // MÉTODO: Configurar Tiempo de Sesión
        private void ConfigurarSesion()
        {
            // Configuración del tiempo de sesión en minutos
            Session.Timeout = 20; // 20 minutos de inactividad
        }

        // MÉTODO: Manejar Sesión Expirada
        public ActionResult SesionExpirada()
        {
            // Redirigir al usuario al login con un mensaje de sesión expirada
            TempData["Mensaje"] = "Tu sesión ha expirado. Por favor, inicia sesión nuevamente.";
            return RedirectToAction("Login", "Login");
        }

        // MÉTODO: Validar Sesión Activa
        private bool EsSesionActiva()
        {
            // Verifica si la sesión sigue activa
            return Session["Usuario"] != null && Session["Rol"] != null;
        }

        // MÉTODO: Cerrar Sesión (Botón de Logout)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // Limpiar y cerrar la sesión
            Session.Clear();
            Session.Abandon();
            FormsAuthentication.SignOut(); // Opcional si usas FormsAuthentication
            return RedirectToAction("Login", "Login");
        }

        // MÉTODO: Protección de Acciones
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // Si la sesión está inactiva y no estamos en Login/Logout, redirigir
            if (!EsSesionActiva() && filterContext.ActionDescriptor.ActionName != "Login")
            {
                filterContext.Result = SesionExpirada();
            }
        }

    }

}