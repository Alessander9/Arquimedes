using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Arquimedes.Models
{
    public class Curso
    {
        [Key]
        [StringLength(100)]
        public string IdCurso { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreCurso { get; set; }

        [StringLength(255)]
        public string DescripcionCurso { get; set; }

        [Required]
        [StringLength(15)]
        public string Dia { get; set; }

        [Required]
        [StringLength(20)]
        public string Hora { get; set; }

        [Required]
        [StringLength(20)]
        public string Nivel { get; set; }

        [Required]
        [StringLength(100)]
        [ForeignKey("Profesor")]
        public string IdProfesor { get; set; }

        // Relación con el modelo Profesor
        public Profesor Profesor { get; set; }


        public Curso() { }

        public Curso(string idCurso, string nombreCurso, string descripcionCurso, string dia, string hora, string nivel, string idProfesor, Profesor profesor)
        {
            IdCurso = idCurso;
            NombreCurso = nombreCurso;
            DescripcionCurso = descripcionCurso;
            Dia = dia;
            Hora = hora;
            Nivel = nivel;
            IdProfesor = idProfesor;
            Profesor = profesor;
        }
    }

    

}