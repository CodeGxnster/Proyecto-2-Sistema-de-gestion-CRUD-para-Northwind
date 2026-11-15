using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Configuraciones;

/// <summary>
/// Mapeo Fluent API de la entidad DetallePedido contra la tabla Order Details.
/// </summary>
public sealed class DetallePedidoConfiguracion : IEntityTypeConfiguration<DetallePedido>
{
    public void Configure(EntityTypeBuilder<DetallePedido> builder)
    {
        builder.ToTable("Order Details");

        builder.HasKey(d => new { d.PedidoId, d.ProductoId });

        builder.Property(d => d.PedidoId)
            .HasColumnName("OrderID");

        builder.Property(d => d.ProductoId)
            .HasColumnName("ProductID");

        builder.Property(d => d.PrecioUnitario)
            .HasColumnName("UnitPrice")
            .HasColumnType("money");

        builder.Property(d => d.Cantidad)
            .HasColumnName("Quantity");

        builder.Property(d => d.Descuento)
            .HasColumnName("Discount");

        builder.HasOne(d => d.Pedido)
            .WithMany(p => p.Detalles)
            .HasForeignKey(d => d.PedidoId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Producto)
            .WithMany()
            .HasForeignKey(d => d.ProductoId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
