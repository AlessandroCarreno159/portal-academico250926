using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using portal_academico.Data;
using portal_academico.Models;

namespace portal_academico.Controllers;

public class CatalogoController : Controller
{
    private readonly ApplicationDbContext _db;

    public CatalogoController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET /Catalogo?nombre=&minCred=&maxCred=&desde=&hasta=
    public async Task<IActionResult> Index(CatalogoFiltroViewModel filtros)
    {
        var query = _db.Cursos.Where(c => c.Activo).AsQueryable();

        var creditosValidos = true;
        if (filtros.MinCred.HasValue && filtros.MinCred.Value < 0)
        {
            ModelState.AddModelError(string.Empty, "Los créditos no pueden ser negativos.");
            creditosValidos = false;
        }
        if (filtros.MaxCred.HasValue && filtros.MaxCred.Value < 0)
        {
            ModelState.AddModelError(string.Empty, "Los créditos no pueden ser negativos.");
            creditosValidos = false;
        }
        if (creditosValidos)
        {
            if (filtros.MinCred.HasValue && filtros.MaxCred.HasValue
                && filtros.MinCred.Value > filtros.MaxCred.Value)
            {
                ModelState.AddModelError(string.Empty, "El crédito mínimo no puede ser mayor que el máximo.");
            }
            else
            {
                if (filtros.MinCred.HasValue)
                    query = query.Where(c => c.Creditos >= filtros.MinCred.Value);
                if (filtros.MaxCred.HasValue)
                    query = query.Where(c => c.Creditos <= filtros.MaxCred.Value);
            }
        }

        if (filtros.Desde.HasValue && filtros.Hasta.HasValue
            && filtros.Desde.Value >= filtros.Hasta.Value)
        {
            ModelState.AddModelError(string.Empty, "La hora de inicio del filtro debe ser anterior a la de fin.");
        }
        else
        {
            // Cursos cuyo horario se solapa con el rango buscado.
            if (filtros.Desde.HasValue && filtros.Hasta.HasValue)
                query = query.Where(c => c.HorarioInicio < filtros.Hasta.Value
                                      && filtros.Desde.Value < c.HorarioFin);
            else if (filtros.Desde.HasValue)
                query = query.Where(c => c.HorarioFin > filtros.Desde.Value);
            else if (filtros.Hasta.HasValue)
                query = query.Where(c => c.HorarioInicio < filtros.Hasta.Value);
        }

        var cursos = await query.OrderBy(c => c.Nombre).ToListAsync();

        if (!string.IsNullOrWhiteSpace(filtros.Nombre))
        {
            // Filtro en memoria: insensible a mayúsculas y tildes
            // (SQLite no ofrece búsqueda sin acentos en el servidor).
            var nombre = filtros.Nombre.Trim();
            cursos = cursos.Where(c => ContieneNormalizado(c.Nombre, nombre)
                                    || ContieneNormalizado(c.Codigo, nombre)).ToList();
        }

        var ids = cursos.Select(c => c.Id).ToList();
        var conteos = await _db.Matriculas
            .Where(m => ids.Contains(m.CursoId) && m.Estado != EstadoMatricula.Cancelada)
            .GroupBy(m => m.CursoId)
            .Select(g => new { CursoId = g.Key, Total = g.Count() })
            .ToDictionaryAsync(x => x.CursoId, x => x.Total);

        filtros.Resultados = cursos.Select(c => new CursoCatalogoItem
        {
            Curso = c,
            Inscritos = conteos.TryGetValue(c.Id, out var n) ? n : 0
        }).ToList();

        return View(filtros);
    }

    // GET /Catalogo/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var curso = await _db.Cursos.FirstOrDefaultAsync(c => c.Id == id && c.Activo);
        if (curso is null)
            return NotFound();

        var inscritos = await _db.Matriculas
            .CountAsync(m => m.CursoId == id && m.Estado != EstadoMatricula.Cancelada);

        // P4: aquí se guardará el último curso visitado en Session (Redis-backed).
        return View(new CursoCatalogoItem { Curso = curso, Inscritos = inscritos });
    }

    private static bool ContieneNormalizado(string texto, string busqueda)
    {
        return Normalizar(texto).Contains(Normalizar(busqueda), StringComparison.Ordinal);
    }

    private static string Normalizar(string valor)
    {
        var descompuesto = valor.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();
        foreach (var ch in descompuesto)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch)
                != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(ch);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }
}
