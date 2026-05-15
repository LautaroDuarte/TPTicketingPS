using FluentValidation;

namespace TPTicketingPS.Application.Payments.UseCases.ProcessPayment;

public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentRequest>
{
    private static readonly string[] AllowedMethods =
        new[] { "creditCard", "debitCard", "mercadoPago", "transfer" };

    public ProcessPaymentValidator()
    {
        RuleFor(x => x.PaymentMethod)
            .NotEmpty().WithMessage("El método de pago es obligatorio.")
            .Must(m => AllowedMethods.Contains(m))
            .WithMessage($"Método inválido. Valores permitidos: {string.Join(", ", AllowedMethods)}.");

        RuleFor(x => x.CardHolder)
            .NotEmpty().WithMessage("El titular es obligatorio.")
            .MaximumLength(100);

        // El número se manda con o sin espacios; lo normalizamos para validar
        RuleFor(x => x.CardNumber)
            .NotEmpty().WithMessage("El número de tarjeta es obligatorio.")
            .Must(n => OnlyDigits(n).Length is 15 or 16)
            .WithMessage("El número de tarjeta debe tener 15 (Amex) o 16 dígitos.");

        RuleFor(x => x.ExpirationDate)
            .NotEmpty().WithMessage("La fecha de vencimiento es obligatoria.")
            .Matches(@"^(0[1-9]|1[0-2])\/\d{2}$")
            .WithMessage("Formato inválido. Debe ser MM/AA.");

        RuleFor(x => x.Cvv)
            .NotEmpty().WithMessage("El CVV es obligatorio.")
            .Matches(@"^\d{3,4}$")
            .WithMessage("El CVV debe tener 3 o 4 dígitos.");
    }

    private static string OnlyDigits(string s) =>
        string.IsNullOrEmpty(s) ? string.Empty : new string(s.Where(char.IsDigit).ToArray());
}