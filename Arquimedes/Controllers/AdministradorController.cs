using Arquimedes.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Arquimedes.Controllers
{
    public class AdministradorController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // GET: RegistrarAdministrador
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult RegistrarAdministrador()
        {
            return View();
        }

        // POST: RegistrarAdministrador
        [RoleAuthorize("Administrador", "Asistente")]
        [HttpPost]
        public ActionResult RegistrarAdministrador(Administrador model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("RegistrarAdministrador", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Parámetros del procedimiento almacenado
                            cmd.Parameters.AddWithValue("@IdAdministrador", model.IdAdministrador);
                            cmd.Parameters.AddWithValue("@NombreAdministrador", model.NombreAdministrador);
                            cmd.Parameters.AddWithValue("@ApellidoAdministrador", model.ApellidoAdministrador);
                            cmd.Parameters.AddWithValue("@TelefonoAdministrador", model.TelefonoAdministrador);
                            cmd.Parameters.AddWithValue("@DireccionAdministrador", model.DireccionAdministrador);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Administrador registrado correctamente.";
                }
                catch (SqlException ex)
                {
                    ViewBag.Message = "Error al registrar el administrador: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Por favor, verifica los datos ingresados.";
            }

            return View(model);
        }

        [RoleAuthorize("Administrador")]
        public ActionResult VistaAdministrador()
        {

            return View();
        }

    }
}