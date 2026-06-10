using FluentValidation;
using TPTicketingPS.Application.Events.Dtos;

namespace TPTicketingPS.Application.Events;

public class CreateEventValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(200);

        RuleFor(x => x.EventDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("La fecha del evento debe ser futura.");

        RuleFor(x => x.Venue)
            .NotEmpty().WithMessage("El recinto es obligatorio.")
            .MaximumLength(200);

        RuleFor(x => x.MaxReservationsPerUser)
            .InclusiveBetween(1, 20)
            .When(x => x.MaxReservationsPerUser.HasValue)
            .WithMessage("El máximo de reservas por usuario debe estar entre 1 y 20.");

        RuleFor(x => x.Sectors)
            .NotEmpty().WithMessage("El evento debe tener al menos un sector.");

        RuleForEach(x => x.Sectors).ChildRules(sector =>
        {
            sector.RuleFor(s => s.Name)
                .NotEmpty().WithMessage("El nombre del sector es obligatorio.")
                .MaximumLength(100);

            sector.RuleFor(s => s.Price)
                .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

            sector.RuleFor(s => s.Rows)
                .InclusiveBetween(1, 30)
                .WithMessage("Las filas deben estar entre 1 y 30.");

            sector.RuleFor(s => s.SeatsPerRow)
                .InclusiveBetween(1, 50)
                .WithMessage("Los asientos por fila deben estar entre 1 y 50.");
        });
    }
}