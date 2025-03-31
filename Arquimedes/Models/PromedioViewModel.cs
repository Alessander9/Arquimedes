using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class PromedioViewModel
    {
        // Propiedades
        public string CodEstudiante_CodAcademico { get; set; }
        public string NombreEstudiante_Academico { get; set; }
        public string AnioEscolar { get; set; }
        public string GradoAcademico { get; set; }
        public string Curso { get; set; }
        public string PrimerBimestre { get; set; }
        public string SegundoBimestre { get; set; }
        public string TercerBimestre { get; set; }
        public string CuartoBimestre { get; set; }
        public string PromedioFinal { get; set; }

        // Constructor vacío
        public PromedioViewModel()
        {
        }

        // Constructor con parámetros
        public PromedioViewModel(
            string codEstudiante_CodAcademico,
            string nombreEstudiante_Academico,
            string anioEscolar,
            string gradoAcademico,
            string curso,
            string primerBimestre,
            string segundoBimestre,
            string tercerBimestre,
            string cuartoBimestre,
            string promedioFinal)
        {
            CodEstudiante_CodAcademico = codEstudiante_CodAcademico;
            NombreEstudiante_Academico = nombreEstudiante_Academico;
            AnioEscolar = anioEscolar;
            GradoAcademico = gradoAcademico;
            Curso = curso;
            PrimerBimestre = primerBimestre;
            SegundoBimestre = segundoBimestre;
            TercerBimestre = tercerBimestre;
            CuartoBimestre = cuartoBimestre;
            PromedioFinal = promedioFinal;
        }
    }

}