using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Arquimedes.Models;

namespace Arquimedes.Controllers
{
    public class EvaluacionesController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // MÉTODO: Registrar Evaluación (GET)
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente","Profesor")]

        public ActionResult RegistrarEvaluacion()
        {
            CargarOpciones();
            return View(new Evaluacion());
        }

        // MÉTODO: Registrar Evaluación (POST)
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEvaluacion(Evaluacion evaluacion)
        {
            if (string.IsNullOrEmpty(evaluacion.CodEstudiante) && string.IsNullOrEmpty(evaluacion.CodAcademico))
            {
                ModelState.AddModelError("", "Debe seleccionar un estudiante o un académico.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("RegistrarEvaluacion", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Parámetros para el procedimiento almacenado
                            cmd.Parameters.AddWithValue("@IdEvaluacion", evaluacion.IdEvaluacion);
                            cmd.Parameters.AddWithValue("@CodEstudiante",
                                string.IsNullOrEmpty(evaluacion.CodEstudiante) ? (object)DBNull.Value : evaluacion.CodEstudiante);
                            cmd.Parameters.AddWithValue("@CodAcademico",
                                string.IsNullOrEmpty(evaluacion.CodAcademico) ? (object)DBNull.Value : evaluacion.CodAcademico);
                            cmd.Parameters.AddWithValue("@IdCurso", evaluacion.IdCurso);
                            cmd.Parameters.AddWithValue("@Bimestre", evaluacion.Bimestre);
                            cmd.Parameters.AddWithValue("@T1",
                                string.IsNullOrEmpty(evaluacion.T1) ? (object)DBNull.Value : evaluacion.T1);
                            cmd.Parameters.AddWithValue("@T2",
                                string.IsNullOrEmpty(evaluacion.T2) ? (object)DBNull.Value : evaluacion.T2);
                            cmd.Parameters.AddWithValue("@T3",
                                string.IsNullOrEmpty(evaluacion.T3) ? (object)DBNull.Value : evaluacion.T3);
                            cmd.Parameters.AddWithValue("@T4",
                                string.IsNullOrEmpty(evaluacion.T4) ? (object)DBNull.Value : evaluacion.T4);
                            cmd.Parameters.AddWithValue("@T5",
                                string.IsNullOrEmpty(evaluacion.T5) ? (object)DBNull.Value : evaluacion.T5);
                            cmd.Parameters.AddWithValue("@PromedioLetra",
                                string.IsNullOrEmpty(evaluacion.PromedioLetra) ? (object)DBNull.Value : evaluacion.PromedioLetra);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Mensaje"] = "Evaluación registrada y acumulada correctamente.";
                    return RedirectToAction("ListarEvaluaciones");
                }
                catch (Exception ex)
                {
                    // Manejar errores
                    ViewBag.ErrorMessage = "Error al registrar la evaluación: " + ex.Message;
                }
            }

            // Recargar opciones si ocurre un error
            CargarOpciones();
            return View(evaluacion);
        }


