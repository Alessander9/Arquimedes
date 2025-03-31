using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Academico
    {
        // Propiedades
        public string CodAcademico { get; set; } // Código único del académico (Primary Key)
        public string NombreAcademico { get; set; } // Nombre del académico
        public string ApellidoAcademico { get; set; } // Apellido del académico
     
        public int EdadAcademico { get; set; } // Edad del académico
        public int PeriodoAcademico { get; set; } // Periodo académico
        public string TelefonoAcademico { get; set; } // Teléfono del académico
        public string DireccionAcademico { get; set; } // Dirección del académico
        public string EstadoAcademico { get; set; } // Estado del académico (Activo o Inactivo)
        public string DniAcademico { get; set; }

        // Constructor vacío
        public Academico() { }

        // Constructor con parámetros
        public Academico(string codAcademico, string nombreAcademico, string apellidoAcademico, int edadAcademico,
                         int periodoAcademico, string telefonoAcademico, string direccionAcademico, string estadoAcademico, string dniAcademico)
        {
            CodAcademico = codAcademico;
            NombreAcademico = nombreAcademico;
            ApellidoAcademico = apellidoAcademico;
            EdadAcademico = edadAcademico;
            PeriodoAcademico = periodoAcademico;
            TelefonoAcademico = telefonoAcademico;
            DireccionAcademico = direccionAcademico;
            EstadoAcademico = estadoAcademico;
            DniAcademico = dniAcademico;
        }
    }
}
