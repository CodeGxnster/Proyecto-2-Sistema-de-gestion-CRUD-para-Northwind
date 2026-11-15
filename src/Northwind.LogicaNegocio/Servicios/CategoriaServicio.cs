using FluentValidation;
using Northwind.AccesoDatos.Repositorios;
using Northwind.Entidades;
using Serilog;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Reglas de negocio para la administracion de categorias de Northwind.
/// </summary>
public class CategoriaServicio : ICategoriaServicio
{
    private readonly ICategoriaRepositorio _categoriaRepositorio;
    private readonly IValidator<CategoryDTO> _validador;
    private readonly ILogger _logger;

    public CategoriaServicio(
        ICategoriaRepositorio categoriaRepositorio,
        IValidator<CategoryDTO> validador,
        ILogger logger)
    {
        _categoriaRepositorio = categoriaRepositorio;
        _validador = validador;
        _logger = logger;
    }

    public IReadOnlyList<CategoryDTO> Listar(string? termino = null)
    {
        try
        {
            var categorias = _categoriaRepositorio.ObtenerTodas(termino);
            return categorias.Select(MapearDto).ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al listar categorias con el termino {Termino}", termino);
            return Array.Empty<CategoryDTO>();
        }
    }

    public CategoryDTO? ObtenerPorId(int id)
    {
        try
        {
            var categoria = _categoriaRepositorio.ObtenerPorId(id);
            return categoria == null ? null : MapearDto(categoria);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener la categoria {CategoriaId}", id);
            return null;
        }
    }

    public ResultadoOperacion<int> Crear(CategoryDTO categoria)
    {
        categoria.Nombre = categoria.Nombre.Trim();
        categoria.Descripcion = string.IsNullOrWhiteSpace(categoria.Descripcion)
            ? null
            : categoria.Descripcion.Trim();

        var validacion = _validador.Validate(categoria);
        if (!validacion.IsValid)
        {
            return ResultadoOperacion<int>.Falla(
                "Existen errores de validacion en la categoria.",
                validacion.Errors.Select(e => e.ErrorMessage));
        }

        if (_categoriaRepositorio.ExisteNombre(categoria.Nombre))
        {
            return ResultadoOperacion<int>.Falla(
                "Ya existe una categoria registrada con ese nombre.",
                new[] { "El nombre de la categoria debe ser unico." });
        }

        try
        {
            var entidad = new Categoria
            {
                Nombre = categoria.Nombre,
                Descripcion = categoria.Descripcion
            };

            var creada = _categoriaRepositorio.Agregar(entidad);
            _logger.Information(
                "Categoria creada con ID {CategoriaId} y nombre {Nombre}",
                creada.CategoriaId,
                creada.Nombre);

            return ResultadoOperacion<int>.ExitoConDatos(creada.CategoriaId, "Categoria creada correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear la categoria {Nombre}", categoria.Nombre);
            return ResultadoOperacion<int>.Falla("No fue posible guardar la categoria. Consulte el registro de errores.");
        }
    }

    public ResultadoOperacion Actualizar(CategoryDTO categoria)
    {
        if (categoria.CategoriaId <= 0)
        {
            return ResultadoOperacion.Falla("El identificador de la categoria no es valido.");
        }

        categoria.Nombre = categoria.Nombre.Trim();
        categoria.Descripcion = string.IsNullOrWhiteSpace(categoria.Descripcion)
            ? null
            : categoria.Descripcion.Trim();

        var validacion = _validador.Validate(categoria);
        if (!validacion.IsValid)
        {
            return ResultadoOperacion.Falla(
                "Existen errores de validacion en la categoria.",
                validacion.Errors.Select(e => e.ErrorMessage));
        }

        var existente = _categoriaRepositorio.ObtenerPorId(categoria.CategoriaId);
        if (existente == null)
        {
            return ResultadoOperacion.Falla("La categoria indicada no existe.");
        }

        if (_categoriaRepositorio.ExisteNombre(categoria.Nombre, categoria.CategoriaId))
        {
            return ResultadoOperacion.Falla(
                "Ya existe otra categoria registrada con ese nombre.",
                new[] { "El nombre de la categoria debe ser unico." });
        }

        try
        {
            existente.Nombre = categoria.Nombre;
            existente.Descripcion = categoria.Descripcion;
            _categoriaRepositorio.Actualizar(existente);

            _logger.Information(
                "Categoria actualizada con ID {CategoriaId} y nombre {Nombre}",
                existente.CategoriaId,
                existente.Nombre);

            return ResultadoOperacion.Exito("Categoria actualizada correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al actualizar la categoria {CategoriaId}", categoria.CategoriaId);
            return ResultadoOperacion.Falla("No fue posible actualizar la categoria. Consulte el registro de errores.");
        }
    }

    public ResultadoOperacion Eliminar(int id)
    {
        var existente = _categoriaRepositorio.ObtenerPorId(id);
        if (existente == null)
        {
            return ResultadoOperacion.Falla("La categoria indicada no existe.");
        }

        var productosVinculados = _categoriaRepositorio.ContarProductos(id);
        if (productosVinculados > 0)
        {
            return ResultadoOperacion.Falla(
                $"No se puede eliminar la categoria porque tiene {productosVinculados} producto(s) vinculado(s).");
        }

        try
        {
            _categoriaRepositorio.Eliminar(existente);
            _logger.Information("Categoria eliminada con ID {CategoriaId} y nombre {Nombre}", id, existente.Nombre);
            return ResultadoOperacion.Exito("Categoria eliminada correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar la categoria {CategoriaId}", id);
            return ResultadoOperacion.Falla("No fue posible eliminar la categoria. Consulte el registro de errores.");
        }
    }

    public int Contar()
    {
        try
        {
            return _categoriaRepositorio.Contar();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al contar las categorias registradas");
            return 0;
        }
    }

    private CategoryDTO MapearDto(Categoria categoria)
    {
        return new CategoryDTO
        {
            CategoriaId = categoria.CategoriaId,
            Nombre = categoria.Nombre,
            Descripcion = categoria.Descripcion,
            CantidadProductos = _categoriaRepositorio.ContarProductos(categoria.CategoriaId)
        };
    }
}
