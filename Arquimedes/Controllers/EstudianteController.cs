using Arquimedes.Controllers;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;
using MigraDoc.Rendering;
using PdfSharp.Pdf;
using System.IO;

namespace Arquimedes.Controllers
{
    public class EstudianteController : Controller
    {
        readonly SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString);
        SqlCommand cmd;

        // GET: ListarEstudiantes
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult ListarEstudiantes()
        {
            List<Estudiante> estudiantes = new List<Estudiante>();

            try
            {
                cn.Open();
                cmd = new SqlCommand("ListarEstudiantes", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    Estudiante estudiante = new Estudiante
                    {
                        CodEstudiante = dr["codEstudiante"].ToString(),
                        NombreEstudiante = dr["nombreEstudiante"].ToString(),
                        ApellidoEstudiante = dr["apellidoEstudiante"].ToString(),
                        EdadEstudiante = Convert.ToInt32(dr["edadEstudiante"]),
                        AnioEstudiante = Convert.ToInt32(dr["anioEstudiante"]),
                        SeccionEstudiante = dr["seccionEstudiante"].ToString(),
                        GradoAcademico = dr["gradoAcademico"].ToString(),
                        Telefono = dr["telefono"].ToString(),
                        Direccion = dr["direccion"].ToString(),
                        EstadoEstudiante = dr["estadoEstudiante"].ToString(),
                        DniEstudiante = dr["dniEstudiante"].ToString()
                    };
                    estudiantes.Add(estudiante);
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                ViewBag.errorMessage = ex.Message;
            }
            finally
            {
                cn.Close();
            }

            return View(estudiantes);
        }

       

        // GET: DetalleEstudiante
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult DetalleEstudiante(string codEstudiante)
        {
            if (string.IsNullOrEmpty(codEstudiante))
            {
                return HttpNotFound("El código del estudiante no fue especificado.");
            }

            Estudiante estudiante = BuscarEstudiante(codEstudiante);

            if (estudiante == null)
            {
                return HttpNotFound("Estudiante no encontrado.");
            }

            return View(estudiante);
        }


        // GET: RegistrarEstudiante
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult RegistrarEstudiante()
        {
            // Inicializar los datos de los DropDownList
            RecargarViewData();

            // Retornar la vista con un modelo vacío
            return View(new Estudiante());
        }

        [RoleAuthorize("Administrador", "Asistente")]

        // POST: RegistrarEstudiante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEstudiante(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
                    {
                        cn.Open();

                        // Validar duplicados
                        using (SqlCommand checkCmd = new SqlCommand("SELECT COUNT(*) FROM tb_Estudiante WHERE codEstudiante = @CodEstudiante", cn))
                        {
                            checkCmd.Parameters.AddWithValue("@CodEstudiante", estudiante.CodEstudiante);
                            int count = (int)checkCmd.ExecuteScalar();
                            if (count > 0)
                            {
                                ModelState.AddModelError("", "El código del estudiante ya existe.");
                                RecargarViewData();
                                return View(estudiante);
                            }
                        }

                        // Registrar estudiante
                        using (SqlCommand cmd = new SqlCommand("RegistrarEstudiante", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CodEstudiante", estudiante.CodEstudiante);
                            cmd.Parameters.AddWithValue("@Nombre", estudiante.NombreEstudiante);
                            cmd.Parameters.AddWithValue("@Apellido", estudiante.ApellidoEstudiante);
                            cmd.Parameters.AddWithValue("@Edad", estudiante.EdadEstudiante);
                            cmd.Parameters.AddWithValue("@AnioEstudiante", estudiante.AnioEstudiante);
                            cmd.Parameters.AddWithValue("@Seccion", estudiante.SeccionEstudiante);
                            cmd.Parameters.AddWithValue("@GradoAcademico", estudiante.GradoAcademico);
                            cmd.Parameters.AddWithValue("@Telefono", estudiante.Telefono);
                            cmd.Parameters.AddWithValue("@Direccion", estudiante.Direccion);
                            cmd.Parameters.AddWithValue("@EstadoEstudiante", estudiante.EstadoEstudiante);
                            cmd.Parameters.AddWithValue("@DniEstudiante", estudiante.DniEstudiante);

                            int result = cmd.ExecuteNonQuery();
                            ViewBag.Mensaje = result > 0 ? "Estudiante registrado correctamente." : "No se pudo registrar el estudiante.";
                        }
                    }

                    return RedirectToAction("ListarEstudiantes");
                }
                catch (SqlException ex)
                {
                    ViewBag.Mensaje = "Error SQL al registrar el estudiante: " + ex.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Mensaje = "Error general: " + ex.Message;
                }
            }

