using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Xml.Linq;

public class Estudiante
{
    [Key]
    [Display(Name = "Código Estudiante")]
    public string CodEstudiante { get; set; } // Cambiar a string para permitir el formato "EST-0001"

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El campo Nombre Estudiante es obligatorio")]
    public string NombreEstudiante { get; set; }

    [Display(Name = "Apellido")]
    [Required(ErrorMessage = "El campo Apellido Estudiante es obligatorio")]
    public string ApellidoEstudiante { get; set; }

   

    [Display(Name = "Edad")]
    [Required(ErrorMessage = "El campo Edad Estudiante es obligatorio")]
    public int EdadEstudiante { get; set; }

    [Display(Name = "Grado Escolar")]
    [Required(ErrorMessage = "El campo Año Estudiante es obligatorio")]
    public int AnioEstudiante { get; set; }
    
    [Display(Name = "Sección")]
    [Required(ErrorMessage = "El campo Sección Estudiante es obligatorio")]
    public string SeccionEstudiante { get; set; }

    [Display(Name = "Grado Academico")]
    [Required(ErrorMessage = "El campo Grado Academico es obligatorio")]
    public string GradoAcademico { get; set; }

    [Display(Name = "Teléfono")]
    [Required(ErrorMessage = "El campo Teléfono es obligatorio")]
    [MaxLength(9, ErrorMessage = "El teléfono debe tener 9 dígitos")]
    public string Telefono { get; set; }

    [Display(Name = "Dirección")]
    [Required(ErrorMessage = "El campo Dirección Estudiante es obligatorio")]
    [MaxLength(70, ErrorMessage = "La dirección no puede exceder los 70 caracteres")]
    public string Direccion { get; set; }

    [Display(Name = "Estado Estudiante")]
    [Required(ErrorMessage = "El campo Estado Estudiante es obligatorio")]
    [RegularExpression("Activo|Inactivo", ErrorMessage = "El estado debe ser 'Activo' o 'Inactivo'.")]
    public string EstadoEstudiante { get; set; }
    [Display(Name = "Dni")]
    [Required(ErrorMessage = "El campo Dni es obligatorio")]
    public string DniEstudiante { get; set; }

    // Constructor vacío
    public Estudiante() { }

    public Estudiante(string codEstudiante, string nombreEstudiante, string apellidoEstudiante, int edadEstudiante, int anioEstudiante, string seccionEstudiante, string telefono, string direccion, string gradoAcademico, string estadoEstudiante, string dniEstudiante)
    {
        CodEstudiante = codEstudiante;
        NombreEstudiante = nombreEstudiante;
        ApellidoEstudiante = apellidoEstudiante;
        EdadEstudiante = edadEstudiante;
        AnioEstudiante = anioEstudiante;
        SeccionEstudiante = seccionEstudiante;
        GradoAcademico = gradoAcademico;
        Telefono = telefono;
        Direccion = direccion;
        EstadoEstudiante = estadoEstudiante;
        DniEstudiante = dniEstudiante;
    }

}
