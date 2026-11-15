using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Configuraciones;

/// <summary>
/// Mapeo Fluent API de la entidad Pedido contra la tabla Orders.
/// </summary>
public sealed class PedidoConfiguracion : IEntityTypeConfiguration<Pedido>
{
    public void Configure(EntityTypeBuilder<Pedido> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(p => p.PedidoId);

        builder.Property(p => p.PedidoId)
            .HasColumnName("OrderID")
            .ValueGeneratedOnAdd();

        builder.Property(p => p.FechaPedido)
            .HasColumnName("OrderDate");

        builder.Property(p => p.FechaRequerida)
            .HasColumnName("RequiredDate");

        builder.Property(p => p.FechaEnvio)
            .HasColumnName("ShippedDate");
    }
}
