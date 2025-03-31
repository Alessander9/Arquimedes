using Arquimedes.Controllers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class EstudianteController : Controller
    {
        readonly SqlConnection cn = new SqlConnection(ConfigurationManager.ConnectionStrings["cadena"].ConnectionString);
        SqlCommand cmd;
        string mensaje;

        [HttpGet]
        public ActionResult RegistrarEstudiante()
        {
            ViewData["GradoAcademicoOptions"] = new SelectList(new[]
   {
        new { Value = "Inicial", Text = "Inicial" },
        new { Value = "Primaria", Text = "Primaria" },
        new { Value = "Secundaria", Text = "Secundaria" }
    }, "Value", "Text");

            ViewData["EstadoEstudianteOptions"] = new SelectList(new[]
            {
        new { Value = "Activo", Text = "Activo" },
        new { Value = "Inactivo", Text = "Inactivo" }
    }, "Value", "Text");

            return View(new Estudiante());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrarEstudiante(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    cn.Open();
                    cmd = new SqlCommand("RegistrarEstudiante", cn);
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

                    int result = cmd.ExecuteNonQuery();

                    if (result > 0)
                    {
                        mensaje = "Estudiante registrado correctamente";
                    }
                    else
                    {
                        mensaje = "No se pudo registrar el estudiante.";
                    }

                    ViewBag.Mensaje = mensaje;
                }
                catch (SqlException ex)
                {
                    ViewBag.Mensaje = "Error al registrar el estudiante: " + ex.Message;
                }
                finally
                {
                    cn.Close();
                }

                return RedirectToAction("ListarEstudiantes");

            }
            ViewData["GradoAcademicoOptions"] = new SelectList(new[]
  {
        new { Value = "Inicial", Text = "Inicial" },
        new { Value = "Primaria", Text = "Primaria" },
        new { Value = "Secundaria", Text = "Secundaria" }
    }, "Value", "Text");

            ViewData["EstadoEstudianteOptions"] = new SelectList(new[]
            {
        new { Value = "Activo", Text = "Activo" },
        new { Value = "Inactivo", Text = "Inactivo" }
    }, "Value", "Text");

            ViewBag.Mensaje = "El modelo no es válido. Revise los datos ingresados.";
            return View(estudiante);



        }

        public ActionResult ListarEstudiantes()
        {
            List<Estudiante> estudiantes = new List<Estudiante>();

            try
            {
                cn.Open();
                cmd = new SqlCommand("ListarEstudiantes", cn);
                cmd.CommandType = CommandType.StoredProcedure;
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
                        EstadoEstudiante = dr["estadoEstudiante"].ToString()
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

        // GET: ActualizarEstudiante
        [HttpGet]
        public ActionResult ActualizarEstudiante(string CodEstudiante)
        {
            Estudiante estudiante = new Estudiante();

            try
            {
                cn.Open();
                cmd = new SqlCommand("ObtenerEstudiantePorID", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CodEstudiante", CodEstudiante);

                SqlDataReader dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    estudiante.CodEstudiante = dr["codEstudiante"].ToString();
                    estudiante.NombreEstudiante = dr["nombreEstudiante"].ToString();
                    estudiante.ApellidoEstudiante = dr["apellidoEstudiante"].ToString();
                    estudiante.EdadEstudiante = Convert.ToInt32(dr["edadEstudiante"]);
                    estudiante.AnioEstudiante = Convert.ToInt32(dr["anioEstudiante"]);
                    estudiante.SeccionEstudiante = dr["seccionEstudiante"].ToString();
                    estudiante.GradoAcademico = dr["gradoAcademico"].ToString();
                    estudiante.Telefono = dr["telefono"].ToString();
                    estudiante.Direccion = dr["direccion"].ToString();
                    estudiante.EstadoEstudiante = dr["estadoEstudiante"].ToString();
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

            ViewData["GradoAcademicoOptions"] = new SelectList(new[]
            {
                new { Value = "Inicial", Text = "Inicial" },
                new { Value = "Primaria", Text = "Primaria" },
                new { Value = "Secundaria", Text = "Secundaria" }
            }, "Value", "Text", estudiante.GradoAcademico);

            ViewData["EstadoEstudianteOptions"] = new SelectList(new[]
            {
                new { Value = "Activo", Text = "Activo" },
                new { Value = "Inactivo", Text = "Inactivo" }
            }, "Value", "Text", estudiante.EstadoEstudiante);

            return View(estudiante);
        }
        // POST: ActualizarEstudiante
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ActualizarEstudiante(Estudiante estudiante)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    cn.Open();
                    cmd = new SqlCommand("ActualizarEstudiante", cn);
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

                    int result = cmd.ExecuteNonQuery();
                    mensaje = result > 0 ? "Estudiante actualizado correctamente" : "No se pudo actualizar el estudiante.";
                    ViewBag.Mensaje = mensaje;
                }
                catch (SqlException ex)
                {
                    ViewBag.Mensaje = "Error al actualizar el estudiante: " + ex.Message;
                }
                finally
                {
                    cn.Close();
                }

                return RedirectToAction("ListarEstudiantes");
            }

            // Si el modelo no es válido, recargar opciones de dropdown y mostrar errores
            ViewData["GradoAcademicoOptions"] = new SelectList(new[]
            {
        new { Value = "Inicial", Text = "Inicial" },
        new { Value = "Primaria", Text = "Primaria" },
        new { Value = "Secundaria", Text = "Secundaria" }
    }, "Value", "Text", estudiante.GradoAcademico);

            ViewData["EstadoEstudianteOptions"] = new SelectList(new[]
            {
        new { Value = "Activo", Text = "Activo" },
        new { Value = "Inactivo", Text = "Inactivo" }
    }, "Value", "Text", estudiante.EstadoEstudiante);

            ViewBag.Mensaje = "El modelo no es válido. Revise los datos ingresados.";
            return View(estudiante);
        }

        public ActionResult BuscarEstudiante(string codEstudiante, string nombre, string apellido)
        {   
            List<Estudiante> estudiantes = new List<Estudiante>();

            try
            {
                cn.Open();
                cmd = new SqlCommand("BuscarEstudiante", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@CodEstudiante", string.IsNullOrEmpty(codEstudiante) ? (object)DBNull.Value : codEstudiante);
                cmd.Parameters.AddWithValue("@Nombre", string.IsNullOrEmpty(nombre) ? (object)DBNull.Value : nombre);
                cmd.Parameters.AddWithValue("@Apellido", string.IsNullOrEmpty(apellido) ? (object)DBNull.Value : apellido);

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
                        EstadoEstudiante = dr["estadoEstudiante"].ToString()
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

            return View("ListarEstudiantes", estudiantes);
        }

    }



}
