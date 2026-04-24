using TPTicketingPS.Domain.Enums;


namespace TPTicketingPS.Domain.Entities;

public class Seat
{
    public Guid Id { get; private set; }

    public int SectorId { get; private set; }
    public Sector? Sector { get; private set; }

    public string RowIdentifier { get; private set; } = string.Empty;

    public int SeatNumber { get; private set; }

    public SeatStatus Status { get; private set; } = SeatStatus.Available;

    public Guid? CurrentReservationId { get; private set; }

    public int Version { get; private set; }

    public Seat(int sectorId, string rowIdentifier, int seatNumber)
    {
        Id = Guid.NewGuid();
        SectorId = sectorId;
        RowIdentifier = rowIdentifier;
        SeatNumber = seatNumber;
    }

    private Seat() { }

    public void Reserve(Guid reservationId)
    {
        if (Status != SeatStatus.Available)
            throw new InvalidOperationException($"Seat {Id} is not available (current status: {Status}).");

        Status = SeatStatus.Reserved;
        CurrentReservationId = reservationId;
    }

    public void Release()
    {
        if (Status == SeatStatus.Sold)
            throw new InvalidOperationException("Sold seat cannot be released.");

        Status = SeatStatus.Available;
        CurrentReservationId = null;
    }

    public void MarkAsSold()
    {
        if (Status != SeatStatus.Reserved)
            throw new InvalidOperationException("Seat must be reserved before selling");

        Status = SeatStatus.Sold;
    }
}