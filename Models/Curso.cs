using System.ComponentModel.DataAnnotations;

namespace portal_academico.Models;

public class Curso : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El código es obligatorio.")]
    [StringLength(20, ErrorMessage = "El código no puede superar 20 caracteres.")]
    public string Codigo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, ErrorMessage = "El nombre no puede superar 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Range(1, 20, ErrorMessage = "Los créditos deben ser mayores a 0.")]
    public int Creditos { get; set; }

    [Range(1, 500, ErrorMessage = "El cupo máximo debe ser mayor a 0.")]
    [Display(Name = "Cupo máximo")]
    public int CupoMaximo { get; set; }

    [Required(ErrorMessage = "El horario de inicio es obligatorio.")]
    [Display(Name = "Horario inicio")]
    public TimeOnly HorarioInicio { get; set; }

    [Required(ErrorMessage = "El horario de fin es obligatorio.")]
    [Display(Name = "Horario fin")]
    public TimeOnly HorarioFin { get; set; }

    public bool Activo { get; set; } = true;

    public ICollection<Matricula> Matriculas { get; set; } = new List<Matricula>();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (HorarioInicio >= HorarioFin)
        {
            yield return new ValidationResult(
                "El horario de inicio debe ser anterior al horario de fin.",
                new[] { nameof(HorarioInicio), nameof(HorarioFin) });
        }
    }
}
