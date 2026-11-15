using Microsoft.EntityFrameworkCore;
using Northwind.AccesoDatos.Contexto;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Implementacion de las consultas analiticas con LINQ sobre EF Core.
/// </summary>
public class ReporteRepositorio : IReporteRepositorio
{
    private readonly NorthwindDbContext _contexto;

    public ReporteRepositorio(NorthwindDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<ReporteTopProductoDTO> ObtenerTopProductosVendidos(int top)
    {
        var filas = (
            from detalle in _contexto.DetallesPedido.AsNoTracking()
            join producto in _contexto.Productos.AsNoTracking()
                on detalle.ProductoId equals producto.ProductoId
            join categoria in _contexto.Categorias.AsNoTracking()
                on producto.CategoriaId equals categoria.CategoriaId into categorias
            from categoria in categorias.DefaultIfEmpty()
            select new
            {
                producto.ProductoId,
                producto.Nombre,
                Categoria = categoria == null ? "Sin categoria" : categoria.Nombre,
                detalle.Cantidad,
                detalle.PrecioUnitario,
                detalle.Descuento
            }).ToList();

        return filas
            .GroupBy(f => new { f.ProductoId, f.Nombre, f.Categoria })
            .Select(grupo => new ReporteTopProductoDTO
            {
                ProductoId = grupo.Key.ProductoId,
                NombreProducto = grupo.Key.Nombre,
                NombreCategoria = grupo.Key.Categoria,
                UnidadesVendidas = grupo.Sum(f => (int)f.Cantidad),
                TotalVendido = grupo.Sum(f => f.Cantidad * f.PrecioUnitario * (1m - (decimal)f.Descuento))
            })
            .OrderByDescending(r => r.UnidadesVendidas)
            .ThenByDescending(r => r.TotalVendido)
            .Take(top)
            .ToList();
    }

    public IReadOnlyList<ReporteVentasCategoriaDTO> ObtenerVentasPorCategoria(DateTime desde, DateTime hasta)
    {
        var fechaDesde = desde.Date;
        var fechaHastaExclusiva = hasta.Date.AddDays(1);

        var filas = (
            from pedido in _contexto.Pedidos.AsNoTracking()
            join detalle in _contexto.DetallesPedido.AsNoTracking()
                on pedido.PedidoId equals detalle.PedidoId
            join producto in _contexto.Productos.AsNoTracking()
                on detalle.ProductoId equals producto.ProductoId
            join categoria in _contexto.Categorias.AsNoTracking()
                on producto.CategoriaId equals categoria.CategoriaId
            where pedido.FechaPedido >= fechaDesde && pedido.FechaPedido < fechaHastaExclusiva
            select new
            {
                Categoria = categoria.Nombre,
                detalle.Cantidad,
                detalle.PrecioUnitario,
                detalle.Descuento
            }).ToList();

        var totalGeneral = filas.Sum(f => f.Cantidad * f.PrecioUnitario * (1m - (decimal)f.Descuento));

        return filas
            .GroupBy(f => f.Categoria)
            .Select(grupo =>
            {
                var totalCategoria = grupo.Sum(f => f.Cantidad * f.PrecioUnitario * (1m - (decimal)f.Descuento));
                return new ReporteVentasCategoriaDTO
                {
                    Categoria = grupo.Key,
                    UnidadesVendidas = grupo.Sum(f => (int)f.Cantidad),
                    TotalVentas = totalCategoria,
                    Porcentaje = totalGeneral == 0m ? 0m : totalCategoria / totalGeneral
                };
            })
            .OrderByDescending(r => r.TotalVentas)
            .ToList();
    }

    public IReadOnlyList<ReporteInventarioSuplidorDTO> ObtenerSuplidoresMayorValorInventario(int top)
    {
        var filas = (
            from producto in _contexto.Productos.AsNoTracking()
            join suplidor in _contexto.Suplidores.AsNoTracking()
                on producto.SuplidorId equals suplidor.SuplidorId
            where !producto.Descontinuado
            select new
            {
                Suplidor = suplidor.NombreEmpresa,
                producto.UnidadesEnStock,
                producto.PrecioUnitario
            }).ToList();

        return filas
            .GroupBy(f => f.Suplidor)
            .Select(grupo => new ReporteInventarioSuplidorDTO
            {
                Suplidor = grupo.Key,
                CantidadProductos = grupo.Count(),
                UnidadesEnInventario = grupo.Sum(f => (int)(f.UnidadesEnStock ?? 0)),
                ValorInventario = grupo.Sum(f => (f.PrecioUnitario ?? 0m) * (f.UnidadesEnStock ?? 0))
            })
            .OrderByDescending(r => r.ValorInventario)
            .Take(top)
            .ToList();
    }
}
