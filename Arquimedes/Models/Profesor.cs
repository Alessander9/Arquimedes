using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Profesor
    {
        [Key]
        [Display(Name = "Id Profesor")]
        public string IdProfesor { get; set; }
        public string NombreProfesor { get; set; }
        public string ApellidoProfesor { get; set; }
        public int EdadProfesor { get; set; }
        public string DniProfesor { get; set; }
        public string TelefonoProfesor { get; set; }
        public string DireccionProfesor { get; set; }
        public string EstadoProfesor { get; set; }


        public Profesor() { }

        // Constructor para inicializar la clase
        public Profesor(string idProfesor, string nombreProfesor, string apellidoProfesor, int edadProfesor,
                        string dniProfesor, string telefonoProfesor, string direccionProfesor, string estadoProfesor)
        {
            IdProfesor = idProfesor;
            NombreProfesor = nombreProfesor;
            ApellidoProfesor = apellidoProfesor;
            EdadProfesor = edadProfesor;
            DniProfesor = dniProfesor;
            TelefonoProfesor = telefonoProfesor;
            DireccionProfesor = direccionProfesor;
            EstadoProfesor = estadoProfesor;
        }

     
    }
}