            RecargarViewData();
            return View(estudiante);
        }

        // Método para recargar los datos de los DropDownList
        private void RecargarViewData()
        {
            // Grado Académico
            ViewBag.GradoAcademico = new SelectList(new[] { "Inicial", "Primaria", "Secundaria", "Academico" });

            // Sección del Estudiante
            ViewBag.SeccionEstudiante = new SelectList(new[] { "A", "B", "C", "D" });

            // Estado del Estudiante
            ViewBag.EstadoEstudiante = new SelectList(new[] { "Activo", "Inactivo" });

            // Año del Estudiante
            ViewBag.AnioEstudiante = new SelectList(new[]
            {
            "1° Grado",
            "2° Grado",
            "3° Grado",
            "4° Grado",
            "5° Grado",
            "6° Grado"
        });
        }




        // Método auxiliar para buscar un estudiante
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        private Estudiante BuscarEstudiante(string codEstudiante)
        {
            Estudiante estudiante = null;

            try
            {
                cn.Open();
                cmd = new SqlCommand("BuscarEstudiante", cn)
                {
                    CommandType = CommandType.StoredProcedure
                };
                cmd.Parameters.AddWithValue("@CodEstudiante", codEstudiante);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    estudiante = new Estudiante
                    {
                        CodEstudiante = dr["codEstudiante"].ToString(),
                        NombreEstudiante = dr["nombreEstudiante"].ToString(),
                        ApellidoEstudiante = dr["apellidoEstudiante"].ToString(),
                        EdadEstudiante = Convert.ToInt32(dr["edadEstudiante"]),
                        AnioEstudiante = Convert.ToInt32(dr["anioEstudiante"]),
                        SeccionEstudiante = dr["seccionEstudiante"].ToString(),
                        GradoAcademico = dr["gradoAcademico"].ToString(),
                        Telefono = dr["telefono"].ToString(),
                        Direccion = dr["direccion"].ToString(),
                        EstadoEstudiante = dr["estadoEstudiante"].ToString(),
                        DniEstudiante = dr["dniEstudiante"].ToString()
                    };
                }
                dr.Close();
            }
            catch (SqlException ex)
            {
                Console.WriteLine("Error al buscar estudiante: " + ex.Message);
            }
            finally
            {
                cn.Close();
            }

            return estudiante;
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult EliminarEstudiante(string codEstudiante)
        {
            if (string.IsNullOrEmpty(codEstudiante))
            {
                return HttpNotFound("El código del estudiante no fue especificado.");
            }

            // Buscar el estudiante por su código
            Estudiante estudiante = BuscarEstudiante(codEstudiante);
            if (estudiante == null)
            {
                return HttpNotFound("No se encontró al estudiante especificado.");
            }

            // Mostrar la vista de confirmación
            return View(estudiante);
        }

        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente")]

        [ValidateAntiForgeryToken]
        public ActionResult EliminarEstudiantes(string codEstudiante)
        {
            if (string.IsNullOrEmpty(codEstudiante))
            {
                TempData["MensajeError"] = "El código del estudiante no fue especificado.";
                return RedirectToAction("ListarEstudiantes");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
                {
                    cn.Open();
                    using (SqlCommand cmd = new SqlCommand("EliminarEstudiante", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodEstudiante", codEstudiante);

                        int result = cmd.ExecuteNonQuery();
                        if (result > 0)
                        {
                            TempData["MensajeExito"] = "Estudiante eliminado correctamente.";
                        }
                        else
                        {
                            TempData["MensajeError"] = "No se encontró al estudiante especificado.";
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                TempData["MensajeError"] = "Ocurrió un error al intentar eliminar al estudiante: " + ex.Message;
            }

            return RedirectToAction("ListarEstudiantes");
        }


        [RoleAuthorize("Administrador", "Asistente", "Estudiante")]

        public ActionResult VistaEstudiante()
        {
            return View();
        
        }


        // GET: ActualizarEstudiante
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ActualizarEstudiante(string codEstudiante)
        {
            if (string.IsNullOrEmpty(codEstudiante))
            {
                return HttpNotFound("El código del estudiante no fue especificado.");
            }

            Estudiante estudiante = BuscarEstudiante(codEstudiante);

            if (estudiante == null)
            {
                return HttpNotFound("Estudiante no encontrado.");
            }

            RecargarViewData(); // Cargar datos para los DropDownList
            return View(estudiante);
        }

        // POST: ActualizarEstudiante
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente")]

        [ValidateAntiForgeryToken]

        public ActionResult ActualizarEstudiante(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString))
                    {
                        cn.Open();
                        using (SqlCommand cmd = new SqlCommand("ActualizarEstudiante", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@CodEstudiante", estudiante.CodEstudiante);
                            cmd.Parameters.AddWithValue("@Nombre", estudiante.NombreEstudiante);
                            cmd.Parameters.AddWithValue("@Apellido", estudiante.ApellidoEstudiante);
                            cmd.Parameters.AddWithValue("@Edad", estudiante.EdadEstudiante);
                            cmd.Parameters.AddWithValue("@AnioEstudiante", estudiante.AnioEstudiante);
                            cmd.Parameters.AddWithValue("@Seccion", estudiante.SeccionEstudiante);
                            cmd.Parameters.AddWithValue("@GradoAcademico", estudiante.GradoAcademico);
                            cmd.Parameters.AddWithValue("@Telefono", estudiante.Telefono);
                            cmd.Parameters.AddWithValue("@Direccion", estudiante.Direccion);
                            cmd.Parameters.AddWithValue("@EstadoEstudiante", estudiante.EstadoEstudiante);
                            cmd.Parameters.AddWithValue("@DniEstudiante", estudiante.DniEstudiante);

                            int result = cmd.ExecuteNonQuery();

                            if (result > 0)
                            {
                                ViewBag.Mensaje = "Estudiante actualizado correctamente.";
                            }
                            else
                            {
                                ViewBag.Mensaje = "No se pudo actualizar el estudiante.";
                            }
                        }
                    }

                    return RedirectToAction("ListarEstudiantes");
                }
                catch (SqlException ex)
                {
                    ViewBag.Mensaje = "Error SQL al actualizar el estudiante: " + ex.Message;
                }
                catch (Exception ex)
                {
                    ViewBag.Mensaje = "Error general: " + ex.Message;
                }
            }

            RecargarViewData(); // Cargar nuevamente los datos para los DropDownList
            return View(estudiante);
        }


      


    }
}
