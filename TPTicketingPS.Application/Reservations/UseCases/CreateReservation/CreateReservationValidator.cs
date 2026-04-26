using FluentValidation;

namespace TPTicketingPS.Application.Reservations.UseCases.CreateReservation;

public class CreateReservationValidator : AbstractValidator<CreateReservationRequest>
{
    public CreateReservationValidator()
    {
        RuleFor(x => x.EventId)
            .GreaterThan(0).WithMessage("El EventId debe ser mayor a 0.");

        RuleFor(x => x.SeatIds)
            .NotEmpty().WithMessage("Hay que seleccionar al menos un asiento.")
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("No se permiten asientos duplicados en la misma reserva.");
    }
}