namespace portal_academico.Models;

public class HomeIndexViewModel
{
    public int TotalCursos { get; set; }
    public int TotalCupos { get; set; }
    public List<Curso> CursosRecientes { get; set; } = new();
    public bool EsCoordinador { get; set; }
}
