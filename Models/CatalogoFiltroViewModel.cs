using portal_academico.Models;

namespace portal_academico.Models;

public class CursoCatalogoItem
{
    public Curso Curso { get; set; } = null!;
    public int Inscritos { get; set; }
    public int CuposDisponibles => Math.Max(0, Curso.CupoMaximo - Inscritos);
}

public class CatalogoFiltroViewModel
{
    public string? Nombre { get; set; }
    public int? MinCred { get; set; }
    public int? MaxCred { get; set; }
    public TimeOnly? Desde { get; set; }
    public TimeOnly? Hasta { get; set; }

    public List<CursoCatalogoItem> Resultados { get; set; } = new();

    public bool TieneFiltros =>
        !string.IsNullOrWhiteSpace(Nombre) || MinCred.HasValue || MaxCred.HasValue
        || Desde.HasValue || Hasta.HasValue;
}
