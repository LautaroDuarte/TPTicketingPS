using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using TPTicketingPS.Domain.Entities;

namespace TPTicketingPS.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Event> Events { get; }
    DbSet<Sector> Sectors { get; }
    DbSet<Seat> Seats { get; }
    DbSet<User> Users { get; }
    DbSet<Reservation> Reservations { get; }
    DbSet<ReservationItem> ReservationItems { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
}