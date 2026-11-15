using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Configuraciones;

/// <summary>
/// Mapeo Fluent API de la entidad Categoria contra la tabla Categories.
/// </summary>
public sealed class CategoriaConfiguracion : IEntityTypeConfiguration<Categoria>
{
    public void Configure(EntityTypeBuilder<Categoria> builder)
    {
        builder.ToTable("Categories");

        builder.HasKey(c => c.CategoriaId);

        builder.Property(c => c.CategoriaId)
            .HasColumnName("CategoryID")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.Nombre)
            .HasColumnName("CategoryName")
            .HasMaxLength(15)
            .IsRequired();

        builder.Property(c => c.Descripcion)
            .HasColumnName("Description")
            .HasColumnType("ntext");
    }
}
