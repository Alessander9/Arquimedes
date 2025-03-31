using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Nota
    {
        public string IdEvaluacion { get; set; }
        public string NombreCurso { get; set; }
        public int Bimestre { get; set; }
        public string T1 { get; set; }
        public string T2 { get; set; }
        public string T3 { get; set; }
        public string T4 { get; set; }
        public string T5 { get; set; }
        public string PromedioLetra { get; set; }
        // Propiedades para almacenar los promedios de cada bimestre como enteros
        public int PromedioBimestre1 { get; set; }
        public int PromedioBimestre2 { get; set; }
        public int PromedioBimestre3 { get; set; }
        public int PromedioBimestre4 { get; set; }

        // Propiedad para almacenar el promedio general como entero
        public int PromedioGeneral { get; set; }

        // Constructor vacío
        public Nota()
        {
        }

        // Constructor con parámetros (modificado para usar int en lugar de float)
        public Nota(
            string idEvaluacion,
            string nombreCurso,
            int bimestre,
            string t1,
            string t2,
            string t3,
            string t4,
            string t5,
            string promedioLetra,
            int promedioBimestre1,
            int promedioBimestre2,
            int promedioBimestre3,
            int promedioBimestre4)
        {
            IdEvaluacion = idEvaluacion;
            NombreCurso = nombreCurso;
            Bimestre = bimestre;
            T1 = t1;
            T2 = t2;
            T3 = t3;
            T4 = t4;
            T5 = t5;
            PromedioLetra = promedioLetra;
            PromedioBimestre1 = promedioBimestre1;
            PromedioBimestre2 = promedioBimestre2;
            PromedioBimestre3 = promedioBimestre3;
            PromedioBimestre4 = promedioBimestre4;

            // Calcular el promedio general a partir de los promedios de los bimestres
            PromedioGeneral = (PromedioBimestre1 + PromedioBimestre2 + PromedioBimestre3 + PromedioBimestre4) / 4;
        }

        // Método para actualizar el promedio general en caso de cambios
        public void CalcularPromedioGeneral()
        {
            PromedioGeneral = (PromedioBimestre1 + PromedioBimestre2 + PromedioBimestre3 + PromedioBimestre4) / 4;
        }
    }   

}