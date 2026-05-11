namespace TPTicketingPS.Application.Payments.UseCases.ProcessPayment;

public sealed record ProcessPaymentRequest(
    string PaymentMethod,
    string CardHolder,
    string CardNumber,
    string ExpirationDate,
    string Cvv);