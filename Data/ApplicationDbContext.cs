using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using portal_academico.Models;

namespace portal_academico.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    public DbSet<Curso> Cursos => Set<Curso>();
    public DbSet<Matricula> Matriculas => Set<Matricula>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Curso>(e =>
        {
            e.HasIndex(c => c.Codigo).IsUnique();
            e.Property(c => c.Codigo).IsRequired().HasMaxLength(20);
            e.Property(c => c.Nombre).IsRequired().HasMaxLength(100);
            e.ToTable(t =>
            {
                t.HasCheckConstraint("CK_Curso_Creditos", "\"Creditos\" > 0");
                t.HasCheckConstraint("CK_Curso_Horario", "\"HorarioInicio\" < \"HorarioFin\"");
            });
        });

        builder.Entity<Matricula>(e =>
        {
            // Un usuario no puede estar matriculado más de una vez en el mismo curso
            // (índice único total; la reapertura tras cancelación se maneja en código si se requiere).
            e.HasIndex(m => new { m.CursoId, m.UsuarioId }).IsUnique();
            e.Property(m => m.UsuarioId).IsRequired().HasMaxLength(450);
            e.HasOne(m => m.Curso)
                .WithMany(c => c.Matriculas)
                .HasForeignKey(m => m.CursoId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
