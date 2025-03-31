using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Arquimedes.Models;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace Arquimedes.Controllers
{

    public class UsuarioController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;


        // GET: Usuario


        // GET: RegistrarUsuario


        public ActionResult RegistrarUsuario()
        {
            return View();
        }


        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult RegistrarUsuario(Usuario model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("RegistrarUsuario", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@IdUsuario", model.IdUsuario);
                            cmd.Parameters.AddWithValue("@NombreUsuario", model.NombreUsuario);
                            cmd.Parameters.AddWithValue("@Contrasenia", model.Contrasenia);
                            cmd.Parameters.AddWithValue("@RolUsuario", model.RolUsuario);
                            cmd.Parameters.AddWithValue("@IdRelacionado", model.IdRelacionado);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Usuario registrado correctamente.";
                }
                catch (SqlException ex)
                {
                    ViewBag.Message = "Error al registrar el usuario: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Datos inválidos. Por favor verifica el formulario.";
            }

            return View(model);
        }
        [RoleAuthorize("Administrador", "Asistente", "Estudiante")]

        public ActionResult VistaEstudiante()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }


    }
}