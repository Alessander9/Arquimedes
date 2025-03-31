using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Administrador
    {
        [Key]
        [Required(ErrorMessage = "El ID del administrador es obligatorio.")]
        [StringLength(100, ErrorMessage = "El ID del administrador no puede exceder los 100 caracteres.")]
        public string IdAdministrador { get; set; }

        [Required(ErrorMessage = "El nombre del administrador es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder los 50 caracteres.")]
        public string NombreAdministrador { get; set; }

        [Required(ErrorMessage = "El apellido del administrador es obligatorio.")]
        [StringLength(50, ErrorMessage = "El apellido no puede exceder los 50 caracteres.")]
        public string ApellidoAdministrador { get; set; }

        [Required(ErrorMessage = "El teléfono del administrador es obligatorio.")]
        [StringLength(15, ErrorMessage = "El teléfono no puede exceder los 15 caracteres.")]
        public string TelefonoAdministrador { get; set; }

        [Required(ErrorMessage = "La dirección del administrador es obligatoria.")]
        [StringLength(70, ErrorMessage = "La dirección no puede exceder los 70 caracteres.")]
        public string DireccionAdministrador { get; set; }

        public Administrador() { }

        public Administrador(string idAdministrador, string nombreAdministrador, string apellidoAdministrador, string telefonoAdministrador, string direccionAdministrador)
        {
            IdAdministrador = idAdministrador;
            NombreAdministrador = nombreAdministrador;
            ApellidoAdministrador = apellidoAdministrador;
            TelefonoAdministrador = telefonoAdministrador;
            DireccionAdministrador = direccionAdministrador;
        }
    }
}