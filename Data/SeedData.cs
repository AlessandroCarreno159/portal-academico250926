using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using portal_academico.Models;

namespace portal_academico.Data;

public static class SeedData
{
    public const string CoordinadorEmail = "coordinador@uni.edu";
    public const string CoordinadorPassword = "Coord123!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (!await roleManager.RoleExistsAsync("Coordinador"))
            await roleManager.CreateAsync(new IdentityRole("Coordinador"));

        var coord = await userManager.FindByEmailAsync(CoordinadorEmail);
        if (coord is null)
        {
            coord = new IdentityUser
            {
                UserName = CoordinadorEmail,
                Email = CoordinadorEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(coord, CoordinadorPassword);
        }
        if (!await userManager.IsInRoleAsync(coord, "Coordinador"))
            await userManager.AddToRoleAsync(coord, "Coordinador");

        if (!await db.Cursos.AnyAsync())
        {
            db.Cursos.AddRange(
                new Curso
                {
                    Codigo = "SIS101",
                    Nombre = "Programación I",
                    Creditos = 4,
                    CupoMaximo = 30,
                    HorarioInicio = new TimeOnly(8, 0),
                    HorarioFin = new TimeOnly(10, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "SIS102",
                    Nombre = "Base de Datos I",
                    Creditos = 4,
                    CupoMaximo = 25,
                    HorarioInicio = new TimeOnly(10, 0),
                    HorarioFin = new TimeOnly(12, 0),
                    Activo = true
                },
                new Curso
                {
                    Codigo = "SIS103",
                    Nombre = "Redes de Computadoras",
                    Creditos = 3,
                    CupoMaximo = 20,
                    HorarioInicio = new TimeOnly(14, 0),
                    HorarioFin = new TimeOnly(16, 0),
                    Activo = true
                });
            await db.SaveChangesAsync();
        }
    }
}
