using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Configuraciones;

/// <summary>
/// Mapeo Fluent API de la entidad Suplidor contra la tabla Suppliers.
/// </summary>
public sealed class SuplidorConfiguracion : IEntityTypeConfiguration<Suplidor>
{
    public void Configure(EntityTypeBuilder<Suplidor> builder)
    {
        builder.ToTable("Suppliers");

        builder.HasKey(s => s.SuplidorId);

        builder.Property(s => s.SuplidorId)
            .HasColumnName("SupplierID")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.NombreEmpresa)
            .HasColumnName("CompanyName")
            .HasMaxLength(40)
            .IsRequired();

        builder.Property(s => s.NombreContacto)
            .HasColumnName("ContactName")
            .HasMaxLength(30);

        builder.Property(s => s.Pais)
            .HasColumnName("Country")
            .HasMaxLength(15);
    }
}
