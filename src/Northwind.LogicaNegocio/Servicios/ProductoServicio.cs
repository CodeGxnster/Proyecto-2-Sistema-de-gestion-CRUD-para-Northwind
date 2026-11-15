using FluentValidation;
using Northwind.AccesoDatos.Repositorios;
using Northwind.Entidades;
using Serilog;

namespace Northwind.LogicaNegocio.Servicios;

/// <summary>
/// Reglas de negocio y filtros combinables para el catalogo de productos.
/// </summary>
public class ProductoServicio : IProductoServicio
{
    private readonly IProductoRepositorio _productoRepositorio;
    private readonly ICategoriaRepositorio _categoriaRepositorio;
    private readonly ISuplidorConsultaRepositorio _suplidorRepositorio;
    private readonly IValidator<ProductDTO> _validador;
    private readonly ILogger _logger;

    public ProductoServicio(
        IProductoRepositorio productoRepositorio,
        ICategoriaRepositorio categoriaRepositorio,
        ISuplidorConsultaRepositorio suplidorRepositorio,
        IValidator<ProductDTO> validador,
        ILogger logger)
    {
        _productoRepositorio = productoRepositorio;
        _categoriaRepositorio = categoriaRepositorio;
        _suplidorRepositorio = suplidorRepositorio;
        _validador = validador;
        _logger = logger;
    }

    public IReadOnlyList<ProductDTO> Listar(FiltroProductoDTO? filtro = null)
    {
        try
        {
            var consulta = _productoRepositorio.ObtenerTodos()
                .Select(MapearDto)
                .AsQueryable();

            if (filtro != null)
            {
                if (filtro.CategoriaId.HasValue && filtro.CategoriaId.Value > 0)
                {
                    consulta = consulta.Where(p => p.CategoriaId == filtro.CategoriaId.Value);
                }

                if (filtro.SuplidorId.HasValue && filtro.SuplidorId.Value > 0)
                {
                    consulta = consulta.Where(p => p.SuplidorId == filtro.SuplidorId.Value);
                }

                if (filtro.PrecioMinimo.HasValue && filtro.PrecioMinimo.Value > 0)
                {
                    consulta = consulta.Where(p => p.PrecioUnitario >= filtro.PrecioMinimo.Value);
                }

                if (filtro.PrecioMaximo.HasValue && filtro.PrecioMaximo.Value > 0)
                {
                    consulta = consulta.Where(p => p.PrecioUnitario <= filtro.PrecioMaximo.Value);
                }

                consulta = filtro.Estado switch
                {
                    EstadoProducto.Activos => consulta.Where(p => !p.Descontinuado),
                    EstadoProducto.Descontinuados => consulta.Where(p => p.Descontinuado),
                    _ => consulta
                };

                if (!string.IsNullOrWhiteSpace(filtro.Texto))
                {
                    var texto = filtro.Texto.Trim();
                    consulta = consulta.Where(p =>
                        p.Nombre.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        p.NombreCategoria.Contains(texto, StringComparison.OrdinalIgnoreCase) ||
                        p.NombreSuplidor.Contains(texto, StringComparison.OrdinalIgnoreCase));
                }
            }

            return consulta.ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al listar productos con el filtro {@Filtro}", filtro);
            return Array.Empty<ProductDTO>();
        }
    }

    public ProductDTO? ObtenerPorId(int id)
    {
        try
        {
            var producto = _productoRepositorio.ObtenerPorId(id);
            return producto == null ? null : MapearDto(producto);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener el producto {ProductoId}", id);
            return null;
        }
    }

    public IReadOnlyList<OpcionDTO> ObtenerCategorias()
    {
        try
        {
            return _categoriaRepositorio.ObtenerTodas()
                .Select(c => new OpcionDTO(c.CategoriaId, c.Nombre))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener las categorias para el filtro de productos");
            return Array.Empty<OpcionDTO>();
        }
    }

    public IReadOnlyList<OpcionDTO> ObtenerSuplidores()
    {
        try
        {
            return _suplidorRepositorio.ObtenerTodos()
                .Select(s => new OpcionDTO(s.SuplidorId, s.NombreEmpresa))
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al obtener los suplidores para el filtro de productos");
            return Array.Empty<OpcionDTO>();
        }
    }

    public ResultadoOperacion<int> Crear(ProductDTO producto)
    {
        Normalizar(producto);

        var validacion = _validador.Validate(producto);
        if (!validacion.IsValid)
        {
            return ResultadoOperacion<int>.Falla(
                "Existen errores de validacion en el producto.",
                validacion.Errors.Select(e => e.ErrorMessage));
        }

        var erroresReferencias = ValidarReferencias(producto);
        if (erroresReferencias.Count > 0)
        {
            return ResultadoOperacion<int>.Falla(
                "Existen errores de validacion en el producto.",
                erroresReferencias);
        }

        try
        {
            var entidad = new Producto
            {
                Nombre = producto.Nombre,
                CategoriaId = producto.CategoriaId,
                SuplidorId = producto.SuplidorId,
                CantidadPorUnidad = producto.CantidadPorUnidad,
                PrecioUnitario = producto.PrecioUnitario,
                UnidadesEnStock = producto.UnidadesEnStock,
                UnidadesEnPedido = producto.UnidadesEnPedido,
                NivelReorden = producto.NivelReorden,
                Descontinuado = producto.Descontinuado
            };

            var creado = _productoRepositorio.Agregar(entidad);
            _logger.Information(
                "Producto creado con ID {ProductoId}, nombre {Nombre}, categoria {CategoriaId} y suplidor {SuplidorId}",
                creado.ProductoId,
                creado.Nombre,
                creado.CategoriaId,
                creado.SuplidorId);

            return ResultadoOperacion<int>.ExitoConDatos(creado.ProductoId, "Producto creado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al crear el producto {Nombre}", producto.Nombre);
            return ResultadoOperacion<int>.Falla("No fue posible guardar el producto. Consulte el registro de errores.");
        }
    }

