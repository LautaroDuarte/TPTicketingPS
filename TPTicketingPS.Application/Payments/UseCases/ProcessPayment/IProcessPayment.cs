using TPTicketingPS.Application.Payments.Dtos;

namespace TPTicketingPS.Application.Payments.UseCases.ProcessPayment;

public interface IProcessPayment
{
    Task<PaymentReceiptDto> ExecuteAsync(
        Guid reservationId,
        ProcessPaymentRequest request,
        CancellationToken cancellationToken = default);
}