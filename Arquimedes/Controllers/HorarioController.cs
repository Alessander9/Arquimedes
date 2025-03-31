using Arquimedes.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class HorarioController : Controller
    {
        private string ConnectionString => ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;


        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        // MÉTODO: Listar Horarios
        public ActionResult ListarHorarios()
        {
            List<Horario> horarios = new List<Horario>();

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarHorarios", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                horarios.Add(new Horario
                                {
                                    IdHorario = dr["idHorario"].ToString(),
                                    TipoInscripcion = dr["tipoInscripcion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"] as string,
                                    CodAcademico = dr["codAcademico"] as string,
                                    IdCurso = dr["idCurso"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al listar los horarios: " + ex.Message;
            }

            return View(horarios);
        }

        // MÉTODO: GET para Registrar Horario.
        [RoleAuthorize("Administrador", "Asistente")]
        [HttpGet]
        public ActionResult RegistrarHorario()
        {
            try
            {
                // Cargar datos necesarios para dropdowns
                ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
                ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
                ViewBag.CodCursoOptions = ObtenerCodCursos();
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar datos para el formulario: " + ex.Message;
            }

            return View(new Horario());
        }


        // MÉTODO: POST para Registrar Horario
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente")]

        [ValidateAntiForgeryToken]
        public ActionResult RegistrarHorario(Horario horario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("RegistrarHorario", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Agregar parámetros del procedimiento almacenado
                            cmd.Parameters.AddWithValue("@IdHorario", horario.IdHorario);
                            cmd.Parameters.AddWithValue("@TipoInscripcion", horario.TipoInscripcion);
                            cmd.Parameters.AddWithValue("@CodEstudiante", (object)horario.CodEstudiante ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CodAcademico", (object)horario.CodAcademico ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@IdCurso", horario.IdCurso);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Mensaje"] = "Horario registrado correctamente.";
                    return RedirectToAction("ListarHorarios");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al registrar el horario: " + ex.Message;
                }
            }

            // Recargar opciones para dropdowns en caso de error
            ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
            ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
            ViewBag.CodCursoOptions = ObtenerCodCursos();

            return View(horario);
        }

        // MÉTODOS ADICIONALES

        private SelectList ObtenerCodEstudiantes()
        {
            var estudiantes = new List<SelectListItem>();
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT codEstudiante, CONCAT(nombreEstudiante, ' ', apellidoEstudiante) AS NombreCompleto FROM tb_Estudiante";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                estudiantes.Add(new SelectListItem
                                {
                                    Value = dr["codEstudiante"].ToString(),
                                    Text = dr["NombreCompleto"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar estudiantes: " + ex.Message;
            }
            return new SelectList(estudiantes, "Value", "Text");
        }

        private SelectList ObtenerCodAcademicos()
        {
            var academicos = new List<SelectListItem>();
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT codAcademico, CONCAT(nombreAcademico, ' ', apellidoAcademico) AS NombreCompleto FROM tb_Academico";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                academicos.Add(new SelectListItem
                                {
                                    Value = dr["codAcademico"].ToString(),
                                    Text = dr["NombreCompleto"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar académicos: " + ex.Message;
            }
            return new SelectList(academicos, "Value", "Text");
        }

        private SelectList ObtenerCodCursos()
        {
            var cursos = new List<SelectListItem>();
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    string query = "SELECT idCurso, nombreCurso FROM tb_Cursos";
                    using (SqlCommand cmd = new SqlCommand(query, cn))
                    {
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cursos.Add(new SelectListItem
                                {
                                    Value = dr["idCurso"].ToString(),
                                    Text = dr["nombreCurso"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar cursos: " + ex.Message;
            }
            return new SelectList(cursos, "Value", "Text");
        }


        // GET: DetalleHorario
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult DetalleHorario(string idHorario)
        {
            if (string.IsNullOrWhiteSpace(idHorario))
            {
                return HttpNotFound("El ID del horario no fue especificado.");
            }

            Horario detalleHorario = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarHorario", cn)) // Asegúrate de que este procedimiento almacenado exista
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdHorario", idHorario);

                        cn.Open();

                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                detalleHorario = new Horario
                                {
                                    IdHorario = dr["idHorario"].ToString(),
                                    TipoInscripcion = dr["tipoInscripcion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"] != DBNull.Value ? dr["codEstudiante"].ToString() : null,
                                    CodAcademico = dr["codAcademico"] != DBNull.Value ? dr["codAcademico"].ToString() : null,
                                    IdCurso = dr["idCurso"].ToString()
                                };
                            }
                        }
                    }
                }

                if (detalleHorario == null)
                {
                    return HttpNotFound("Horario no encontrado.");
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al obtener el detalle del horario: " + ex.Message;
                return RedirectToAction("ListarHorarios");
            }

            return View(detalleHorario);
        }

        //ACTUALIZAR HORARIO
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ActualizarHorario(string idHorario)
        {
            if (string.IsNullOrWhiteSpace(idHorario))
            {
                return HttpNotFound("El ID del horario no fue especificado.");
            }

            Horario horario = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarHorario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdHorario", idHorario);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                horario = new Horario
                                {
                                    IdHorario = dr["idHorario"].ToString(),
                                    TipoInscripcion = dr["tipoInscripcion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"] != DBNull.Value ? dr["codEstudiante"].ToString() : null,
                                    CodAcademico = dr["codAcademico"] != DBNull.Value ? dr["codAcademico"].ToString() : null,
                                    IdCurso = dr["idCurso"].ToString()
                                };
                            }
                        }
                    }
                }

                if (horario == null)
                {
                    return HttpNotFound("Horario no encontrado.");
                }

                // Cargar opciones para dropdowns
                ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
                ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
                ViewBag.CodCursoOptions = ObtenerCodCursos();
                ViewBag.TipoInscripcionOptions = new SelectList(new[] {
            new { Value = "Estudiante", Text = "Estudiante" },
            new { Value = "Academico", Text = "Académico" }
        }, "Value", "Text", horario.TipoInscripcion);

                return View(horario);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar los datos del horario: " + ex.Message;
                return RedirectToAction("ListarHorarios");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ActualizarHorario(Horario horario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(ConnectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("ActualizarHorario", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            cmd.Parameters.AddWithValue("@IdHorario", horario.IdHorario);
                            cmd.Parameters.AddWithValue("@TipoInscripcion", horario.TipoInscripcion);
                            cmd.Parameters.AddWithValue("@CodEstudiante", (object)horario.CodEstudiante ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@CodAcademico", (object)horario.CodAcademico ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@IdCurso", horario.IdCurso);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Mensaje"] = "Horario actualizado correctamente.";
                    return RedirectToAction("ListarHorarios");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al actualizar el horario: " + ex.Message;
                }
            }

            // Recargar opciones para dropdowns si ocurre un error
            ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
            ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
            ViewBag.CodCursoOptions = ObtenerCodCursos();
            ViewBag.TipoInscripcionOptions = new SelectList(new[] {
        new { Value = "Estudiante", Text = "Estudiante" },
        new { Value = "Academico", Text = "Académico" }
    }, "Value", "Text", horario.TipoInscripcion);

            return View(horario);
        }


        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult EliminarHorario(string idHorario)
        {
            if (string.IsNullOrWhiteSpace(idHorario))
            {
                return HttpNotFound("El ID del horario no fue especificado.");
            }

            Horario horario = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarHorario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdHorario", idHorario);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                horario = new Horario
                                {
                                    IdHorario = dr["idHorario"].ToString(),
                                    TipoInscripcion = dr["tipoInscripcion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"]?.ToString(),
                                    CodAcademico = dr["codAcademico"]?.ToString(),
                                    IdCurso = dr["idCurso"].ToString()
                                };
                            }
                        }
                    }
                }

                if (horario == null)
                {
                    return HttpNotFound("Horario no encontrado.");
                }

                return View(horario);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar los datos del horario: " + ex.Message;
                return RedirectToAction("ListarHorarios");
            }
        }

        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente")]

        [ValidateAntiForgeryToken]
        public ActionResult EliminarHorario(Horario horario)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("EliminarHorario", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdHorario", horario.IdHorario);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Horario eliminado correctamente.";
                return RedirectToAction("ListarHorarios");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar el horario: " + ex.Message;
                return RedirectToAction("ListarHorarios");
            }
        }

            
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Estudiante", "Academico", "Profesor")]
        public ActionResult VerHorario(string codEstudiante, string codAcademico)
        {
            if (string.IsNullOrWhiteSpace(codEstudiante) && string.IsNullOrWhiteSpace(codAcademico))
            {
                ViewBag.ErrorMessage = "Debe proporcionar un código de estudiante o académico.";
                return View("Error");
            }

            List<HorarioDetalle> horarios = new List<HorarioDetalle>();

            try
            {
                using (SqlConnection cn = new SqlConnection(ConnectionString))
                using (SqlCommand cmd = new SqlCommand("BuscarHorarioPorCodigo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Asignar el parámetro según el código proporcionado
                    if (!string.IsNullOrWhiteSpace(codEstudiante))
                    {
                        cmd.Parameters.AddWithValue("@CodEstudiante", codEstudiante);
                        cmd.Parameters.AddWithValue("@CodAcademico", DBNull.Value);
                        ViewBag.Titulo = "Horario del Estudiante";
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@CodEstudiante", DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", codAcademico);
                        ViewBag.Titulo = "Horario del Académico";
                    }

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            horarios.Add(new HorarioDetalle
                            {
                                CodigoHorario = dr["CodigoHorario"]?.ToString(),
                                NombreCurso = dr["NombreCurso"]?.ToString(),
                                Profesor = dr["Profesor"]?.ToString(),
                                Nivel = dr["NivelEducativo"]?.ToString(),
                                Dia = dr["Dia"]?.ToString(),
                                Hora = dr["Hora"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los horarios: " + ex.Message;
                return View("Error");
            }

            if (horarios.Count == 0)
            {
                ViewBag.ErrorMessage = "No se encontraron horarios para el código proporcionado.";
                return View("Error");
            }

            return View(horarios); // Devuelve la lista de horarios a la vista
        }


        // MÉTODO: Página para Buscar Horarios
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult BuscarHorario()
        {
            return View();
        }



        // Método Auxiliar para Ejecutar Procedimientos Almacenados
        private List<HorarioDetalle> ObtenerHorarios(string storedProcedure, string parameterName, string parameterValue)
        {
            var horarios = new List<HorarioDetalle>();

            try
            {
                using (var cn = new SqlConnection(ConnectionString))
                using (var cmd = new SqlCommand(storedProcedure, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(parameterName, parameterValue);

                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            horarios.Add(new HorarioDetalle
                            {
                                CodigoHorario = dr["idHorario"]?.ToString(),
                                NombreCurso = dr["nombreCurso"]?.ToString(),
                                Dia = dr["dia"]?.ToString(),
                                Hora = dr["hora"]?.ToString(),
                                Nivel = dr["nivel"]?.ToString(),
                                Profesor = dr["nombreProfesor"]?.ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los horarios: " + ex.Message;
            }

            return horarios;
        }
    }
}
    