        // MÉTODO PRIVADO: Cargar Opciones
        private void CargarOpciones()
        {
            ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
            ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
            ViewBag.CodCursoOptions = ObtenerCodCursos();
            ViewBag.BimestreOptions = ObtenerBimestres();
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult ListarEvaluaciones()
        {
            List<Evaluacion> evaluacionesEstudiantes = new List<Evaluacion>();
            List<Evaluacion> evaluacionesAcademicos = new List<Evaluacion>();

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarEvaluaciones", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                // Crear una nueva instancia de Evaluacion a partir de los datos del lector
                                Evaluacion evaluacion = new Evaluacion
                                {
                                    IdEvaluacion = dr["idEvaluacion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"]?.ToString(),
                                    NombreEstudiante = dr["NombreEstudiante"]?.ToString(),
                                    CodAcademico = dr["codAcademico"]?.ToString(),
                                    NombreAcademico = dr["NombreAcademico"]?.ToString(),
                                    IdCurso = dr["idCurso"].ToString(),
                                    GradoAcademico = dr["gradoAcademico"]?.ToString(),
                                    AnioEstudiante = dr["anioEstudiante"] != DBNull.Value ? Convert.ToInt32(dr["anioEstudiante"]) : (int?)null,
                                    SeccionEstudiante = dr["seccionEstudiante"]?.ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                };

                                // Separar las evaluaciones en listas según el tipo
                                if (!string.IsNullOrEmpty(evaluacion.CodEstudiante))
                                {
                                    evaluacionesEstudiantes.Add(evaluacion);
                                }
                                else if (!string.IsNullOrEmpty(evaluacion.CodAcademico))
                                {
                                    evaluacionesAcademicos.Add(evaluacion);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al listar las evaluaciones: " + ex.Message;
            }

            // Pasar ambas listas al ViewBag
            ViewBag.EvaluacionesEstudiantes = evaluacionesEstudiantes;
            ViewBag.EvaluacionesAcademicos = evaluacionesAcademicos;

            return View();
        }


        // MÉTODO: Detalle de Evaluación
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult DetalleEvaluacion(string IdEvaluacion)
        {
            Evaluacion evaluacion = BuscarEvaluacion(IdEvaluacion);
            if (evaluacion == null)
            {
                return HttpNotFound("Evaluación no encontrada.");
            }
            return View(evaluacion);
        }

        // MÉTODO: Buscar Evaluación
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        private Evaluacion BuscarEvaluacion(string idEvaluacion)
        {
            Evaluacion evaluacion = null;
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarEvaluacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdEvaluacion", idEvaluacion);
                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                evaluacion = new Evaluacion
                                {
                                    IdEvaluacion = dr["idEvaluacion"].ToString(),
                                    CodEstudiante = dr["codEstudiante"]?.ToString(),
                                    NombreEstudiante = dr["nombreEstudiante"]?.ToString(),
                                    AnioEstudiante = dr["anioEstudiante"] != DBNull.Value ? Convert.ToInt32(dr["anioEstudiante"]) : 0,
                                    SeccionEstudiante = dr["seccionEstudiante"]?.ToString(),
                                    GradoAcademico = dr["gradoAcademico"]?.ToString(),
                                    CodAcademico = dr["codAcademico"]?.ToString(),
                                    NombreAcademico = dr["nombreAcademico"]?.ToString(),
                                    IdCurso = dr["idCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log de error o manejo apropiado
                ViewBag.ErrorMessage = "Error al buscar la evaluación: " + ex.Message;
            }
            return evaluacion;
        }


        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult ActualizarEvaluacion(string idEvaluacion)
        {
            if (string.IsNullOrWhiteSpace(idEvaluacion))
            {
                return HttpNotFound("El ID de la evaluación no fue especificado.");
            }

            try
            {
                Evaluacion evaluacion = BuscarEvaluacion(idEvaluacion);

                if (evaluacion == null)
                {
                    return HttpNotFound("Evaluación no encontrada.");
                }

                // Cargar datos adicionales para los dropdowns
                ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
                ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
                ViewBag.CodCursoOptions = ObtenerCodCursos();
                ViewBag.BimestreOptions = ObtenerBimestres();

                return View(evaluacion);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la evaluación: " + ex.Message;
                return RedirectToAction("ListarEvaluaciones");
            }
        }

        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEvaluacion(Evaluacion evaluacion)
        {
            if (evaluacion == null)
            {
                TempData["ErrorMessage"] = "Datos de evaluación no válidos.";
                return RedirectToAction("ListarEvaluaciones");
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ActualizarEvaluacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros enviados al procedimiento almacenado
                        cmd.Parameters.AddWithValue("@IdEvaluacion", evaluacion.IdEvaluacion);
                        cmd.Parameters.AddWithValue("@CodEstudiante", string.IsNullOrEmpty(evaluacion.CodEstudiante) ? (object)DBNull.Value : evaluacion.CodEstudiante);
                        cmd.Parameters.AddWithValue("@CodAcademico", string.IsNullOrEmpty(evaluacion.CodAcademico) ? (object)DBNull.Value : evaluacion.CodAcademico);
                        cmd.Parameters.AddWithValue("@IdCurso", evaluacion.IdCurso);
                        cmd.Parameters.AddWithValue("@Bimestre", evaluacion.Bimestre);
                        cmd.Parameters.AddWithValue("@T1", string.IsNullOrEmpty(evaluacion.T1) ? (object)DBNull.Value : evaluacion.T1);
                        cmd.Parameters.AddWithValue("@T2", string.IsNullOrEmpty(evaluacion.T2) ? (object)DBNull.Value : evaluacion.T2);
                        cmd.Parameters.AddWithValue("@T3", string.IsNullOrEmpty(evaluacion.T3) ? (object)DBNull.Value : evaluacion.T3);
                        cmd.Parameters.AddWithValue("@T4", string.IsNullOrEmpty(evaluacion.T4) ? (object)DBNull.Value : evaluacion.T4);
                        cmd.Parameters.AddWithValue("@T5", string.IsNullOrEmpty(evaluacion.T5) ? (object)DBNull.Value : evaluacion.T5);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Evaluación actualizada correctamente.";
                return RedirectToAction("ListarEvaluaciones");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar la evaluación: " + ex.Message;

                // Recargar opciones para dropdowns en caso de error
                ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
                ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
                ViewBag.CodCursoOptions = ObtenerCodCursos();
                ViewBag.BimestreOptions = ObtenerBimestres();

                return View(evaluacion);
            }
        }



        // MÉTODO PRIVADO: Convertir nota en letra a valor numérico
        private int ConvertNotaToValue(string nota, ref int divisor)
        {
            if (string.IsNullOrEmpty(nota))
                return 0;

            divisor++;

            if (nota == "AD") return 20;
            if (nota == "A") return 16;
            if (nota == "B") return 12;
            if (nota == "C") return 8;

            return 0; // Caso por defecto
        }

        // MÉTODO PRIVADO: Determinar promedio en letra
        private string DeterminePromedioLetra(double promedioNum, int divisor)
        {
            if (divisor == 0)
                return "Sin notas registradas"; // Valor por defecto si no hay notas válidas

            if (promedioNum >= 18) return "AD";
            if (promedioNum >= 14) return "A";
            if (promedioNum >= 10) return "B";

            return "C"; // Menor a 10.00
        }





        // MÉTODO: GET Eliminar Evaluación
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult EliminarEvaluacion(string IdEvaluacion)
        {
            if (string.IsNullOrWhiteSpace(IdEvaluacion))
            {
                return HttpNotFound("El ID de la evaluación no fue especificado.");
            }

            try
            {
                Evaluacion evaluacion = null;

                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarEvaluacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdEvaluacion", IdEvaluacion);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                evaluacion = new Evaluacion
                                {
                                    IdEvaluacion = dr["IdEvaluacion"].ToString(),
                                    CodEstudiante = dr["CodEstudiante"] != DBNull.Value ? dr["CodEstudiante"].ToString() : null,
                                    IdCurso = dr["IdCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["Bimestre"]),
                                    T1 = dr["T1"] != DBNull.Value ? dr["T1"].ToString() : null,
                                    T2 = dr["T2"] != DBNull.Value ? dr["T2"].ToString() : null,
                                    T3 = dr["T3"] != DBNull.Value ? dr["T3"].ToString() : null,
                                    T4 = dr["T4"] != DBNull.Value ? dr["T4"].ToString() : null,
                                    T5 = dr["T5"] != DBNull.Value ? dr["T5"].ToString() : null
                                };
                            }
                        }
                    }
                }

                if (evaluacion == null)
                {
                    return HttpNotFound("Evaluación no encontrada.");
                }

                return View(evaluacion); // Pasar el modelo a la vista
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al cargar la evaluación para eliminar: " + ex.Message;
                return RedirectToAction("ListarEvaluaciones");
            }
        }




        // MÉTODO: Eliminar Evaluación
        [HttpPost]
        public ActionResult EliminarEvaluaciones(string IdEvaluacion)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("EliminarEvaluacion", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdEvaluacion", IdEvaluacion);
                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["Mensaje"] = "Evaluación eliminada correctamente.";
                return RedirectToAction("ListarEvaluaciones");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al eliminar la evaluación: " + ex.Message;
                return RedirectToAction("ListarEvaluaciones");
            }
        }

        // MÉTODOS AUXILIARES PARA LISTAS
        private SelectList ObtenerCodEstudiantes()
        {
            var estudiantes = new List<SelectListItem>();
            using (SqlConnection cn = new SqlConnection(connectionString))
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
            return new SelectList(estudiantes, "Value", "Text");
        }

        private SelectList ObtenerCodAcademicos()
        {
            var academicos = new List<SelectListItem>();
            using (SqlConnection cn = new SqlConnection(connectionString))
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
            return new SelectList(academicos, "Value", "Text");
        }

        private SelectList ObtenerCodCursos()
        {
            var cursos = new List<SelectListItem>();
            using (SqlConnection cn = new SqlConnection(connectionString))
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
            return new SelectList(cursos, "Value", "Text");
        }

        private SelectList ObtenerBimestres()
        {
            return new SelectList(new[]
            {
                new { Value = "1", Text = "Primer Bimestre" },
                new { Value = "2", Text = "Segundo Bimestre" },
                new { Value = "3", Text = "Tercer Bimestre" },
                new { Value = "4", Text = "Cuarto Bimestre" }
            }, "Value", "Text");
        }

        [HttpGet]
        public ActionResult ResultadosNotas()
        {
            // Asegúrate de que las notas ya estén en ViewBag o TempData desde otros métodos.
            var notas = ViewBag.Notas as List<Evaluacion>;

            if (notas == null || notas.Count == 0)
            {
                ViewBag.ErrorMessage = "No se encontraron notas para el código proporcionado.";
            }

            return View(notas);
        }


        [HttpGet]
        public ActionResult BuscarNotas()
        {
            return View();

        }

        [HttpPost]
        public ActionResult BuscarNotas(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo.";
                return View();
            }

            List<Nota> notas = new List<Nota>();

            if (tipo == "Estudiante")
            {
                notas = ObtenerNotaEstudiante(codigo);
            }
            else if (tipo == "Academico")
            {
                notas = ObtenerNotaAcademico(codigo);
            }

            if (notas.Count == 0)
            {
                TempData["ErrorMessage"] = "No se encontraron notas para el código proporcionado.";
            }

            return View("ResultadosNotas", notas);
        }

        private List<Nota> ObtenerNotaEstudiante(string codEstudiante)
        {
            var notas = new List<Nota>();

            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("BuscarNotasPorCodigo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodEstudiante", codEstudiante);
                    cmd.Parameters.AddWithValue("@CodAcademico", DBNull.Value);

                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            notas.Add(new Nota
                            {
                                IdEvaluacion = dr["idEvaluacion"].ToString(),
                                NombreCurso = dr["nombreCurso"].ToString(),
                                Bimestre = Convert.ToInt32(dr["bimestre"]),
                                T1 = dr["T1"].ToString(),
                                T2 = dr["T2"].ToString(),
                                T3 = dr["T3"].ToString(),
                                T4 = dr["T4"].ToString(),
                                T5 = dr["T5"].ToString(),
                                PromedioLetra = dr["promedioLetra"].ToString()
                            });
                        }
                    }
                }
            }

            return notas;
        }

        private List<Nota> ObtenerNotaAcademico(string codAcademico)
        {
            var notas = new List<Nota>();

            using (var cn = new SqlConnection(connectionString))
            {
                using (var cmd = new SqlCommand("BuscarNotasPorCodigo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@CodEstudiante", DBNull.Value);
                    cmd.Parameters.AddWithValue("@CodAcademico", codAcademico);

                    cn.Open();
                    using (var dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            notas.Add(new Nota
                            {
                                IdEvaluacion = dr["idEvaluacion"].ToString(),
                                NombreCurso = dr["nombreCurso"].ToString(),
                                Bimestre = Convert.ToInt32(dr["bimestre"]),
                                T1 = dr["T1"].ToString(),
                                T2 = dr["T2"].ToString(),
                                T3 = dr["T3"].ToString(),
                                T4 = dr["T4"].ToString(),
                                T5 = dr["T5"].ToString(),
                                PromedioLetra = dr["promedioLetra"].ToString()
                            });
                        }
                    }
                }
            }

            return notas;
        }

        [HttpGet]
        public ActionResult ListarAcumulados()
        {
            List<AcumuladoBimestre> acumulados = new List<AcumuladoBimestre>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ListarAcumuladoBimestres", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            acumulados.Add(new AcumuladoBimestre
                            {
                                IdAcumulado = Convert.ToInt32(dr["idAcumulado"]),
                                CodEstudiante = dr["codEstudiante"]?.ToString(),
                                CodAcademico = dr["codAcademico"]?.ToString(),
                                IdCurso = dr["idCurso"].ToString(),
                                Bimestre = Convert.ToInt32(dr["bimestre"]),
                                T1 = dr["T1"]?.ToString(),
                                T2 = dr["T2"]?.ToString(),
                                T3 = dr["T3"]?.ToString(),
                                T4 = dr["T4"]?.ToString(),
                                T5 = dr["T5"]?.ToString(),
                                PromedioLetra = dr["promedioLetra"]?.ToString(),
                            });
                        }
                    }
                }
            }

            return View(acumulados);
        }

        [HttpGet]
        public ActionResult ConsultarAcumulado(string codigo, string tipo)
        {
            if (string.IsNullOrEmpty(codigo) || string.IsNullOrEmpty(tipo))
            {
                TempData["ErrorMessage"] = "Debe proporcionar un código y seleccionar un tipo.";
                return RedirectToAction("Buscar");
            }

            List<AcumuladoBimestre> acumulados = new List<AcumuladoBimestre>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ConsultarAcumuladoPorCodigo", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue(tipo == "Estudiante" ? "@CodEstudiante" : "@CodAcademico", codigo);
                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            acumulados.Add(new AcumuladoBimestre
                            {
                                IdAcumulado = Convert.ToInt32(dr["idAcumulado"]),
                                CodEstudiante = dr["codEstudiante"]?.ToString(),
                                CodAcademico = dr["codAcademico"]?.ToString(),
                                IdCurso = dr["idCurso"].ToString(),
                                Bimestre = Convert.ToInt32(dr["bimestre"]),
                                T1 = dr["T1"]?.ToString(),
                                T2 = dr["T2"]?.ToString(),
                                T3 = dr["T3"]?.ToString(),
                                T4 = dr["T4"]?.ToString(),
                                T5 = dr["T5"]?.ToString(),
                                PromedioLetra = dr["promedioLetra"]?.ToString(),
                            });
                        }
                    }
                }
            }

            return View("ResultadosAcumulados", acumulados);
        }

        [HttpPost]
        public ActionResult ActualizarAcumulado(AcumuladoBimestre acumulado)
        {
            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ActualizarAcumuladoBimestre", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdAcumulado", acumulado.IdAcumulado);
                        cmd.Parameters.AddWithValue("@CodEstudiante", acumulado.CodEstudiante ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", acumulado.CodAcademico ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IdCurso", acumulado.IdCurso);
                        cmd.Parameters.AddWithValue("@Bimestre", acumulado.Bimestre);
                        cmd.Parameters.AddWithValue("@T1", acumulado.T1 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@T2", acumulado.T2 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@T3", acumulado.T3 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@T4", acumulado.T4 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@T5", acumulado.T5 ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PromedioLetra", acumulado.PromedioLetra ?? (object)DBNull.Value);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Acumulado actualizado correctamente.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al actualizar el acumulado: " + ex.Message;
            }

            return RedirectToAction("ListarAcumulados");
        }

        // Acción 1: Buscar Bimestre (GET)
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult BuscarBimestre()
        {
            return View();
        }

        // Acción 1: Buscar Bimestre (POST)
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult BuscarBimestre(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo.";
                return View();
            }

            try
            {
                List<Evaluacion> notas = new List<Evaluacion>();
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarNotasPorCodigo", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros
                        cmd.Parameters.AddWithValue("@CodEstudiante", tipo == "Estudiante" ? codigo : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", tipo == "Academico" ? codigo : (object)DBNull.Value);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                notas.Add(new Evaluacion
                                {
                                    IdCurso = dr["nombreCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                });
                            }
                        }
                    }
                }

                // Mapeo al modelo esperado por la vista
                var acumulados = notas.Select(nota => new AcumuladoBimestre
                {
                    IdCurso = nota.IdCurso,
                    Bimestre = nota.Bimestre,
                    T1 = nota.T1,
                    T2 = nota.T2,
                    T3 = nota.T3,
                    T4 = nota.T4,
                    T5 = nota.T5,
                    PromedioLetra = nota.PromedioLetra
                }).ToList();

                return View("ListarPromedioBimestres", acumulados); // Enviar datos transformados
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al buscar las notas: " + ex.Message;
                return View();
            }
        }


        // Acción 2: Listar Promedio de Bimestres (GET)
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult ListarPromedioBimestres(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo.";
                return RedirectToAction("BuscarBimestre");
            }

            try
            {
                List<AcumuladoBimestre> acumulados = new List<AcumuladoBimestre>();
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarAcumuladoPorCodigo", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parámetros
                        cmd.Parameters.AddWithValue("@CodEstudiante", tipo == "Estudiante" ? codigo : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", tipo == "Academico" ? codigo : (object)DBNull.Value);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                acumulados.Add(new AcumuladoBimestre
                                {
                                    IdCurso = dr["idCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                });
                            }
                        }
                    }
                }

                return View(acumulados);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al listar los acumulados: " + ex.Message;
                return RedirectToAction("BuscarBimestre");
            }
        }



        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult ConsultarAcumuladoPorCodigo(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo.";
                return RedirectToAction("BuscarBimestre");
            }

            List<AcumuladoBimestre> acumulados = new List<AcumuladoBimestre>();

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ConsultarAcumuladoPorCodigo", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodEstudiante", tipo == "Estudiante" ? codigo : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", tipo == "Academico" ? codigo : (object)DBNull.Value);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                acumulados.Add(new AcumuladoBimestre
                                {
                                    IdAcumulado = Convert.ToInt32(dr["idAcumulado"]),
                                    CodEstudiante = dr["codEstudiante"]?.ToString(),
                                    CodAcademico = dr["codAcademico"]?.ToString(),
                                    IdCurso = dr["idCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al consultar el acumulado: " + ex.Message;
                return RedirectToAction("BuscarBimestre");
            }

            return View(acumulados); // Redirige a la vista con los datos obtenidos
        }
        [HttpGet]

        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult ListarAcumulados(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo.";
                return RedirectToAction("BuscarBimestre");
            }

            List<AcumuladoBimestre> acumulados = new List<AcumuladoBimestre>();

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarAcumuladoPorCodigo", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CodEstudiante", tipo == "Estudiante" ? codigo : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CodAcademico", tipo == "Academico" ? codigo : (object)DBNull.Value);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                acumulados.Add(new AcumuladoBimestre
                                {
                                    IdAcumulado = Convert.ToInt32(dr["idAcumulado"]),
                                    CodEstudiante = dr["codEstudiante"]?.ToString(),
                                    CodAcademico = dr["codAcademico"]?.ToString(),
                                    IdCurso = dr["idCurso"].ToString(),
                                    Bimestre = Convert.ToInt32(dr["bimestre"]),
                                    T1 = dr["T1"]?.ToString(),
                                    T2 = dr["T2"]?.ToString(),
                                    T3 = dr["T3"]?.ToString(),
                                    T4 = dr["T4"]?.ToString(),
                                    T5 = dr["T5"]?.ToString(),
                                    PromedioLetra = dr["promedioLetra"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al listar los acumulados: " + ex.Message;
                return RedirectToAction("BuscarBimestre");
            }

            return View(acumulados);
        }


        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult VerNotaMasAlta(string codigo, string tipo)
        {
            int notaMasAlta = ObtenerNotaMasAlta(codigo, tipo);
            ViewBag.NotaMasAlta = notaMasAlta;
            return View();
        }



        public int ObtenerNotaMasAlta(string codigo, string tipo)
        {
            int notaMasAlta = 0;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = cn;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = @"
                SELECT MAX(CASE 
                    WHEN T1 = 'AD' THEN 20
                    WHEN T1 = 'A' THEN 16
                    WHEN T1 = 'B' THEN 12
                    WHEN T1 = 'C' THEN 8
                    ELSE 0 END) AS NotaMaxima
                FROM tb_AcumuladoBimestres
                WHERE " + (tipo == "Estudiante" ? "codEstudiante" : "codAcademico") + " = @Codigo";

                    cmd.Parameters.AddWithValue("@Codigo", codigo);

                    cn.Open();
                    var resultado = cmd.ExecuteScalar();
                    if (resultado != DBNull.Value)
                    {
                        notaMasAlta = Convert.ToInt32(resultado);
                    }
                }
            }

            return notaMasAlta;
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]
        public ActionResult BuscarTopPromedios()
        {
            // Retorna una vista inicial sin resultados
            return View(new List<TopPromedioViewModel>());
        }


        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]
        [ValidateAntiForgeryToken]
        public ActionResult BuscarTopPromedios(int anioEstudiante)
        {
            var resultados = new List<TopPromedioViewModel>();

            try
            {
                resultados = ObtenerTopPromedios(anioEstudiante);

                if (resultados == null || !resultados.Any())
                {
                    ViewBag.ErrorMessage = "No se encontraron resultados para el año ingresado.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Ocurrió un error al buscar los datos: " + ex.Message;
            }

            // Retorna la vista con los resultados obtenidos
            return View(resultados);
        }


        public List<TopPromedioViewModel> ObtenerTopPromedios(int anioEstudiante)
        {
            var resultados = new List<TopPromedioViewModel>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("ObtenerTopPromedios", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Agrega los parámetros
                    cmd.Parameters.AddWithValue("@AnioEstudiante", anioEstudiante);

                    cn.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            resultados.Add(new TopPromedioViewModel
                            {
                                Codigo = dr["codEstudiante"] == DBNull.Value ? string.Empty : dr["codEstudiante"].ToString(),
                                Nombre = dr["nombreEstudiante"] == DBNull.Value ? string.Empty : dr["nombreEstudiante"].ToString(),
                                Apellido = dr["apellidoEstudiante"] == DBNull.Value ? string.Empty : dr["apellidoEstudiante"].ToString(),
                                Grado = dr["gradoAcademico"] == DBNull.Value ? string.Empty : dr["gradoAcademico"].ToString(),
                                Anio = dr["anioEstudiante"] == DBNull.Value ? 0 : SafeConvertToInt(dr["anioEstudiante"]),
                                IdCurso = dr["idCurso"] == DBNull.Value ? 0 : SafeConvertToInt(dr["idCurso"]),
                                Bimestre = dr["bimestre"] == DBNull.Value ? 0 : SafeConvertToInt(dr["bimestre"]),
                                PromedioBimestre = dr["promedioBimestre"] == DBNull.Value ? 0 : SafeConvertToDecimal(dr["promedioBimestre"]),
                                PromedioGeneral = dr["promedioGeneral"] == DBNull.Value ? 0 : SafeConvertToDecimal(dr["promedioGeneral"]),
                                PromedioLetra = dr["promedioLetra"] == DBNull.Value ? string.Empty : dr["promedioLetra"].ToString()
                            });

                        }
                    }
                }
            }

            return resultados;
        }

        private int SafeConvertToInt(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (int.TryParse(value.ToString(), out int result))
                return result;

            return 0; // Valor por defecto si la conversión falla
        }

        private decimal SafeConvertToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
                return 0;

            if (decimal.TryParse(value.ToString(), out decimal result))
                return result;

            return 0; // Valor por defecto si la conversión falla
        }


        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]
        public ActionResult BuscarEstudianteOAcademico()
        {
            return View();
        }

        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]
        [ValidateAntiForgeryToken]
        public ActionResult BuscarEstudianteOAcademico(string codigo, string tipo)
        {
            if (string.IsNullOrWhiteSpace(codigo) || string.IsNullOrWhiteSpace(tipo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido y seleccionar un tipo (Estudiante o Académico).";
                return RedirectToAction("BuscarEstudianteOAcademico");
            }

            try
            {
                if (tipo == "Estudiante")
                {
                    return RedirectToAction("AgregarNotas", new { idEstudiante = codigo });
                }
                else if (tipo == "Academico")
                {
                    return RedirectToAction("AgregarNotas", new { idAcademico = codigo });
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Error al buscar: " + ex.Message;
            }

            return RedirectToAction("BuscarEstudianteOAcademico");
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult AgregarNotas(string idEstudiante = null, string idAcademico = null)
        {
            // Pasar datos iniciales a la vista
            ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
            ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
            ViewBag.Cursos = ObtenerCodCursos();

            // Crear una nueva instancia del modelo y asignar valores iniciales
            var evaluacion = new Evaluacion
            {
                CodEstudiante = idEstudiante,
                CodAcademico = idAcademico
            };

            return View(evaluacion);
        }

        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [ValidateAntiForgeryToken]
        public ActionResult AgregarNotas(Evaluacion evaluacion)
        {
            if (string.IsNullOrEmpty(evaluacion.CodEstudiante) && string.IsNullOrEmpty(evaluacion.CodAcademico))
            {
                ModelState.AddModelError("", "Debe seleccionar un estudiante o un académico.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (SqlConnection cn = new SqlConnection(connectionString))
                    {
                        using (SqlCommand cmd = new SqlCommand("IngresarNotasPorCursoYBimestre", cn))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            // Generar un nuevo ID si no se proporcionó
                            if (string.IsNullOrEmpty(evaluacion.IdEvaluacion))
                            {
                                evaluacion.IdEvaluacion = Guid.NewGuid().ToString();
                            }

                            // Asignar parámetros al procedimiento almacenado
                            cmd.Parameters.AddWithValue("@IdEvaluacion", evaluacion.IdEvaluacion);
                            cmd.Parameters.AddWithValue("@CodEstudiante", string.IsNullOrEmpty(evaluacion.CodEstudiante) ? (object)DBNull.Value : evaluacion.CodEstudiante);
                            cmd.Parameters.AddWithValue("@CodAcademico", string.IsNullOrEmpty(evaluacion.CodAcademico) ? (object)DBNull.Value : evaluacion.CodAcademico);
                            cmd.Parameters.AddWithValue("@IdCurso", evaluacion.IdCurso);
                            cmd.Parameters.AddWithValue("@Bimestre", evaluacion.Bimestre);
                            cmd.Parameters.AddWithValue("@T1", string.IsNullOrEmpty(evaluacion.T1) ? (object)DBNull.Value : evaluacion.T1);
                            cmd.Parameters.AddWithValue("@T2", string.IsNullOrEmpty(evaluacion.T2) ? (object)DBNull.Value : evaluacion.T2);
                            cmd.Parameters.AddWithValue("@T3", string.IsNullOrEmpty(evaluacion.T3) ? (object)DBNull.Value : evaluacion.T3);
                            cmd.Parameters.AddWithValue("@T4", string.IsNullOrEmpty(evaluacion.T4) ? (object)DBNull.Value : evaluacion.T4);
                            cmd.Parameters.AddWithValue("@T5", string.IsNullOrEmpty(evaluacion.T5) ? (object)DBNull.Value : evaluacion.T5);

                            cn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }

                    TempData["Mensaje"] = "Notas agregadas correctamente.";
                    return RedirectToAction("ListarEvaluaciones");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "Error al agregar las notas: " + ex.Message;
                }
            }

            // Recargar opciones en caso de error
            ViewBag.CodEstudianteOptions = ObtenerCodEstudiantes();
            ViewBag.CodAcademicoOptions = ObtenerCodAcademicos();
            ViewBag.Cursos = ObtenerCodCursos();
            return View(evaluacion);
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]

        public ActionResult BuscarPromedios()
        {
            return View();
        }


        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante", "Academico")]
        [ValidateAntiForgeryToken]
        public ActionResult BuscarPromedios(string codigo)
        {

            if (string.IsNullOrWhiteSpace(codigo))
            {
                TempData["ErrorMessage"] = "Debe ingresar un código válido.";
                return RedirectToAction("BuscarPromedios");
            }

            var resultados = ObtenerPromedios(codigo);

            if (resultados == null || !resultados.Any())
            {
                TempData["ErrorMessage"] = "No se encontraron resultados para los criterios proporcionados.";
                return RedirectToAction("BuscarPromedios");
            }

            return View("ResultadosPromedios", resultados);
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor", "Estudiante","Academico")]

        public ActionResult ResultadosPromedios(List<PromedioViewModel> resultados)
        {
            if (resultados == null || !resultados.Any())
            {
                TempData["ErrorMessage"] = "No se encontraron resultados para mostrar.";
                return RedirectToAction("BuscarPromedios");
            }

            return View(resultados);
        }


        private List<PromedioViewModel> ObtenerPromedios(string codigo)
        {
            var resultados = new List<PromedioViewModel>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("CalcularPromedioGeneralPorCurso", cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Proveer solo @CodEstudiante
                    cmd.Parameters.AddWithValue("@CodEstudiante", (object)codigo ?? DBNull.Value);

                    cn.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            resultados.Add(new PromedioViewModel
                            {
                                CodEstudiante_CodAcademico = dr["CodEstudiante_CodAcademico"].ToString(),
                                NombreEstudiante_Academico = dr["NombreEstudiante_Academico"].ToString(),
                                AnioEscolar = dr["AnioEscolar"].ToString(),
                                GradoAcademico = dr["GradoAcademico"].ToString(),
                                Curso = dr["Curso"].ToString(),
                                PrimerBimestre = dr["PrimerBimestre"].ToString(),
                                SegundoBimestre = dr["SegundoBimestre"].ToString(),
                                TercerBimestre = dr["TercerBimestre"].ToString(),
                                CuartoBimestre = dr["CuartoBimestre"].ToString(),
                                PromedioFinal = dr["PromedioFinalLetra"].ToString()
                            });
                        }
                    }
                }
            }

            return resultados;
        }



    }

}





