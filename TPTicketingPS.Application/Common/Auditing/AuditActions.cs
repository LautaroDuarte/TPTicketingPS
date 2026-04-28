namespace TPTicketingPS.Application.Common.Auditing;

/// <summary>
/// Catálogo de acciones que se registran en AuditLog.
/// Centralizamos los strings acá para evitar typos en use cases.
/// </summary>
public static class AuditActions
{
    // Reservas
    public const string ReserveAttempt = "RESERVE_ATTEMPT";
    public const string ReserveSuccess = "RESERVE_SUCCESS";
    public const string ReserveFailedConcurrency = "RESERVE_FAILED_CONCURRENCY";
    public const string ReserveFailedUnavailable = "RESERVE_FAILED_UNAVAILABLE";
    public const string ReserveCancelled = "RESERVE_CANCELLED";
    public const string ReserveExpired = "RESERVE_EXPIRED";

    // Pagos
    public const string PaymentAttempt = "PAYMENT_ATTEMPT";
    public const string PaymentSuccess = "PAYMENT_SUCCESS";
    public const string PaymentFailed = "PAYMENT_FAILED";
}

public static class AuditEntityTypes
{
    public const string Reservation = "Reservation";
    public const string Seat = "Seat";
    public const string Payment = "Payment";
    public const string User = "User";
}