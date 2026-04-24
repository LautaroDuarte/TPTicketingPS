using Domain.Common;
using TPTicketingPS.Domain.Enums;


namespace TPTicketingPS.Domain.Entities;

public class Reservation
{
    public Guid Id { get; private set; }

    public int UserId { get; private set; }
    public User? User { get; private set; }

    public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;

    public List<ReservationItem> Items { get; private set; } = new();
    public DateTime ReservedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    public decimal TotalAmount { get; private set; } //Se puede calcular a partir de los items, pero lo guardamos aquí para no sumar en cada GET

    // navegación


    public Reservation(int userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ReservedAt = DateTime.UtcNow;
        ExpiresAt = ReservedAt.AddMinutes(5);
    }

    private Reservation() { }

    public void AddItem(ReservationItem item)
    {
        Items.Add(item);
        TotalAmount += item.UnitPrice;
    }

    public void MarkAsPaid() => Status = ReservationStatus.Paid;
    public void Expire() => Status = ReservationStatus.Expired;

    public bool IsExpired(DateTime utcNow) =>
        Status == ReservationStatus.Pending && utcNow >= ExpiresAt;
}