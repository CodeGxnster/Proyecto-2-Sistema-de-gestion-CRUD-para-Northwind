using Microsoft.EntityFrameworkCore;
using Northwind.AccesoDatos.Contexto;
using Northwind.Entidades;

namespace Northwind.AccesoDatos.Repositorios;

/// <summary>
/// Implementacion de solo lectura del catalogo de suplidores.
/// </summary>
public class SuplidorConsultaRepositorio : ISuplidorConsultaRepositorio
{
    private readonly NorthwindDbContext _contexto;

    public SuplidorConsultaRepositorio(NorthwindDbContext contexto)
    {
        _contexto = contexto;
    }

    public IReadOnlyList<Suplidor> ObtenerTodos()
    {
        return _contexto.Suplidores
            .AsNoTracking()
            .OrderBy(s => s.NombreEmpresa)
            .ToList();
    }

    public Suplidor? ObtenerPorId(int id)
    {
        return _contexto.Suplidores
            .AsNoTracking()
            .FirstOrDefault(s => s.SuplidorId == id);
    }
}
