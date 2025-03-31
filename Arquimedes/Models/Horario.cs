using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Horario
    {
        public string IdHorario { get; set; } // Clave primaria
        public string TipoInscripcion { get; set; } // 'Estudiante' o 'Academico'
        public string CodEstudiante { get; set; } // Clave foránea a Estudiante (nullable)
        public string CodAcademico { get; set; } // Clave foránea a Académico (nullable)
        public string IdCurso { get; set; } // Clave foránea a Curso

        // Constructor vacío
        public Horario()
        {
        }

        // Constructor con parámetros
        public Horario(string idHorario, string tipoInscripcion, string codEstudiante, string codAcademico, string idCurso)
        {
            IdHorario = idHorario;
            TipoInscripcion = tipoInscripcion;
            CodEstudiante = codEstudiante;
            CodAcademico = codAcademico;
            IdCurso = idCurso;
        }
    }
}