using Domain.Common;


namespace TPTicketingPS.Domain.Entities;

public class Event : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    public DateTime EventDate { get; private set; }

    public string Venue { get; private set; } = string.Empty;

    public string Status { get; private set; } = "Active";

    public string? Description { get; private set; }

    public int MaxReservationsPerUser { get; private set; } = 3; //default value, se puede cambiar

    // navegación
    public List<Sector> Sectors { get; private set; } = new();

    // constructor para crear eventos válidos
    public Event(string name, DateTime eventDate, string venue, string? description = null, int maxReservationsPerUser = 3)
    {
        if (maxReservationsPerUser < 1)
            throw new ArgumentException("Max reservations must be at least 1.", nameof(maxReservationsPerUser));

        Name = name;
        EventDate = eventDate;
        Venue = venue;
        Description = description;
        MaxReservationsPerUser = maxReservationsPerUser;
    }

    // constructor vacío requerido por EF Core
    private Event() { }

    // comportamiento simple (opcional pero correcto)
    public void ChangeStatus(string status) => Status = status;
    public void UpdateDescription(string description) => Description = description;

    public void UpdateMaxReservationsPerUser(int max)
    {
        if (max < 1)
            throw new InvalidOperationException("Max reservations must be at least 1");

        MaxReservationsPerUser = max;
    }
}
