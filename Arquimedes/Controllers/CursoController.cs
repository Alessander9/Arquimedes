using Arquimedes.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class CursosController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

        // GET: Listar todos los cursos
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]
        public ActionResult ListarCursos()
        {
            List<Curso> cursos = new List<Curso>();

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarCursos", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                cursos.Add(new Curso
                                {
                                    IdCurso = dr["idCurso"].ToString(),
                                    NombreCurso = dr["nombreCurso"].ToString(),
                                    DescripcionCurso = dr["descripcionCurso"].ToString(),
                                    Dia = dr["dia"].ToString(),
                                    Hora = dr["hora"].ToString(),
                                    Nivel = dr["nivel"].ToString(),
                                    IdProfesor = dr["idProfesor"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al listar los cursos: " + ex.Message;
            }

            return View(cursos);
        }


        [RoleAuthorize("Administrador", "Asistente", "Profesor")]
        [HttpGet]
        public ActionResult RegistrarCurso()
        {
            CargarProfesores(); // Carga la lista de profesores en ViewBag
            return View(new Curso());
        }


        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarCurso(Curso curso)
        {
            if (!ModelState.IsValid)
            {
                CargarProfesores();
                return View(curso);
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("RegistrarCurso", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdCurso", curso.IdCurso);
                        cmd.Parameters.AddWithValue("@NombreCurso", curso.NombreCurso);
                        cmd.Parameters.AddWithValue("@DescripcionCurso", curso.DescripcionCurso);
                        cmd.Parameters.AddWithValue("@Dia", curso.Dia);
                        cmd.Parameters.AddWithValue("@Hora", TimeSpan.Parse(curso.Hora));
                        cmd.Parameters.AddWithValue("@Nivel", curso.Nivel);
                        cmd.Parameters.AddWithValue("@IdProfesor", curso.IdProfesor);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
                TempData["Mensaje"] = "Curso registrado correctamente.";
                return RedirectToAction("ListarCursos");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al registrar el curso: " + ex.Message);
                CargarProfesores();
                return View(curso);
            }
        }


        // GET: Actualizar un curso existente
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult ActualizarCurso(string idCurso)
        {
            if (string.IsNullOrEmpty(idCurso))
            {
                return HttpNotFound("El ID del curso no fue especificado.");
            }

            Curso curso = BuscarCurso(idCurso);
            if (curso == null)
            {
                return HttpNotFound("Curso no encontrado.");
            }

            CargarProfesores(); // Cargar la lista de profesores en ViewBag
            return View(curso);
        }

        // POST: Actualizar un curso
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [ValidateAntiForgeryToken]
        public ActionResult ActualizarCurso(Curso curso)
        {
            if (!ModelState.IsValid)
            {
                CargarProfesores();
                return View(curso);
            }

            string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ActualizarCurso", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cmd.Parameters.AddWithValue("@IdCurso", curso.IdCurso);
                        cmd.Parameters.AddWithValue("@NombreCurso", curso.NombreCurso);
                        cmd.Parameters.AddWithValue("@DescripcionCurso", curso.DescripcionCurso);
                        cmd.Parameters.AddWithValue("@Dia", curso.Dia);
                        cmd.Parameters.AddWithValue("@Hora", TimeSpan.Parse(curso.Hora));
                        cmd.Parameters.AddWithValue("@Nivel", curso.Nivel);
                        cmd.Parameters.AddWithValue("@IdProfesor", curso.IdProfesor);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Curso actualizado correctamente.";
                return RedirectToAction("ListarCursos");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al actualizar el curso: " + ex.Message;
                CargarProfesores();
                return View(curso);
            }
        }


        // GET: Eliminar un curso
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult EliminarCurso(string idCurso)
        {
            if (string.IsNullOrEmpty(idCurso))
            {
                return HttpNotFound("El ID del curso no fue especificado.");
            }

            Curso curso = BuscarCurso(idCurso);
            if (curso == null)
            {
                return HttpNotFound("Curso no encontrado.");
            }

            return View(curso);
        }

        // POST: Confirmar eliminación de un curso
        [HttpPost]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        [ValidateAntiForgeryToken]
        public ActionResult EliminarCursoConfirmado(string idCurso)
        {
            if (string.IsNullOrEmpty(idCurso))
            {
                ViewBag.ErrorMessage = "El ID del curso no fue especificado.";
                return View();
            }

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("EliminarCurso", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdCurso", idCurso);

                        cn.Open();
                        cmd.ExecuteNonQuery();
                    }
                }

                TempData["Mensaje"] = "Curso eliminado correctamente.";
                return RedirectToAction("ListarCursos");
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al eliminar el curso: " + ex.Message;
                return View();
            }
        }

        // GET: Mostrar detalles de un curso
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult DetalleCurso(string idCurso)
        {
            if (string.IsNullOrEmpty(idCurso))
            {
                return HttpNotFound("El ID del curso no fue especificado.");
            }

            Curso curso = BuscarCurso(idCurso);
            if (curso == null)
            {
                return HttpNotFound("Curso no encontrado.");
            }

            return View(curso);
        }

        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        // Método auxiliar: Buscar curso por ID
        private Curso BuscarCurso(string idCurso)
        {
            Curso curso = null;

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("BuscarCurso", cn))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@IdCurso", idCurso);

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            if (dr.Read())
                            {
                                curso = new Curso
                                {
                                    IdCurso = dr["idCurso"].ToString(),
                                    NombreCurso = dr["nombreCurso"].ToString(),
                                    DescripcionCurso = dr["descripcionCurso"].ToString(),
                                    Dia = dr["dia"].ToString(),
                                    Hora = dr["hora"].ToString(),
                                    Nivel = dr["nivel"].ToString(),
                                    IdProfesor = dr["idProfesor"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al buscar el curso: " + ex.Message;
            }

            return curso;
        }

        // Método auxiliar: Cargar lista de profesores para un dropdown
        private void CargarProfesores()
        {
            List<SelectListItem> profesores = new List<SelectListItem>();
            string connectionString = ConfigurationManager.ConnectionStrings["cadena"].ConnectionString;

            try
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("ListarProfesores", cn)) // Procedimiento almacenado
                    {
                        cmd.CommandType = CommandType.StoredProcedure;

                        cn.Open();
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                profesores.Add(new SelectListItem
                                {
                                    Value = dr["idProfesor"].ToString(),
                                    Text = $"{dr["nombreProfesor"]} {dr["apellidoProfesor"]}"
                                });
                            }
                        }
                    }
                }

                ViewBag.IdProfesor = new SelectList(profesores, "Value", "Text"); // Crear el SelectList
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al cargar los profesores: " + ex.Message;
            }


        }


    }

}

