using EventBoard.Application.Interfaces;
using EventBoard.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventBoard.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IEventService, EventService>();
        services.AddScoped<IRegistrationService, RegistrationService>();
        return services;
    }
}
