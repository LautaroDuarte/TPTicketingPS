using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace TPTicketingPS.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        // Registra todos los validators del assembly
        services.AddValidatorsFromAssembly(assembly);

        // Registra use cases por convención
        var useCases = assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false, IsNested: false });

        foreach (var implementation in useCases)
        {
            var matchingInterface = implementation.GetInterface($"I{implementation.Name}");
            if (matchingInterface is not null)
            {
                services.AddScoped(matchingInterface, implementation);
            }
        }

        return services;
    }
}