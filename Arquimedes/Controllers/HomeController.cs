using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Arquimedes.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contacto()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Nosotros()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult seccionPrimaria()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

     
        public ActionResult Pruebas()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult Plataforma()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult Plataforma2()
        {
            ViewBag.Message = "Your";
            return View();

        }

        public ActionResult FormularioMatricula()
        {
            ViewBag.Message = "Your";
            return View();

        }

        public ActionResult OratoriaEstrategica()
        {

            return View();

        }
        [RoleAuthorize("Administrador", "Asistente")]

        public ActionResult MenuDirector() 
        {
            return View();
        }

        public ActionResult Estudiante() { 
        
            return View();
        }
        
        public ActionResult Academico()
        {
            return View();
        }

        public ActionResult Profesor()
        {
            return View();
        }
         
        public ActionResult cursos()
        {

            return View();
        }

        public ActionResult Horario()
        {
            return View();

        }

        public ActionResult Evaluaciones() { 
        
            return View();
        }

        public ActionResult buscarNotas() { 
            return View();
        }

        public ActionResult buscarnotasBimestre()
        {

            return View();
        }

        
        public ActionResult Oratoria() {

            return View();
        }

        public ActionResult Secundaria() {
            return View();
        }

        public ActionResult NivelAcademico()
        {
            return View();
        }

        public ActionResult Matricula()
        {
            return View();
        }
        public ActionResult Vacaciones()
        {
            return View();
        }
        public ActionResult Historia()
        {
            return View();
        }
        public ActionResult Reglamento()
        {
            return View();
        }
        public ActionResult PerfilEstudiante()
        {
            return View();
        }
        public ActionResult PlanAcademico()
        {
            return View();
        }
        public ActionResult Noticia1()
        {
            return View();
        }
        public ActionResult Noticia2()
        {
            return View();
        }
        public ActionResult Noticia3()
        {
            return View();
        }
        public ActionResult Noticia4()
        {
            return View();
        }
        public ActionResult Noticia5()
        {
            return View();
        }
        public ActionResult Noticia6()
        {
            return View();
        }
        public ActionResult Noticia7()
        {
            return View();
        }
        public ActionResult Noticia8()
        {
            return View();
        }
    }
}