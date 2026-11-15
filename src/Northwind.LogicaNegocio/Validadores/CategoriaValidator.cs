using FluentValidation;
using Northwind.Entidades;

namespace Northwind.LogicaNegocio.Validadores;

/// <summary>
/// Reglas de validacion para el alta y edicion de categorias.
/// </summary>
public class CategoriaValidator : AbstractValidator<CategoryDTO>
{
    public CategoriaValidator()
    {
        RuleFor(c => c.Nombre)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("El nombre de la categoria es obligatorio.")
            .Must(nombre => nombre.Trim().Length >= 3)
                .WithMessage("El nombre de la categoria debe tener al menos 3 caracteres.")
            .Must(nombre => nombre.Trim().Length <= 15)
                .WithMessage("El nombre de la categoria no puede superar los 15 caracteres.");

        RuleFor(c => c.Descripcion)
            .MaximumLength(500)
            .WithMessage("La descripcion no puede superar los 500 caracteres.");
    }
}
