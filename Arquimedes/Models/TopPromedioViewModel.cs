using System;

namespace Arquimedes.Models
{
    public class TopPromedioViewModel
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Grado { get; set; }
        public int Anio { get; set; }
        public int IdCurso { get; set; }
        public int Bimestre { get; set; }
        public decimal PromedioBimestre { get; set; }
        public decimal PromedioGeneral { get; set; }
        public string PromedioLetra { get; set; }

        public TopPromedioViewModel() { }

        public TopPromedioViewModel(string codigo, string nombre, string apellido, string grado, int anio,
                                    int idCurso, int bimestre, decimal promedioBimestre, decimal promedioGeneral, string promedioLetra)
        {
            Codigo = codigo;
            Nombre = nombre;
            Apellido = apellido;
            Grado = grado;
            Anio = anio;
            IdCurso = idCurso;
            Bimestre = bimestre;
            PromedioBimestre = promedioBimestre;
            PromedioGeneral = promedioGeneral;
            PromedioLetra = promedioLetra;
        }
    }
}
