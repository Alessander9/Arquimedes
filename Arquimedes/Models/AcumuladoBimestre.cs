using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class AcumuladoBimestre
    {
        
            public int IdAcumulado { get; set; }
            public string CodEstudiante { get; set; }
            public string CodAcademico { get; set; }
            public string IdCurso { get; set; }
            public int Bimestre { get; set; }
            public string T1 { get; set; }
            public string T2 { get; set; }
            public string T3 { get; set; }
            public string T4 { get; set; }
            public string T5 { get; set; }
            public string PromedioLetra { get; set; }
        
        public AcumuladoBimestre() { }

        public AcumuladoBimestre(int idAcumulado, string codEstudiante, string codAcademico, string idCurso, int bimestre, string t1, string t2, string t3, string t4, string t5, string promedioLetra)
        {
            IdAcumulado = idAcumulado;
            CodEstudiante = codEstudiante;
            CodAcademico = codAcademico;
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