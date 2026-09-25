using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using portal_academico.Data;
using portal_academico.Models;

namespace portal_academico.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _db;

    public HomeController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var cursos = await _db.Cursos
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .Take(3)
            .ToListAsync();

        var vm = new HomeIndexViewModel
        {
            TotalCursos = await _db.Cursos.CountAsync(c => c.Activo),
            TotalCupos = await _db.Cursos.Where(c => c.Activo).SumAsync(c => (int?)c.CupoMaximo) ?? 0,
            CursosRecientes = cursos,
            EsCoordinador = User.IsInRole("Coordinador")
        };
        return View(vm);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
