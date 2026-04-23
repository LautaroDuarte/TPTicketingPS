using Microsoft.EntityFrameworkCore;
using TPTicketingPS.Domain.Entities; // o el namespace real que uses

namespace TPTicketingPS.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Sector> Sectors { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationItem> ReservationItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // EVENT → SECTOR
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Sectors)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId);

            // SECTOR → SEAT
            modelBuilder.Entity<Sector>()
                .HasMany(s => s.Seats)
                .WithOne(s => s.Sector)
                .HasForeignKey(s => s.SectorId);

            // USER → RESERVATIONS
            modelBuilder.Entity<User>()
                .HasMany(u => u.Reservations)
                .WithOne(r => r.User)
                .HasForeignKey(r => r.UserId);

            // RESERVATION → RESERVATION ITEMS
            modelBuilder.Entity<Reservation>()
                .HasMany(r => r.Items)
                .WithOne(i => i.Reservation)
                .HasForeignKey(i => i.ReservationId)
                .OnDelete(DeleteBehavior.Cascade);

            // RESERVATION ITEM → SEAT (CORREGIDO)
            modelBuilder.Entity<ReservationItem>()
                .HasOne(i => i.Seat)
                .WithMany()
                .HasForeignKey(i => i.SeatId);

            // USER → AUDITLOG
            modelBuilder.Entity<User>()
                .HasMany(u => u.AuditLogs)
                .WithOne(a => a.User)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // EMAIL UNIQUE
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // EVENT DEFAULT STATUS
            modelBuilder.Entity<Event>()
                .Property(e => e.Status)
                .HasDefaultValue("Active");
        }
    }
}