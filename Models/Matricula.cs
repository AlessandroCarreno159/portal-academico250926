using System.ComponentModel.DataAnnotations;

namespace portal_academico.Models;

public class Matricula
{
    public int Id { get; set; }

    [Required]
    public int CursoId { get; set; }

    public Curso? Curso { get; set; }

    [Required]
    public string UsuarioId { get; set; } = string.Empty;

    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    public EstadoMatricula Estado { get; set; } = EstadoMatricula.Pendiente;
}
