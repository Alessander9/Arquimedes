using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contrasenia { get; set; }

        [Required(ErrorMessage = "Debe seleccionar un rol.")]
        public string RolUsuario { get; set; }

        public LoginModel() { }

        public LoginModel(string nombreUsuario, string contrasenia, string rolUsuario)
        {
            NombreUsuario = nombreUsuario;
            Contrasenia = contrasenia;
            RolUsuario = rolUsuario;
        }
    }
}