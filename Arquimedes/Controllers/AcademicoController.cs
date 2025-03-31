using Arquimedes.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class AcademicoController : Controller
    {
        // Cadena de conexión
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // Método para listar académicos
        [RoleAuthorize("Profesor", "Administrador", "Asistente")]
        public ActionResult ListarAcademicos()
        {
            List<Academico> academicos = new List<Academico>();

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarAcademicos", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                academicos.Add(new Academico
                                {
                                    CodAcademico = dr["codAcademico"].ToString(),
                                    NombreAcademico = dr["nombreAcademico"].ToString(),
                                    ApellidoAcademico = dr["apellidoAcademico"].ToString(),
                                    EdadAcademico = Convert.ToInt32(dr["edadAcademico"]),
                                    PeriodoAcademico = Convert.ToInt32(dr["periodoAcademico"]),
                                    TelefonoAcademico = dr["telefonoAcademico"].ToString(),
                                    DireccionAcademico = dr["direccionAcademico"].ToString(),
                                    EstadoAcademico = dr["estadoAcademico"].ToString(),
                                    DniAcademico = dr["dniAcademico"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al listar los académicos: " + ex.Message;
            }

            return View(academicos);
        }

        // Método para registrar un académico
        [RoleAuthorize("Profesor", "Administrador", "Asistente")]
        [HttpGet]
        public ActionResult RegistrarAcademico()
        {
            return View(new Academico());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarAcademico(Academico academico)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("RegistrarAcademico", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CodAcademico", academico.CodAcademico);
                            cmd.Parameters.AddWithValue("@NombreAcademico", academico.NombreAcademico);
                            cmd.Parameters.AddWithValue("@ApellidoAcademico", academico.ApellidoAcademico);
                            cmd.Parameters.AddWithValue("@EdadAcademico", academico.EdadAcademico);
                            cmd.Parameters.AddWithValue("@PeriodoAcademico", academico.PeriodoAcademico);
                            cmd.Parameters.AddWithValue("@TelefonoAcademico", academico.TelefonoAcademico);
                            cmd.Parameters.AddWithValue("@DireccionAcademico", academico.DireccionAcademico);
                            cmd.Parameters.AddWithValue("@EstadoAcademico", academico.EstadoAcademico);
                            cmd.Parameters.AddWithValue("@DniAcademico", academico.DniAcademico);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                    TempData["Mensaje"] = "Académico registrado correctamente.";
                    return RedirectToAction("ListarAcademicos");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al registrar el académico: " + ex.Message;
                }
            }

            return View(academico);
        }

        // GET: ActualizarAcademico
        [RoleAuthorize("Profesor", "Administrador", "Asistente")]
        [HttpGet]
        public ActionResult ActualizarAcademico(string CodAcademico)
        {
            if (string.IsNullOrEmpty(CodAcademico))
            {
                return HttpNotFound("El código del académico no fue proporcionado.");
            }

            Academico academico = BuscarAcademico(CodAcademico);

            if (academico == null)
            {
                return HttpNotFound("Académico no encontrado.");
            }

            return View(academico);
        }

        // POST: ActualizarAcademico
        [RoleAuthorize("Profesor", "Administrador", "Asistente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarAcademico(Academico academico)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Por favor, verifica los datos ingresados.";
                return View(academico);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("ActualizarAcademico", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agregar parámetros al comando
                    cmd.Parameters.AddWithValue("@CodAcademico", academico.CodAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@NombreAcademico", academico.NombreAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApellidoAcademico", academico.ApellidoAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EdadAcademico", academico.EdadAcademico);
                    cmd.Parameters.AddWithValue("@PeriodoAcademico", academico.PeriodoAcademico);
                    cmd.Parameters.AddWithValue("@TelefonoAcademico", academico.TelefonoAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DireccionAcademico", academico.DireccionAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@EstadoAcademico", academico.EstadoAcademico ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@DniAcademico", academico.DniAcademico ?? (object)DBNull.Value);

                    // Abrir conexión y ejecutar el procedimiento almacenado
                    cn.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        TempData["Mensaje"] = "Académico actualizado correctamente.";
                        return RedirectToAction("ListarAcademicos");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "No se pudo actualizar el académico. Verifica los datos ingresados.";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al actualizar el académico: " + ex.Message;
            }

            return View(academico);
        }



        // Método para eliminar un académico (GET para mostrar confirmación)
        [HttpGet]
        public ActionResult EliminarAcademico(string CodAcademico)
        {
            if (string.IsNullOrEmpty(CodAcademico))
            {
                return HttpNotFound("El código del académico no fue especificado.");
            }

            Academico academico = BuscarAcademico(CodAcademico);
            if (academico == null)
            {
                return HttpNotFound("Académico no encontrado.");
            }

            return View(academico);
        }

        // Método para eliminar un académico (POST para confirmar eliminación)
        [RoleAuthorize("Profesor", "Administrador", "Asistente")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EliminarAcademicoConfirmado(string CodAcademico)
        {
            if (string.IsNullOrEmpty(CodAcademico))
            {
                ViewBag.ErrorMessage = "El código del académico no fue especificado.";
                return View();
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("EliminarAcademico", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodAcademico", CodAcademico);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Académico eliminado correctamente.";
                return RedirectToAction("ListarAcademicos");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al eliminar el académico: " + ex.Message;
                return View("ListarAcademicos");
            }
        }


        [RoleAuthorize("Profesor", "Administrador", "Asistente")]

        // Método para mostrar detalles de un académico
        [HttpGet]
        public ActionResult DetalleAcademico(string CodAcademico)
        {
            if (string.IsNullOrEmpty(CodAcademico))
            {
                return HttpNotFound("El código del académico no fue especificado.");
            }

            Academico academico = BuscarAcademico(CodAcademico);
            if (academico == null)
            {
                return HttpNotFound("Académico no encontrado.");
            }

            return View(academico);
        }

        [RoleAuthorize("Academico", "Administrador", "Asistente")] // Solo los usuarios con el rol "Academico" pueden acceder

        public ActionResult VistaAcademico() { 
        
            return View();
        
        }

        [RoleAuthorize("Academico", "Profesor", "Administrador", "Asistente")]
        private Academico BuscarAcademico(string codAcademico)
        {
            Academico academico = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                using (SqlCommand cmd = new SqlCommand("BuscarAcademico", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodAcademico", codAcademico);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            academico = new Academico
                            {
                                CodAcademico = dr["codAcademico"].ToString(),
                                NombreAcademico = dr["nombreAcademico"].ToString(),
                                ApellidoAcademico = dr["apellidoAcademico"].ToString(),
                                EdadAcademico = Convert.ToInt32(dr["edadAcademico"]),
                                PeriodoAcademico = Convert.ToInt32(dr["periodoAcademico"]),
                                TelefonoAcademico = dr["telefonoAcademico"].ToString(),
                                DireccionAcademico = dr["direccionAcademico"].ToString(),
                                EstadoAcademico = dr["estadoAcademico"].ToString(),
                                DniAcademico = dr["dniAcademico"].ToString()
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Manejar excepciones si es necesario
                ViewBag.ErrorMessage = "Error al buscar el académico: " + ex.Message;
            }

            return academico;
        }

    }
}
