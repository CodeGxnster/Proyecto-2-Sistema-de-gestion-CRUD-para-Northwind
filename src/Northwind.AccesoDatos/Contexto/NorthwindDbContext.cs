using Microsoft.EntityFrameworkCore;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Contexto;

/// <summary>
/// Contexto de Entity Framework Core para la base de datos Northwind.
/// </summary>
public class NorthwindDbContext : DbContext
{
    public NorthwindDbContext(DbContextOptions<NorthwindDbContext> options)
        : base(options)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Producto> Productos => Set<Producto>();

    public DbSet<Suplidor> Suplidores => Set<Suplidor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(NorthwindDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
