using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Usuario
    {
        [Key]
        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El ID del usuario no puede exceder los 100 caracteres.")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(50, ErrorMessage = "El nombre de usuario no puede exceder los 50 caracteres.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(50, ErrorMessage = "La contraseña no puede exceder los 50 caracteres.")]
        public string Contrasenia { get; set; }

        [Required(ErrorMessage = "El rol del usuario es obligatorio.")]
        [StringLength(40, ErrorMessage = "El rol del usuario no puede exceder los 10 caracteres.")]
        public string RolUsuario { get; set; }

        [Required(ErrorMessage = "El ID relacionado es obligatorio.")]
        [StringLength(100, ErrorMessage = "El ID relacionado no puede exceder los 100 caracteres.")]
        public string IdRelacionado { get; set; }

        public Usuario() { }

        public Usuario(string idUsuario, string nombreUsuario, string contrasenia, string rolUsuario, string idRelacionado)
        {
            IdUsuario = idUsuario;
            NombreUsuario = nombreUsuario;
            Contrasenia = contrasenia;
            RolUsuario = rolUsuario;
            IdRelacionado = idRelacionado;
        }
    }
}