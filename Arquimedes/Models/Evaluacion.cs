using System;

namespace Arquimedes.Models
{
    public class Evaluacion
    {
        // Identificador de evaluación
        public string IdEvaluacion { get; set; }

        // Información del estudiante
        public string CodEstudiante { get; set; }
        public string NombreEstudiante { get; set; } // Nombre completo del estudiante
        public int? AnioEstudiante { get; set; } // Si puede ser nulo
        public string SeccionEstudiante { get; set; } // Sección del estudiante
        public string GradoAcademico { get; set; } // Grado académico del estudiante

        // Información del académico
        public string CodAcademico { get; set; }
        public string NombreAcademico { get; set; } // Nombre completo del académico

        // Información del curso
        public string IdCurso { get; set; }

        // Detalles de la evaluación
        public int Bimestre { get; set; } // Bimestre evaluado
        public string T1 { get; set; }    // Nota Trimestre 1 ('AD', 'A', 'B', 'C')
        public string T2 { get; set; }    // Nota Trimestre 2 ('AD', 'A', 'B', 'C')
        public string T3 { get; set; }    // Nota Trimestre 3 ('AD', 'A', 'B', 'C')
        public string T4 { get; set; }    // Nota Trimestre 4 ('AD', 'A', 'B', 'C')
        public string T5 { get; set; }    // Nota adicional ('AD', 'A', 'B', 'C')
        public string PromedioLetra { get; set; }  // Promedio final en letra ('AD', 'A', 'B', 'C')

        // Constructor vacío
        public Evaluacion() { }

        // Constructor con parámetros
        public Evaluacion(string idEvaluacion, string codEstudiante, string nombreEstudiante,
                          int anioEstudiante, string seccionEstudiante, string gradoAcademico,
                          string codAcademico, string nombreAcademico, string idCurso,
                          int bimestre, string t1, string t2, string t3, string t4, string t5, string promedioLetra)
        {
            IdEvaluacion = idEvaluacion;
            CodEstudiante = codEstudiante;
            NombreEstudiante = nombreEstudiante;
            AnioEstudiante = anioEstudiante;
            SeccionEstudiante = seccionEstudiante;
            GradoAcademico = gradoAcademico;
            CodAcademico = codAcademico;
            NombreAcademico = nombreAcademico;
            IdCurso = idCurso;
            Bimestre = bimestre;
            T1 = t1;
            T2 = t2;
            T3 = t3;
            T4 = t4;
            T5 = t5;
            PromedioLetra = promedioLetra;
        }

       
    }
}
