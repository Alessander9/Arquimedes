using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Asistente
    {
        [Key]
        [Required(ErrorMessage = "El ID del asistente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El ID del asistente no puede exceder los 100 caracteres.")]
        public string IdAsistente { get; set; }

        [Required(ErrorMessage = "El nombre del asistente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre del asistente no puede exceder los 50 caracteres.")]
        public string NombreAsistente { get; set; }

        [Required(ErrorMessage = "El apellido del asistente es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido del asistente no puede exceder los 50 caracteres.")]
        public string ApellidoAsistente { get; set; }

        [Required(ErrorMessage = "El teléfono del asistente es obligatorio.")]
        [StringLength(15, ErrorMessage = "El teléfono del asistente no puede exceder los 15 caracteres.")]
        public string TelefonoAsistente { get; set; }

        [Required(ErrorMessage = "La dirección del asistente es obligatoria.")]
        [StringLength(70, ErrorMessage = "La dirección del asistente no puede exceder los 70 caracteres.")]
        public string DireccionAsistente { get; set; }

        // Constructor vacío (builder void)
        public Asistente() { }

        // Constructor con campos (builder con fields)
        public Asistente(string idAsistente, string nombreAsistente, string apellidoAsistente, string telefonoAsistente, string direccionAsistente)
        {
            IdAsistente = idAsistente;
            NombreAsistente = nombreAsistente;
            ApellidoAsistente = apellidoAsistente;
            TelefonoAsistente = telefonoAsistente;
            DireccionAsistente = direccionAsistente;
        }
    }
}