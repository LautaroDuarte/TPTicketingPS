using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TPTicketingPS.Application.Common.Interfaces;
using TPTicketingPS.Infrastructure.Auditing;
using TPTicketingPS.Infrastructure.Persistence;
using TPTicketingPS.Infrastructure.BackgroundJobs;
using TPTicketingPS.Infrastructure.Persistence.Repositories;

namespace TPTicketingPS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' no configurada en appsettings.json");

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connectionString, sql =>
            {
                sql.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName);
            }));

        services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
        services.AddScoped<DbInitializer>();

        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddHostedService<ReservationExpirationJob>();

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<ISeatRepository, SeatRepository>();
        services.AddScoped<IEventRepository, EventRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }
}