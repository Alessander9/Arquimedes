using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Controllers
{
    public class HorarioDetalle
    {
        public string CodigoHorario { get; set; }
        public string NombreCurso { get; set; }
        public string Dia { get; set; }
        public string Hora { get; set; }
        public string Nivel { get; set; }
        public string Profesor { get; set; }



        public HorarioDetalle() { }

        public HorarioDetalle(string codigoHorario, string nombreCurso, string dia, string hora, string nivel, string profesor)
        {
            CodigoHorario = codigoHorario;
            NombreCurso = nombreCurso;
            Dia = dia;
            Hora = hora;
            Nivel = nivel;
            Profesor = profesor;
        }



    }
}