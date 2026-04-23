using Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TPTicketingPS.Domain.Entities;

public class Sector : BaseEntity
{
    public int EventId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Capacity { get; private set; }

    // navegación
    public Event? Event { get; private set; }

    public List<Seat> Seats { get; private set; } = new();

    public Sector(int eventId, string name, decimal price, int capacity)
    {
        EventId = eventId;
        Name = name;
        Price = price;
        Capacity = capacity;
    }

    private Sector() { }
}