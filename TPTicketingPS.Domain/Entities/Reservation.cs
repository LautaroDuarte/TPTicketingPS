using Domain.Common;
using Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Domain.Entities;

public class Reservation
{
    public Guid Id { get; private set; }

    public int UserId { get; private set; }

    public ReservationStatus Status { get; private set; } = ReservationStatus.Pending;

    public List<ReservationItem> Items { get; private set; } = new();
    public DateTime ReservedAt { get; private set; }

    public DateTime ExpiresAt { get; private set; }

    // navegación
    public User? User { get; private set; }

    public Reservation(int userId)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        ReservedAt = DateTime.UtcNow;
        ExpiresAt = ReservedAt.AddMinutes(5);
    }

    private Reservation() { }

    public void MarkAsPaid()
    {
        Status = ReservationStatus.Paid;
    }

    public void Expire()
    {
        Status = ReservationStatus.Expired;
    }
}