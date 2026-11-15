using FluentValidation;
using Northwind.Entidades;

namespace Northwind.LogicaNegocio.Validadores;

/// <summary>
/// Reglas de validacion para el alta y edicion de productos.
/// </summary>
public class ProductoValidator : AbstractValidator<ProductDTO>
{
    public ProductoValidator()
    {
        RuleFor(p => p.Nombre)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El nombre del producto es obligatorio.")
            .MaximumLength(40).WithMessage("El nombre del producto no puede superar los 40 caracteres.");

        RuleFor(p => p.PrecioUnitario)
            .GreaterThan(0).WithMessage("El precio unitario debe ser mayor a 0.")
            .Must(precio => decimal.Round(precio, 2) == precio)
                .WithMessage("El precio unitario admite un maximo de 2 decimales.");

        RuleFor(p => p.CategoriaId)
            .NotNull().WithMessage("Debe seleccionar una categoria.")
            .GreaterThan(0).WithMessage("Debe seleccionar una categoria valida.");

        RuleFor(p => p.SuplidorId)
            .NotNull().WithMessage("Debe seleccionar un suplidor.")
            .GreaterThan(0).WithMessage("Debe seleccionar un suplidor valido.");

        RuleFor(p => p.CantidadPorUnidad)
            .MaximumLength(20).WithMessage("La cantidad por unidad no puede superar los 20 caracteres.");

        RuleFor(p => p.UnidadesEnStock)
            .GreaterThanOrEqualTo((short)0).WithMessage("Las unidades en stock no pueden ser negativas.");

        RuleFor(p => p.UnidadesEnPedido)
            .GreaterThanOrEqualTo((short)0).WithMessage("Las unidades en pedido no pueden ser negativas.");

        RuleFor(p => p.NivelReorden)
            .GreaterThanOrEqualTo((short)0).WithMessage("El nivel de reorden no puede ser negativo.");
    }
}
