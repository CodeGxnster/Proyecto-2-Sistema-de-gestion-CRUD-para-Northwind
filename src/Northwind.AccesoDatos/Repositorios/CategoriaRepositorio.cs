using System.Data;
using Microsoft.EntityFrameworkCore;
using Northwind.AccesoDatos.Contexto;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Implementacion EF Core del repositorio de categorias.
/// </summary>
public class CategoriaRepositorio : ICategoriaRepositorio
{
    private readonly NorthwindDbContext _contexto;

    public CategoriaRepositorio(NorthwindDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<Categoria> ObtenerTodas(string? termino = null)
    {
        var consulta = _contexto.Categorias.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(termino))
        {
            var patron = $"%{termino.Trim()}%";
            consulta = consulta.Where(c =>
                EF.Functions.Like(c.Nombre, patron) ||
                (c.Descripcion != null && EF.Functions.Like(c.Descripcion, patron)));
        }

        return consulta
            .OrderBy(c => c.Nombre)
            .ToList();
    }

    public Categoria? ObtenerPorId(int id)
    {
        return _contexto.Categorias
            .AsNoTracking()
            .FirstOrDefault(c => c.CategoriaId == id);
    }

    public bool ExisteNombre(string nombre, int? excluirId = null)
    {
        var comparacion = nombre.Trim().ToLower();

        return _contexto.Categorias
            .AsNoTracking()
            .Any(c => c.Nombre.ToLower() == comparacion
                && (!excluirId.HasValue || c.CategoriaId != excluirId.Value));
    }

    public int ContarProductos(int categoriaId)
    {
        return EjecutarEscalar(
            "SELECT COUNT(*) FROM Products WHERE CategoryID = @categoriaId",
            ("@categoriaId", categoriaId));
    }

    public int Contar()
    {
        return _contexto.Categorias.AsNoTracking().Count();
    }

    public Categoria Agregar(Categoria categoria)
    {
        _contexto.Categorias.Add(categoria);
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
        return categoria;
    }

    public void Actualizar(Categoria categoria)
    {
        _contexto.Categorias.Update(categoria);
        _contexto.SaveChanges();
        _contexto.ChangeTracker.Clear();
    }

    public void Eliminar(Categoria categoria)
    {
        _contexto.Categorias.Remove(categoria);
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
