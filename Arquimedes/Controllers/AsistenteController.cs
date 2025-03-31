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
    public class AsistenteController : Controller
    {

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // GET: Asistente
        public ActionResult VistaAsistente()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RegistrarAsistente(Asistente model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();

                        using (SqlCommand cmd = new SqlCommand("RegistrarAsistente", con))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Agregar los parámetros del procedimiento almacenado
                            cmd.Parameters.AddWithValue("@IdAsistente", model.IdAsistente);
                            cmd.Parameters.AddWithValue("@NombreAsistente", model.NombreAsistente);
                            cmd.Parameters.AddWithValue("@ApellidoAsistente", model.ApellidoAsistente);
                            cmd.Parameters.AddWithValue("@TelefonoAsistente", model.TelefonoAsistente);
                            cmd.Parameters.AddWithValue("@DireccionAsistente", model.DireccionAsistente);

                            cmd.ExecuteNonQuery();
                        }
                    }

                    ViewBag.Message = "Asistente registrado correctamente.";
                    return RedirectToAction("RegistrarAsistente");
                }
                catch (SqlException ex)
                {
                    ViewBag.Message = "Error al registrar el asistente: " + ex.Message;
                }
            }
            else
            {
                ViewBag.Message = "Datos inválidos. Por favor verifica el formulario.";
            }

            return View(model);
        }

    }
}