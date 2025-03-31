using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Mvc;
using Arquimedes.Models;

namespace Arquimedes.Controllers
{
    public class ProfesorController : Controller
    {
        // Conexión a la base de datos utilizando ConfigurationManager
        private readonly SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["cadena"].ConnectionString);
        private SqlCommand cmd;
        private string mensaje;

        // GET: ListarProfesores
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ListarProfesores()
        {
            List<Profesor> profesores = new List<Profesor>();

            try
            {
                // Definir consulta para obtener los datos
                string query = "SELECT * FROM tb_Profesor";
                cmd = new SqlCommand(query, cn);

                // Abrir conexión
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        // Agregar cada registro a la lista
                        profesores.Add(new Profesor
                        {
                            IdProfesor = dr["idProfesor"].ToString(),
                            NombreProfesor = dr["nombreProfesor"].ToString(),
                            ApellidoProfesor = dr["apellidoProfesor"].ToString(),
                            EdadProfesor = (int)dr["edadProfesor"],
                            DniProfesor = dr["dniProfesor"].ToString(),
                            TelefonoProfesor = dr["telefonoProfesor"].ToString(),
                            DireccionProfesor = dr["direccionProfesor"].ToString(),
                            EstadoProfesor = dr["estadoProfesor"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                mensaje = "Error al obtener la lista de profesores: " + ex.Message;
                ViewBag.Error = mensaje;
            }
            finally
            {
                // Asegurarse de cerrar la conexión
                if (cn.State == System.Data.ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            return View(profesores); // Devolver la lista de profesores a la vista
        }

        // GET: RegistrarProfesor
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult RegistrarProfesor()
        {
            return View(new Profesor());
        }

        // POST: RegistrarProfesor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult RegistrarProfesor(Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string query = @"INSERT INTO tb_Profesor (idProfesor, nombreProfesor, apellidoProfesor, edadProfesor, dniProfesor, telefonoProfesor, direccionProfesor, estadoProfesor)
                                     VALUES (@IdProfesor, @NombreProfesor, @ApellidoProfesor, @EdadProfesor, @DniProfesor, @TelefonoProfesor, @DireccionProfesor, @EstadoProfesor)";
                    cmd = new SqlCommand(query, cn);

                    cmd.Parameters.AddWithValue("@IdProfesor", profesor.IdProfesor);
                    cmd.Parameters.AddWithValue("@NombreProfesor", profesor.NombreProfesor);
                    cmd.Parameters.AddWithValue("@ApellidoProfesor", profesor.ApellidoProfesor);
                    cmd.Parameters.AddWithValue("@EdadProfesor", profesor.EdadProfesor);
                    cmd.Parameters.AddWithValue("@DniProfesor", profesor.DniProfesor);
                    cmd.Parameters.AddWithValue("@TelefonoProfesor", profesor.TelefonoProfesor);
                    cmd.Parameters.AddWithValue("@DireccionProfesor", profesor.DireccionProfesor);
                    cmd.Parameters.AddWithValue("@EstadoProfesor", profesor.EstadoProfesor);

                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        TempData["Mensaje"] = "Profesor registrado correctamente.";
                        return RedirectToAction("ListarProfesores");
                    }

                    ViewBag.Mensaje = "No se pudo registrar el profesor.";
                }
                catch (SqlException ex)
                {
                    ViewBag.Mensaje = "Error al registrar el profesor: " + ex.Message;
                }
                finally
                {
                    if (cn.State == System.Data.ConnectionState.Open)
                        cn.Close();
                }
            }

            return View(profesor);
        }

        // GET: detalleProfesor
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult detalleProfesor(string idProfesor)
        {
            if (string.IsNullOrEmpty(idProfesor))
            {
                return HttpNotFound("El código del profesor no fue especificado.");
            }

            Profesor profesor = null;

            try
            {
                // Definir el procedimiento almacenado para obtener detalles
                string query = "BuscarProfesor";
                cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Agregar el parámetro al comando
                cmd.Parameters.AddWithValue("@IdProfesor", idProfesor);

                // Abrir conexión
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        // Mapear los datos del profesor
                        profesor = new Profesor
                        {
                            IdProfesor = dr["idProfesor"].ToString(),
                            NombreProfesor = dr["nombreProfesor"].ToString(),
                            ApellidoProfesor = dr["apellidoProfesor"].ToString(),
                            EdadProfesor = (int)dr["edadProfesor"],
                            DniProfesor = dr["dniProfesor"].ToString(),
                            TelefonoProfesor = dr["telefonoProfesor"].ToString(),
                            DireccionProfesor = dr["direccionProfesor"].ToString(),
                            EstadoProfesor = dr["estadoProfesor"].ToString()
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los detalles del profesor: " + ex.Message;
            }
            finally
            {
                // Asegurarse de cerrar la conexión
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            if (profesor == null)
            {
                return HttpNotFound("Profesor no encontrado.");
            }

            return View(profesor); // Pasar el modelo a la vista
        }
        // GET: ActualizarProfesor
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ActualizarProfesor(string idProfesor)
        {
            if (string.IsNullOrEmpty(idProfesor))
            {
                return HttpNotFound("El código del profesor no fue especificado.");
            }

            Profesor profesor = null;

            try
            {
                // Definir consulta para obtener el profesor por ID
                string query = "BuscarProfesor";
                cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@IdProfesor", idProfesor);

                // Abrir conexión
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesor = new Profesor
                        {
                            IdProfesor = dr["idProfesor"].ToString(),
                            NombreProfesor = dr["nombreProfesor"].ToString(),
                            ApellidoProfesor = dr["apellidoProfesor"].ToString(),
                            EdadProfesor = (int)dr["edadProfesor"],
                            DniProfesor = dr["dniProfesor"].ToString(),
                            TelefonoProfesor = dr["telefonoProfesor"].ToString(),
                            DireccionProfesor = dr["direccionProfesor"].ToString(),
                            EstadoProfesor = dr["estadoProfesor"].ToString()
                        };
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los detalles del profesor: " + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            if (profesor == null)
            {
                return HttpNotFound("Profesor no encontrado.");
            }

            return View(profesor);
        }

        // POST: ActualizarProfesor
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult ActualizarProfesor(Profesor profesor)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Verificar los datos recibidos
                    Console.WriteLine($"IdProfesor: {profesor.IdProfesor}");
                    Console.WriteLine($"Nombre: {profesor.NombreProfesor}");
                    Console.WriteLine($"Apellido: {profesor.ApellidoProfesor}");
                    Console.WriteLine($"Edad: {profesor.EdadProfesor}");
                    Console.WriteLine($"DNI: {profesor.DniProfesor}");
                    Console.WriteLine($"Teléfono: {profesor.TelefonoProfesor}");
                    Console.WriteLine($"Dirección: {profesor.DireccionProfesor}");
                    Console.WriteLine($"Estado: {profesor.EstadoProfesor}");

                    string query = "ActualizarProfesor";
                    cmd = new SqlCommand(query, cn)
                    {
                        CommandType = CommandType.StoredProcedure
                    };

                    // Agregar parámetros
                    cmd.Parameters.AddWithValue("@IdProfesor", profesor.IdProfesor);
                    cmd.Parameters.AddWithValue("@NombreProfesor", profesor.NombreProfesor);
                    cmd.Parameters.AddWithValue("@ApellidoProfesor", profesor.ApellidoProfesor);
                    cmd.Parameters.AddWithValue("@EdadProfesor", profesor.EdadProfesor);
                    cmd.Parameters.AddWithValue("@DniProfesor", profesor.DniProfesor);
                    cmd.Parameters.AddWithValue("@TelefonoProfesor", profesor.TelefonoProfesor);
                    cmd.Parameters.AddWithValue("@DireccionProfesor", profesor.DireccionProfesor);
                    cmd.Parameters.AddWithValue("@EstadoProfesor", profesor.EstadoProfesor);

                    // Ejecutar el comando
                    cn.Open();
                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        TempData["Mensaje"] = "Profesor actualizado correctamente.";
                        return RedirectToAction("ListarProfesores");
                    }
                    else
                    {
                        ViewBag.Mensaje = "No se pudo actualizar el profesor.";
                    }
                }
                catch (SqlException ex)
                {
                    ViewBag.ErrorMessage = "Error al actualizar el profesor: " + ex.Message;
                }
                finally
                {
                    if (cn.State == ConnectionState.Open)
                        cn.Close();
                }
            }

            return View(profesor);
        }

        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult EliminarProfesor(string idProfesor)
        {
            if (string.IsNullOrEmpty(idProfesor))
            {
                return HttpNotFound("El código del profesor no fue especificado.");
            }

            Profesor profesor = null;

            try
            {
                string query = "BuscarProfesor";
                cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@IdProfesor", idProfesor);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        profesor = new Profesor
                        {
                            IdProfesor = dr["idProfesor"].ToString(),
                            NombreProfesor = dr["nombreProfesor"].ToString(),
                            ApellidoProfesor = dr["apellidoProfesor"].ToString(),
                            EdadProfesor = (int)dr["edadProfesor"],
                            DniProfesor = dr["dniProfesor"].ToString(),
                            TelefonoProfesor = dr["telefonoProfesor"].ToString(),
                            DireccionProfesor = dr["direccionProfesor"].ToString(),
                            EstadoProfesor = dr["estadoProfesor"].ToString()
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los detalles del profesor: " + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            if (profesor == null)
            {
                return HttpNotFound("Profesor no encontrado.");
            }

            return View(profesor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult EliminarProfesorConfirmed(string idProfesor)
        {
            if (string.IsNullOrEmpty(idProfesor))
            {
                return HttpNotFound("El código del profesor no fue especificado.");
            }

            try
            {
                string query = "EliminarProfesor";
                cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@IdProfesor", idProfesor);

                cn.Open();

                int result = cmd.ExecuteNonQuery();
                if (result > 0)
                {
                    TempData["Mensaje"] = "Profesor eliminado correctamente.";
                    return RedirectToAction("ListarProfesores");
                }

                ViewBag.ErrorMessage = "No se pudo eliminar el profesor.";
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Error al eliminar el profesor: " + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            return RedirectToAction("ListarProfesores");
        }
        [HttpGet]
        [RoleAuthorize("Administrador", "Asistente","Profesor")]

        public ActionResult VerCursosDictados(string idProfesor)
        {
            if (string.IsNullOrEmpty(idProfesor))
            {
                ViewBag.ErrorMessage = "Debe ingresar un código de profesor.";
                return View(new List<Curso>()); // Devolver una lista vacía
            }

            List<Curso> cursos = new List<Curso>();

            try
            {
                string query = "VerCursosProfesor"; // Procedimiento almacenado
                cmd = new SqlCommand(query, cn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.AddWithValue("@IdProfesor", idProfesor);

                cn.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        cursos.Add(new Curso
                        {
                            IdCurso = dr["CodigoCurso"].ToString(),
                            NombreCurso = dr["NombreCurso"].ToString(),
                            DescripcionCurso = dr["DescripcionCurso"].ToString(),
                            Dia = dr["DiaCurso"].ToString(),
                            Hora = dr["HoraCurso"] == DBNull.Value ? null : dr["HoraCurso"].ToString(),
                            Nivel = dr["NivelCurso"].ToString()
                        });
                    }
                }
            }
            catch (SqlException ex)
            {
                ViewBag.ErrorMessage = "Error al obtener los cursos dictados: " + ex.Message;
            }
            finally
            {
                if (cn.State == ConnectionState.Open)
                {
                    cn.Close();
                }
            }

            return View(cursos);
        }

        [RoleAuthorize("Administrador", "Asistente", "Profesor")]

        public ActionResult VistaProfesor()
        {
            return View();
        }
    }
}