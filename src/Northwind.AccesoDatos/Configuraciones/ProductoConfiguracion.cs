using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Configuraciones;

/// <summary>
/// Mapeo Fluent API de la entidad Producto contra la tabla Products.
/// </summary>
public sealed class ProductoConfiguracion : IEntityTypeConfiguration<Producto>
{
    public void Configure(EntityTypeBuilder<Producto> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.ProductoId);

        builder.Property(p => p.ProductoId)
            .HasColumnName("ProductID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.Nombre)
            .HasColumnName("ProductName")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(p => p.SuplidorId)
            .HasColumnName("SupplierID");

        builder.Property(p => p.CategoriaId)
            .HasColumnName("CategoryID");

        builder.Property(p => p.CantidadPorUnidad)
            .HasColumnName("QuantityPerUnit")
            .HasMaxLength(20);

        builder.Property(p => p.PrecioUnitario)
            .HasColumnName("UnitPrice")
            .HasColumnType("money");

        builder.Property(p => p.UnidadesEnStock)
            .HasColumnName("UnitsInStock");

        builder.Property(p => p.UnidadesEnPedido)
            .HasColumnName("UnitsOnOrder");

        builder.Property(p => p.NivelReorden)
            .HasColumnName("ReorderLevel");

        builder.Property(p => p.Descontinuado)
            .HasColumnName("Discontinued");

        builder.HasOne(p => p.Categoria)
            .WithMany()
            .HasForeignKey(p => p.CategoriaId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Suplidor)
            .WithMany()
            .HasForeignKey(p => p.SuplidorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