    public ResultadoOperacion Actualizar(ProductDTO producto)
    {
        if (producto.ProductoId <= 0)
        {
            return ResultadoOperacion.Falla("El identificador del producto no es valido.");
        }

        Normalizar(producto);

        var validacion = _validador.Validate(producto);
        if (!validacion.IsValid)
        {
            return ResultadoOperacion.Falla(
                "Existen errores de validacion en el producto.",
                validacion.Errors.Select(e => e.ErrorMessage));
        }

        var erroresReferencias = ValidarReferencias(producto);
        if (erroresReferencias.Count > 0)
        {
            return ResultadoOperacion.Falla(
                "Existen errores de validacion en el producto.",
                erroresReferencias);
        }

        var existente = _productoRepositorio.ObtenerPorId(producto.ProductoId);
        if (existente == null)
        {
            return ResultadoOperacion.Falla("El producto indicado no existe.");
        }

        try
        {
            existente.Nombre = producto.Nombre;
            existente.CategoriaId = producto.CategoriaId;
            existente.SuplidorId = producto.SuplidorId;
            existente.CantidadPorUnidad = producto.CantidadPorUnidad;
            existente.PrecioUnitario = producto.PrecioUnitario;
            existente.UnidadesEnStock = producto.UnidadesEnStock;
            existente.UnidadesEnPedido = producto.UnidadesEnPedido;
            existente.NivelReorden = producto.NivelReorden;
            existente.Descontinuado = producto.Descontinuado;

            _productoRepositorio.Actualizar(existente);
            _logger.Information(
                "Producto actualizado con ID {ProductoId} y nombre {Nombre}",
                existente.ProductoId,
                existente.Nombre);

            return ResultadoOperacion.Exito("Producto actualizado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al actualizar el producto {ProductoId}", producto.ProductoId);
            return ResultadoOperacion.Falla("No fue posible actualizar el producto. Consulte el registro de errores.");
        }
    }

    public ResultadoOperacion Eliminar(int id)
    {
        var existente = _productoRepositorio.ObtenerPorId(id);
        if (existente == null)
        {
            return ResultadoOperacion.Falla("El producto indicado no existe.");
        }

        if (_productoRepositorio.TienePedidos(id))
        {
            return ResultadoOperacion.Falla(
                "No se puede eliminar el producto porque tiene pedidos asociados. Puede marcarlo como descontinuado.");
        }

        try
        {
            _productoRepositorio.Eliminar(existente);
            _logger.Information("Producto eliminado con ID {ProductoId} y nombre {Nombre}", id, existente.Nombre);
            return ResultadoOperacion.Exito("Producto eliminado correctamente.");
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error al eliminar el producto {ProductoId}", id);
            return ResultadoOperacion.Falla("No fue posible eliminar el producto. Consulte el registro de errores.");
        }
    }

    private List<string> ValidarReferencias(ProductDTO producto)
    {
        var errores = new List<string>();

        if (producto.CategoriaId.HasValue)
        {
            var categoria = _categoriaRepositorio.ObtenerPorId(producto.CategoriaId.Value);
            if (categoria == null)
            {
                errores.Add("La categoria seleccionada no existe.");
            }
        }

        if (producto.SuplidorId.HasValue)
        {
            var suplidor = _suplidorRepositorio.ObtenerPorId(producto.SuplidorId.Value);
            if (suplidor == null)
            {
                errores.Add("El suplidor seleccionado no existe.");
            }
        }

        return errores;
    }

    private static void Normalizar(ProductDTO producto)
    {
        producto.Nombre = producto.Nombre.Trim();
        producto.CantidadPorUnidad = string.IsNullOrWhiteSpace(producto.CantidadPorUnidad)
            ? null
            : producto.CantidadPorUnidad.Trim();
    }

    private static ProductDTO MapearDto(Producto producto)
    {
        return new ProductDTO
        {
            ProductoId = producto.ProductoId,
            Nombre = producto.Nombre,
            CategoriaId = producto.CategoriaId,
            NombreCategoria = producto.Categoria?.Nombre ?? "Sin categoria",
            SuplidorId = producto.SuplidorId,
            NombreSuplidor = producto.Suplidor?.NombreEmpresa ?? "Sin suplidor",
            CantidadPorUnidad = producto.CantidadPorUnidad,
            PrecioUnitario = producto.PrecioUnitario ?? 0m,
            UnidadesEnStock = producto.UnidadesEnStock ?? 0,
            UnidadesEnPedido = producto.UnidadesEnPedido ?? 0,
            NivelReorden = producto.NivelReorden ?? 0,
            Descontinuado = producto.Descontinuado
        };
    }
}
