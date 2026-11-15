using System.Data;
using Microsoft.EntityFrameworkCore;
using Northwind.AccesoDatos.Contexto;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Implementacion EF Core del repositorio de productos.
/// </summary>
public class ProductoRepositorio : IProductoRepositorio
{
    private readonly NorthwindDbContext _contexto;

    public ProductoRepositorio(NorthwindDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<Producto> ObtenerTodos()
    {
        return _contexto.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Suplidor)
            .OrderBy(p => p.Nombre)
            .ToList();
    }

    public Producto? ObtenerPorId(int id)
    {
        return _contexto.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Suplidor)
            .FirstOrDefault(p => p.ProductoId == id);
    }

    public bool TienePedidos(int productoId)
    {
        return EjecutarEscalar(
            "SELECT COUNT(*) FROM [Order Details] WHERE ProductID = @productoId",
            ("@productoId", productoId)) > 0;
    }

    public Producto Agregar(Producto producto)
    {
        _contexto.Productos.Add(producto);
        _contexto.SaveChanges();
        var id = producto.ProductoId;
        _contexto.ChangeTracker.Clear();

        return _contexto.Productos
            .AsNoTracking()
            .Include(p => p.Categoria)
            .Include(p => p.Suplidor)
            .First(p => p.ProductoId == id);
    }

    public void Actualizar(Producto producto)
    {
        producto.Categoria = null;
        producto.Suplidor = null;

        _contexto.Productos.Update(producto);
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    public void Eliminar(Producto producto)
    {
        producto.Categoria = null;
        producto.Suplidor = null;

        _contexto.Productos.Remove(producto);
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    private int EjecutarEscalar(string sentencia, params (string Nombre, object Valor)[] parametros)
    {
        var conexion = _contexto.Database.GetDbConnection();
        var debeCerrar = conexion.State != ConnectionState.Open;

        if (debeCerrar)
        {
            conexion.Open();
        }

        try
        {
            using var comando = conexion.CreateCommand();
            comando.CommandText = sentencia;

            foreach (var (nombre, valor) in parametros)
            {
                var parametro = comando.CreateParameter();
                parametro.ParameterName = nombre;
                parametro.Value = valor;
                comando.Parameters.Add(parametro);
            }

            var resultado = comando.ExecuteScalar();
            return resultado == null || resultado == DBNull.Value
                ? 0
                : Convert.ToInt32(resultado);
        }
        finally
        {
            if (debeCerrar)
            {
                conexion.Close();
            }
        }
    }
}
